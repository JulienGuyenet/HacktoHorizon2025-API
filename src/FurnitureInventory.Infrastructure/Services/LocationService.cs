using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;

namespace FurnitureInventory.Infrastructure.Services;

/// <summary>
/// Service de gestion des localisations
/// </summary>
public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IFurnitureRepository _furnitureRepository;

    public LocationService(
        ILocationRepository locationRepository,
        IFurnitureRepository furnitureRepository)
    {
        _locationRepository = locationRepository;
        _furnitureRepository = furnitureRepository;
    }

    public async Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _locationRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _locationRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Location> CreateAsync(Location location, CancellationToken cancellationToken = default)
    {
        return await _locationRepository.AddAsync(location, cancellationToken);
    }

    public async Task<Location> UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        return await _locationRepository.UpdateAsync(location, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _locationRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetByBuildingAsync(string buildingName, CancellationToken cancellationToken = default)
    {
        return await _locationRepository.GetByBuildingAsync(buildingName, cancellationToken);
    }

    public async Task<Location?> GetWithFurnitureAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _locationRepository.GetWithFurnitureAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetFurnitureAtLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.GetByLocationAsync(locationId, cancellationToken);
    }
}
