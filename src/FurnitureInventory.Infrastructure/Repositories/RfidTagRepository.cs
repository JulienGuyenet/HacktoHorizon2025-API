using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Infrastructure.Data;

namespace FurnitureInventory.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les tags RFID
/// </summary>
public class RfidTagRepository : Repository<RfidTag>, IRfidTagRepository
{
    public RfidTagRepository(FurnitureInventoryContext context) : base(context)
    {
    }

    public async Task<RfidTag?> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Furniture)
            .Include(t => t.LastReader)
            .FirstOrDefaultAsync(t => t.TagId == tagId, cancellationToken);
    }

    public async Task<IEnumerable<RfidTag>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .Include(t => t.Furniture)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RfidTag>> GetUnassignedTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Furniture == null)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateLastReadAsync(string tagId, int readerId, CancellationToken cancellationToken = default)
    {
        var tag = await _dbSet.FirstOrDefaultAsync(t => t.TagId == tagId, cancellationToken);
        if (tag != null)
        {
            tag.LastReadDate = DateTime.UtcNow;
            tag.LastReaderId = readerId;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
