using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le service de localisation
/// </summary>
public interface ILocationService
{
    Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Location> CreateAsync(Location location, CancellationToken cancellationToken = default);
    Task<Location> UpdateAsync(Location location, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetByBuildingAsync(string buildingName, CancellationToken cancellationToken = default);
    Task<Location?> GetWithFurnitureAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Furniture>> GetFurnitureAtLocationAsync(int locationId, CancellationToken cancellationToken = default);
}
