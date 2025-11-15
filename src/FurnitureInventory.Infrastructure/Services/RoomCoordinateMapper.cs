using FurnitureInventory.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FurnitureInventory.Infrastructure.Services;

/// <summary>
/// Service qui génère automatiquement des coordonnées X,Y basées sur le bâtiment, l'étage et la salle
/// </summary>
public class RoomCoordinateMapper : IRoomCoordinateMapper
{
    private readonly ILogger<RoomCoordinateMapper> _logger;
    private readonly Dictionary<string, (double x, double y)> _explicitMappings;

    public RoomCoordinateMapper(ILogger<RoomCoordinateMapper> logger)
    {
        _logger = logger;
        _explicitMappings = new Dictionary<string, (double x, double y)>();
    }

    public (double? x, double? y) GetRoomCoordinates(string buildingName, string? floor, string? room)
    {
        // Normaliser les valeurs
        buildingName = buildingName?.Trim() ?? "";
        floor = floor?.Trim() ?? "";
        room = room?.Trim() ?? "";

        // Vérifier si un mapping explicite existe
        var key = $"{buildingName}|{floor}|{room}";
        if (_explicitMappings.TryGetValue(key, out var coordinates))
        {
            _logger.LogDebug("Utilisation du mapping explicite pour {Key}: ({X}, {Y})", key, coordinates.x, coordinates.y);
            return (coordinates.x, coordinates.y);
        }

        // Sinon, générer automatiquement les coordonnées
        return GenerateCoordinates(buildingName, floor, room);
    }

    private (double? x, double? y) GenerateCoordinates(string buildingName, string? floor, string? room)
    {
        // Si la salle n'est pas définie, retourner null
        if (string.IsNullOrWhiteSpace(room))
        {
            return (null, null);
        }

        try
        {
            // Extraire le numéro d'étage (ex: "4eme etage" -> 4, "1er etage" -> 1, "rdc" -> 0)
            var floorNumber = ExtractFloorNumber(floor);
            
            // Extraire le numéro de salle (ex: "417" -> 417, "201 - Salle de réunion" -> 201)
            var roomNumber = ExtractRoomNumber(room);

            if (roomNumber.HasValue)
            {
                // Algorithme de génération de coordonnées:
                // X = (numéro de salle % 100) * 5 (largeur approximative d'une salle en mètres)
                // Y = étage * 20 (hauteur approximative d'un étage) + (numéro de salle / 100) * 5
                
                var roomUnits = roomNumber.Value % 100; // Dernier deux chiffres
                var roomPrefix = roomNumber.Value / 100; // Premier(s) chiffre(s)
                
                // Calculer X basé sur le numéro de salle (0-100 mètres)
                var x = roomUnits * 5.0;
                
                // Calculer Y basé sur l'étage et le préfixe de la salle
                var y = floorNumber * 20.0 + roomPrefix * 5.0;
                
                _logger.LogDebug("Coordonnées générées pour {Room} étage {Floor}: ({X}, {Y})", 
                    room, floor, x, y);
                
                return (x, y);
            }
            
            // Si on ne peut pas extraire de numéro, essayer d'utiliser un hash pour une position unique
            var hashX = Math.Abs(room.GetHashCode() % 100) * 1.0;
            var hashY = floorNumber * 20.0;
            
            _logger.LogDebug("Coordonnées basées sur hash pour {Room} étage {Floor}: ({X}, {Y})", 
                room, floor, hashX, hashY);
            
            return (hashX, hashY);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lors de la génération de coordonnées pour {Building}/{Floor}/{Room}", 
                buildingName, floor, room);
            return (null, null);
        }
    }

    private int ExtractFloorNumber(string? floor)
    {
        if (string.IsNullOrWhiteSpace(floor))
        {
            return 0;
        }

        // Gérer les cas spéciaux
        var floorLower = floor.ToLowerInvariant();
        if (floorLower.Contains("rdc") || floorLower.Contains("rez"))
        {
            return 0;
        }
        if (floorLower.Contains("sous-sol") || floorLower.Contains("ss"))
        {
            return -1;
        }

        // Extraire le numéro d'étage avec regex
        var match = Regex.Match(floor, @"(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var floorNum))
        {
            return floorNum;
        }

        return 0;
    }

    private int? ExtractRoomNumber(string? room)
    {
        if (string.IsNullOrWhiteSpace(room))
        {
            return null;
        }

        // Extraire le premier nombre trouvé dans la chaîne
        var match = Regex.Match(room, @"(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var roomNum))
        {
            return roomNum;
        }

        return null;
    }

    public Task LoadMappingsAsync()
    {
        // Cette méthode pourrait charger des mappings depuis un fichier de configuration
        // Pour l'instant, on utilise uniquement la génération automatique
        _logger.LogInformation("RoomCoordinateMapper initialisé avec génération automatique de coordonnées");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ajoute un mapping explicite pour une salle spécifique
    /// </summary>
    public void AddMapping(string buildingName, string floor, string room, double x, double y)
    {
        var key = $"{buildingName}|{floor}|{room}";
        _explicitMappings[key] = (x, y);
        _logger.LogInformation("Mapping ajouté: {Key} -> ({X}, {Y})", key, x, y);
    }
}
