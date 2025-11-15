using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;

namespace FurnitureInventory.Infrastructure.Services;

/// <summary>
/// Service de gestion des meubles
/// </summary>
public class FurnitureService : IFurnitureService
{
    private readonly IFurnitureRepository _furnitureRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IRfidTagRepository _rfidTagRepository;

    public FurnitureService(
        IFurnitureRepository furnitureRepository,
        ILocationRepository locationRepository,
        IRfidTagRepository rfidTagRepository)
    {
        _furnitureRepository = furnitureRepository;
        _locationRepository = locationRepository;
        _rfidTagRepository = rfidTagRepository;
    }

    public async Task<Furniture?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.GetAllWithLocationsAsync(cancellationToken);
    }

    public async Task<Furniture> CreateAsync(Furniture furniture, CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.AddAsync(furniture, cancellationToken);
    }

    public async Task<Furniture> UpdateAsync(Furniture furniture, CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.UpdateAsync(furniture, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Furniture>> SearchAsync(
        string? reference,
        string? famille,
        string? site,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(reference))
            return await _furnitureRepository.GetByReferenceAsync(reference, cancellationToken);

        if (!string.IsNullOrEmpty(famille))
            return await _furnitureRepository.GetByFamilyAsync(famille, cancellationToken);

        if (!string.IsNullOrEmpty(site))
            return await _furnitureRepository.GetBySiteAsync(site, cancellationToken);

        return await _furnitureRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Furniture?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _furnitureRepository.GetByBarcodeAsync(barcode, cancellationToken);
    }

    public async Task<bool> AssignLocationAsync(int furnitureId, int locationId, CancellationToken cancellationToken = default)
    {
        var furniture = await _furnitureRepository.GetByIdAsync(furnitureId, cancellationToken);
        if (furniture == null)
            return false;

        var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location == null)
            return false;

        furniture.LocationId = locationId;
        await _furnitureRepository.UpdateAsync(furniture, cancellationToken);

        return true;
    }

    public async Task<bool> AssignRfidTagAsync(int furnitureId, int rfidTagId, CancellationToken cancellationToken = default)
    {
        var furniture = await _furnitureRepository.GetByIdAsync(furnitureId, cancellationToken);
        if (furniture == null)
            return false;

        var rfidTag = await _rfidTagRepository.GetByIdAsync(rfidTagId, cancellationToken);
        if (rfidTag == null)
            return false;

        furniture.RfidTagId = rfidTagId;
        await _furnitureRepository.UpdateAsync(furniture, cancellationToken);

        return true;
    }
}
