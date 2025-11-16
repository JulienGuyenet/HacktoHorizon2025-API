using System.Globalization;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace FurnitureInventory.Infrastructure.Services;

/// <summary>
/// Service d'importation de données depuis des fichiers Excel/CSV
/// </summary>
public class ExcelImportService : IExcelImportService
{
    private readonly IFurnitureRepository _furnitureRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IRoomCoordinateMapper _roomCoordinateMapper;
    private readonly ILogger<ExcelImportService> _logger;

    public ExcelImportService(
        IFurnitureRepository furnitureRepository,
        ILocationRepository locationRepository,
        IRoomCoordinateMapper roomCoordinateMapper,
        ILogger<ExcelImportService> logger)
    {
        _furnitureRepository = furnitureRepository;
        _locationRepository = locationRepository;
        _roomCoordinateMapper = roomCoordinateMapper;
        _logger = logger;
        
        // Configure EPPlus license context
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<int> ImportFurnitureFromExcelAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Le fichier {filePath} n'existe pas");
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        if (extension == ".csv")
        {
            return await ImportFurnitureFromCsvAsync(filePath, cancellationToken);
        }
        else
        {
            using var stream = File.OpenRead(filePath);
            return await ImportFurnitureFromExcelAsync(stream, cancellationToken);
        }
    }

    public async Task<int> ImportFurnitureFromExcelAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var importedCount = 0;
        var locationCache = new Dictionary<string, Location>();

