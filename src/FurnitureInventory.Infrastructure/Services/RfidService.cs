using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;

namespace FurnitureInventory.Infrastructure.Services;

/// <summary>
/// Service de gestion RFID
/// </summary>
public class RfidService : IRfidService
{
    private readonly IRfidTagRepository _rfidTagRepository;
    private readonly IRfidReaderRepository _rfidReaderRepository;
    private readonly IFurnitureRepository _furnitureRepository;

    public RfidService(
        IRfidTagRepository rfidTagRepository,
        IRfidReaderRepository rfidReaderRepository,
        IFurnitureRepository furnitureRepository)
    {
        _rfidTagRepository = rfidTagRepository;
        _rfidReaderRepository = rfidReaderRepository;
        _furnitureRepository = furnitureRepository;
    }

    public async Task<RfidTag> RegisterTagAsync(string tagId, string? tagType, CancellationToken cancellationToken = default)
    {
        var tag = new RfidTag
        {
            TagId = tagId,
            TagType = tagType,
            Status = "Actif",
            ActivationDate = DateTime.UtcNow
        };

        return await _rfidTagRepository.AddAsync(tag, cancellationToken);
    }

    public async Task<bool> AssignTagToFurnitureAsync(string tagId, int furnitureId, CancellationToken cancellationToken = default)
    {
        var tag = await _rfidTagRepository.GetByTagIdAsync(tagId, cancellationToken);
        if (tag == null)
            return false;

        var furniture = await _furnitureRepository.GetByIdAsync(furnitureId, cancellationToken);
        if (furniture == null)
            return false;

        furniture.RfidTagId = tag.Id;
        await _furnitureRepository.UpdateAsync(furniture, cancellationToken);

        return true;
    }

    public async Task ProcessTagReadAsync(string tagId, string readerId, CancellationToken cancellationToken = default)
    {
        var reader = await _rfidReaderRepository.GetByReaderIdAsync(readerId, cancellationToken);
        if (reader == null)
            return;

        await _rfidTagRepository.UpdateLastReadAsync(tagId, reader.Id, cancellationToken);

        // Mettre à jour la localisation du meuble si le lecteur a une localisation
        if (reader.LocationId.HasValue)
        {
            var tag = await _rfidTagRepository.GetByTagIdAsync(tagId, cancellationToken);
            if (tag?.Furniture != null)
            {
                var furniture = await _furnitureRepository.GetByIdAsync(tag.Furniture.Id, cancellationToken);
                if (furniture != null)
                {
                    furniture.LocationId = reader.LocationId;
                    await _furnitureRepository.UpdateAsync(furniture, cancellationToken);
                }
            }
        }
    }

    public async Task<IEnumerable<RfidTag>> GetActiveTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _rfidTagRepository.GetByStatusAsync("Actif", cancellationToken);
    }

    public async Task<RfidTag?> GetTagByIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        return await _rfidTagRepository.GetByTagIdAsync(tagId, cancellationToken);
    }

    public async Task<bool> DeactivateTagAsync(string tagId, CancellationToken cancellationToken = default)
    {
        var tag = await _rfidTagRepository.GetByTagIdAsync(tagId, cancellationToken);
        if (tag == null)
            return false;

        tag.Status = "Inactif";
        await _rfidTagRepository.UpdateAsync(tag, cancellationToken);

        return true;
    }

    public async Task<RfidReader> RegisterReaderAsync(string readerId, string name, int? locationId, CancellationToken cancellationToken = default)
    {
        var reader = new RfidReader
        {
            ReaderId = readerId,
            Name = name,
            LocationId = locationId,
            Status = "En ligne"
        };

        return await _rfidReaderRepository.AddAsync(reader, cancellationToken);
    }

    public async Task UpdateReaderStatusAsync(string readerId, string status, CancellationToken cancellationToken = default)
    {
        await _rfidReaderRepository.UpdateStatusAsync(readerId, status, cancellationToken);
    }

    public async Task<IEnumerable<RfidReader>> GetActiveReadersAsync(CancellationToken cancellationToken = default)
    {
        return await _rfidReaderRepository.GetByStatusAsync("En ligne", cancellationToken);
    }
}
