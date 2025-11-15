using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Infrastructure.Data;

namespace FurnitureInventory.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les meubles
/// </summary>
public class FurnitureRepository : Repository<Furniture>, IFurnitureRepository
{
    public FurnitureRepository(FurnitureInventoryContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Furniture>> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Reference.Contains(reference))
            .Include(f => f.Location)
            .Include(f => f.RfidTag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Furniture?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Location)
            .Include(f => f.RfidTag)
            .FirstOrDefaultAsync(f => f.CodeBarre == barcode, cancellationToken);
    }

    public async Task<Furniture?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Location)
            .Include(f => f.RfidTag)
            .FirstOrDefaultAsync(f => f.NumeroSerie == serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetByFamilyAsync(string famille, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Famille == famille)
            .Include(f => f.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetBySiteAsync(string site, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Site == site)
            .Include(f => f.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.LocationId == locationId)
            .Include(f => f.Location)
            .Include(f => f.RfidTag)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetByUserAsync(string utilisateur, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Utilisateur == utilisateur)
            .Include(f => f.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetAllWithLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetAllWithRfidTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.RfidTag)
            .ToListAsync(cancellationToken);
    }
}
