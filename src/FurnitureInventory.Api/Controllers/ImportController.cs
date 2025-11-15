using Microsoft.AspNetCore.Mvc;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Api.Models;
using Microsoft.Extensions.Localization;
using FurnitureInventory.Api.Resources;

namespace FurnitureInventory.Api.Controllers;

/// <summary>
/// Contrôleur pour l'importation de données
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IExcelImportService _importService;
    private readonly ILogger<ImportController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ImportController(
        IExcelImportService importService, 
        ILogger<ImportController> logger,
        IWebHostEnvironment environment,
        IStringLocalizer<SharedResources> localizer)
    {
        _importService = importService;
        _logger = logger;
        _environment = environment;
        _localizer = localizer;
    }

    /// <summary>
    /// Importe les données depuis le fichier CSV par défaut
    /// </summary>
    /// <returns>Nombre de meubles importés</returns>
    [HttpPost("default")]
    [ProducesResponseType(typeof(ImportResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ImportDefaultData(CancellationToken cancellationToken)
    {
        try
        {
            // Chemin vers le fichier CSV par défaut
            var projectRoot = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", ".."));
            var csvPath = Path.Combine(projectRoot, "docs", "data", "VIOTTE_Inventaire_20251114.csv");

            if (!System.IO.File.Exists(csvPath))
            {
                _logger.LogWarning("Fichier CSV non trouvé: {Path}", csvPath);
                return NotFound(new ApiErrorResponse
                {
                    ErrorCode = ErrorCodes.FILE_NOT_FOUND,
                    Message = "Default CSV file not found",
                    Details = new { path = csvPath }
                });
            }

            _logger.LogInformation("Début de l'import depuis: {Path}", csvPath);
            var count = await _importService.ImportFurnitureFromExcelAsync(csvPath, cancellationToken);

            // Use localized message for server-generated content
            var message = _localizer["Notification.ImportSuccess", count].Value;

            return Ok(new ImportResult 
            { 
                Success = true, 
                ImportedCount = count,
                Message = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'import des données par défaut");
            return StatusCode(500, new ApiErrorResponse
            {
                ErrorCode = ErrorCodes.IMPORT_ERROR,
                Message = $"Import error: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Importe les données depuis un fichier Excel/CSV uploadé
    /// </summary>
    /// <param name="file">Fichier Excel ou CSV à importer</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Nombre de meubles importés</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ImportResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ImportFromUpload(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorCode = ErrorCodes.FILE_NOT_PROVIDED,
                    Message = "No file provided"
                });
            }

            // Valider l'extension du fichier
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls" && extension != ".csv")
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorCode = ErrorCodes.INVALID_FILE_FORMAT,
                    Message = "Unsupported file format. Use .xlsx, .xls or .csv",
                    Details = new { supportedFormats = new[] { ".xlsx", ".xls", ".csv" } }
                });
            }

            _logger.LogInformation("Début de l'import depuis le fichier uploadé: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var count = await _importService.ImportFurnitureFromExcelAsync(stream, cancellationToken);

            // Use localized message for server-generated content
            var message = _localizer["Notification.ImportUploadSuccess", count, file.FileName].Value;

            return Ok(new ImportResult 
            { 
                Success = true, 
                ImportedCount = count,
                Message = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'import du fichier uploadé");
            return StatusCode(500, new ApiErrorResponse
            {
                ErrorCode = ErrorCodes.IMPORT_ERROR,
                Message = $"Import error: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Valide un fichier Excel/CSV sans l'importer
    /// </summary>
    /// <param name="file">Fichier Excel ou CSV à valider</param>
    /// <returns>Résultat de la validation</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorCode = ErrorCodes.FILE_NOT_PROVIDED,
                    Message = "No file provided"
                });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls" && extension != ".csv")
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorCode = ErrorCodes.INVALID_FILE_FORMAT,
                    Message = "Unsupported file format. Use .xlsx, .xls or .csv",
                    Details = new { supportedFormats = new[] { ".xlsx", ".xls", ".csv" } }
                });
            }

            // Sauvegarder temporairement pour valider
            var tempPath = Path.Combine(Path.GetTempPath(), $"validate_{Guid.NewGuid()}{extension}");
            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var isValid = await _importService.ValidateExcelFileAsync(tempPath);

                // Use localized messages for server-generated content
                var validMessage = _localizer["Server.FileValidation.Valid"].Value;
                var invalidMessage = _localizer["Server.FileValidation.Invalid"].Value;

                return Ok(new ValidationResult 
                { 
                    IsValid = isValid, 
                    Message = isValid ? validMessage : invalidMessage
                });
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du fichier");
            return Ok(new ApiErrorResponse
            {
                ErrorCode = ErrorCodes.VALIDATION_FAILED,
                Message = $"Validation error: {ex.Message}"
            });
        }
    }
}

/// <summary>
/// Résultat d'une opération d'import
/// </summary>
public class ImportResult
{
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Résultat d'une validation de fichier
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
