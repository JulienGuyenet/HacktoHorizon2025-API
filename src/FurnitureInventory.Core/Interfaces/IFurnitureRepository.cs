using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le repository de meubles avec des méthodes spécifiques
/// </summary>
public interface IFurnitureRepository : IRepository<Furniture>
{
    /// <summary>
    /// Recherche des meubles par référence
    /// </summary>
    Task<IEnumerable<Furniture>> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles par code barre
    /// </summary>
    Task<Furniture?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles par numéro de série
    /// </summary>
    Task<Furniture?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles par famille
    /// </summary>
    Task<IEnumerable<Furniture>> GetByFamilyAsync(string famille, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles par site
    /// </summary>
    Task<IEnumerable<Furniture>> GetBySiteAsync(string site, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles par localisation
    /// </summary>
    Task<IEnumerable<Furniture>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles par utilisateur
    /// </summary>
    Task<IEnumerable<Furniture>> GetByUserAsync(string utilisateur, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles avec leurs localisations
    /// </summary>
    Task<IEnumerable<Furniture>> GetAllWithLocationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des meubles avec leurs tags RFID
    /// </summary>
    Task<IEnumerable<Furniture>> GetAllWithRfidTagsAsync(CancellationToken cancellationToken = default);
}
