using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le repository de localisations
/// </summary>
public interface ILocationRepository : IRepository<Location>
{
    /// <summary>
    /// Recherche des localisations par bâtiment
    /// </summary>
    Task<IEnumerable<Location>> GetByBuildingAsync(string buildingName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des localisations par étage
    /// </summary>
    Task<IEnumerable<Location>> GetByFloorAsync(string buildingName, string floor, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche une localisation spécifique
    /// </summary>
    Task<Location?> GetByDetailsAsync(string buildingName, string? floor, string? room, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère une localisation avec tous ses meubles
    /// </summary>
    Task<Location?> GetWithFurnitureAsync(int id, CancellationToken cancellationToken = default);
}
