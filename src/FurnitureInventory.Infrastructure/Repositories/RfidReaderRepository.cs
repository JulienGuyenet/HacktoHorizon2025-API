using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Infrastructure.Data;

namespace FurnitureInventory.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les lecteurs RFID
/// </summary>
public class RfidReaderRepository : Repository<RfidReader>, IRfidReaderRepository
{
    public RfidReaderRepository(FurnitureInventoryContext context) : base(context)
    {
    }

    public async Task<RfidReader?> GetByReaderIdAsync(string readerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Location)
            .FirstOrDefaultAsync(r => r.ReaderId == readerId, cancellationToken);
    }

    public async Task<IEnumerable<RfidReader>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.LocationId == locationId)
            .Include(r => r.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RfidReader>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Status == status)
            .Include(r => r.Location)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(string readerId, string status, CancellationToken cancellationToken = default)
    {
        var reader = await _dbSet.FirstOrDefaultAsync(r => r.ReaderId == readerId, cancellationToken);
        if (reader != null)
        {
            reader.Status = status;
            reader.LastSeenAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
