using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le service de gestion des meubles
/// </summary>
public interface IFurnitureService
{
    Task<Furniture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Furniture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Furniture> CreateAsync(Furniture furniture, CancellationToken cancellationToken = default);
    Task<Furniture> UpdateAsync(Furniture furniture, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Furniture>> SearchAsync(string? reference, string? famille, string? site, CancellationToken cancellationToken = default);
    Task<Furniture?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<bool> AssignLocationAsync(int furnitureId, int locationId, CancellationToken cancellationToken = default);
    Task<bool> AssignRfidTagAsync(int furnitureId, int rfidTagId, CancellationToken cancellationToken = default);
}
