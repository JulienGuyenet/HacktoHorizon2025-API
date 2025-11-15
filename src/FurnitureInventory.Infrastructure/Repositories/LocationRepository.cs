using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Infrastructure.Data;

namespace FurnitureInventory.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les localisations
/// </summary>
public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(FurnitureInventoryContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Location>> GetByBuildingAsync(string buildingName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.BuildingName == buildingName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetByFloorAsync(string buildingName, string floor, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.BuildingName == buildingName && l.Floor == floor)
            .ToListAsync(cancellationToken);
    }

    public async Task<Location?> GetByDetailsAsync(string buildingName, string? floor, string? room, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.BuildingName == buildingName);

        if (!string.IsNullOrEmpty(floor))
            query = query.Where(l => l.Floor == floor);

        if (!string.IsNullOrEmpty(room))
            query = query.Where(l => l.Room == room);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Location?> GetWithFurnitureAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.Furnitures)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}
