namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Service pour mapper les salles aux coordonnées X,Y
/// </summary>
public interface IRoomCoordinateMapper
{
    /// <summary>
    /// Obtient les coordonnées X,Y pour une salle donnée
    /// </summary>
    /// <param name="buildingName">Nom du bâtiment</param>
    /// <param name="floor">Étage</param>
    /// <param name="room">Numéro ou nom de la salle</param>
    /// <returns>Tuple (X, Y) ou null si aucune coordonnée n'est trouvée</returns>
    (double? x, double? y) GetRoomCoordinates(string buildingName, string? floor, string? room);
    
    /// <summary>
    /// Charge les mappings depuis la configuration
    /// </summary>
    Task LoadMappingsAsync();
}