        try
        {
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0]; // Premier onglet
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount <= 1)
            {
                _logger.LogWarning("Le fichier Excel ne contient pas de données");
                return 0;
            }

            // Lire les en-têtes (ligne 1)
            var headers = new Dictionary<string, int>();
            if (worksheet.Dimension != null)
            {
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    var header = worksheet.Cells[1, col].Text?.Trim();
                    if (!string.IsNullOrEmpty(header))
                    {
                        headers[header] = col;
                    }
                }
            }

            // Traiter chaque ligne de données
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var furniture = await ParseFurnitureFromRow(worksheet, row, headers, locationCache, cancellationToken);
                    if (furniture != null)
                    {
                        await _furnitureRepository.AddAsync(furniture, cancellationToken);
                        importedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erreur lors de l'import de la ligne {Row}", row);
                }
            }

            _logger.LogInformation("Import terminé : {Count} meubles importés", importedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'import Excel");
            throw;
        }

        return importedCount;
    }

    public async Task<bool> ValidateExcelFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            using var stream = File.OpenRead(filePath);
            using var package = new ExcelPackage(stream);
            
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null || worksheet.Dimension == null)
            {
                return false;
            }

            // Vérifier les en-têtes requis
            var requiredHeaders = new[] { "Référence", "Désignation" };
            var headers = new List<string>();
            
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (!string.IsNullOrEmpty(header))
                {
                    headers.Add(header);
                }
            }

            return requiredHeaders.All(h => headers.Contains(h));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du fichier Excel");
            return false;
        }
    }

    private async Task<Furniture?> ParseFurnitureFromRow(
        ExcelWorksheet worksheet, 
        int row, 
        Dictionary<string, int> headers,
        Dictionary<string, Location> locationCache,
        CancellationToken cancellationToken)
    {
        var reference = GetCellValue(worksheet, row, headers, "Référence");
        var designation = GetCellValue(worksheet, row, headers, "Désignation");

        if (string.IsNullOrWhiteSpace(reference) || string.IsNullOrWhiteSpace(designation))
        {
            return null;
        }

        var furniture = new Furniture
        {
            Reference = reference,
            Designation = designation,
            Famille = GetCellValue(worksheet, row, headers, "Famille"),
            Type = GetCellValue(worksheet, row, headers, "Type"),
            Fournisseur = GetCellValue(worksheet, row, headers, "Fournisseur"),
            Utilisateur = GetCellValue(worksheet, row, headers, "Utilisateur"),
            CodeBarre = GetCellValue(worksheet, row, headers, "Code barre"),
            NumeroSerie = GetCellValue(worksheet, row, headers, "N° série"),
            Informations = GetCellValue(worksheet, row, headers, "Informations"),
            Site = GetCellValue(worksheet, row, headers, "Site"),
            CreatedAt = DateTime.UtcNow
        };

        // Parse date de livraison
        var dateLivraison = GetCellValue(worksheet, row, headers, "Date de livraison");
        if (!string.IsNullOrWhiteSpace(dateLivraison) && DateTime.TryParse(dateLivraison, out var date))
        {
            furniture.DateLivraison = date;
        }

        // Parse et créer la localisation depuis le champ Site
        var siteValue = GetCellValue(worksheet, row, headers, "Site");
        if (!string.IsNullOrWhiteSpace(siteValue))
        {
            var location = await GetOrCreateLocationAsync(siteValue, locationCache, cancellationToken);
            if (location != null)
            {
                furniture.LocationId = location.Id;
                // Assigner les coordonnées de la salle au meuble
                furniture.PositionX = location.PositionX;
                furniture.PositionY = location.PositionY;
            }
        }

        return furniture;
    }

    private async Task<Location?> GetOrCreateLocationAsync(
        string siteValue, 
        Dictionary<string, Location> locationCache,
        CancellationToken cancellationToken)
    {
        if (locationCache.TryGetValue(siteValue, out var cachedLocation))
        {
            return cachedLocation;
        }

        // Parser le format: "25\BESANCON\Siege\VIOTTE\4eme etage\417"
        var parts = siteValue.Split('\\', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length < 2)
        {
            return null;
        }

        var buildingName = parts.Length > 3 ? parts[3] : parts[1]; // VIOTTE ou BESANCON
        var floor = parts.Length > 4 ? parts[4] : null; // 4eme etage
        var room = parts.Length > 5 ? parts[5] : null; // 417

        // Obtenir les coordonnées pour cette salle
        var (posX, posY) = _roomCoordinateMapper.GetRoomCoordinates(buildingName, floor, room);

        var location = new Location
        {
            BuildingName = buildingName,
            Floor = floor,
            Room = room,
            Description = siteValue,
            PositionX = posX,
            PositionY = posY,
            CreatedAt = DateTime.UtcNow
        };

        await _locationRepository.AddAsync(location, cancellationToken);
        locationCache[siteValue] = location;

        return location;
    }

    private string GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, string headerName)
    {
        if (headers.TryGetValue(headerName, out var col))
        {
            return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }

    private async Task<int> ImportFurnitureFromCsvAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var importedCount = 0;
        var locationCache = new Dictionary<string, Location>();

        try
        {
            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            
            if (lines.Length <= 1)
            {
                _logger.LogWarning("Le fichier CSV ne contient pas de données");
                return 0;
            }

            // Lire les en-têtes (première ligne) avec séparateur point-virgule
            var headerLine = lines[0];
            var headerColumns = headerLine.Split(';');
            var headers = new Dictionary<string, int>();
            
            for (int i = 0; i < headerColumns.Length; i++)
            {
                var header = headerColumns[i].Trim();
                if (!string.IsNullOrEmpty(header))
                {
                    headers[header] = i;
                }
            }

            // Traiter chaque ligne de données
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                try
                {
                    var line = lines[lineIndex];
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var furniture = await ParseFurnitureFromCsvLine(line, headers, locationCache, cancellationToken);
                    if (furniture != null)
                    {
                        await _furnitureRepository.AddAsync(furniture, cancellationToken);
                        importedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erreur lors de l'import de la ligne {LineIndex}", lineIndex + 1);
                }
            }

            _logger.LogInformation("Import CSV terminé : {Count} meubles importés", importedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'import CSV");
            throw;
        }

        return importedCount;
    }

    private async Task<Furniture?> ParseFurnitureFromCsvLine(
        string line,
        Dictionary<string, int> headers,
        Dictionary<string, Location> locationCache,
        CancellationToken cancellationToken)
    {
        // Split sur point-virgule
        var columns = line.Split(';');

        var reference = GetCsvValue(columns, headers, "Référence");
        var designation = GetCsvValue(columns, headers, "Désignation");

        if (string.IsNullOrWhiteSpace(reference) || string.IsNullOrWhiteSpace(designation))
        {
            return null;
        }

        var furniture = new Furniture
        {
            Reference = reference,
            Designation = designation,
            Famille = GetCsvValue(columns, headers, "Famille"),
            Type = GetCsvValue(columns, headers, "Type"),
            Fournisseur = GetCsvValue(columns, headers, "Fournisseur"),
            Utilisateur = GetCsvValue(columns, headers, "Utilisateur"),
            CodeBarre = GetCsvValue(columns, headers, "Code barre"),
            NumeroSerie = GetCsvValue(columns, headers, "N° série"),
            Informations = GetCsvValue(columns, headers, "Informations").TrimEnd(),
            Site = GetCsvValue(columns, headers, "Site"),
            CreatedAt = DateTime.UtcNow
        };

        // Parse date de livraison
        var dateLivraison = GetCsvValue(columns, headers, "Date de livraison");
        if (!string.IsNullOrWhiteSpace(dateLivraison) && DateTime.TryParse(dateLivraison, out var date))
        {
            furniture.DateLivraison = date;
        }

        // Parse et créer la localisation depuis le champ Site
        var siteValue = GetCsvValue(columns, headers, "Site");
        if (!string.IsNullOrWhiteSpace(siteValue))
        {
            var location = await GetOrCreateLocationAsync(siteValue, locationCache, cancellationToken);
            if (location != null)
            {
                furniture.LocationId = location.Id;
                // Assigner les coordonnées de la salle au meuble
                furniture.PositionX = location.PositionX;
                furniture.PositionY = location.PositionY;
            }
        }

        return furniture;
    }

    private string GetCsvValue(string[] columns, Dictionary<string, int> headers, string headerName)
    {
        if (headers.TryGetValue(headerName, out var index) && index < columns.Length)
        {
            return columns[index]?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }
}
