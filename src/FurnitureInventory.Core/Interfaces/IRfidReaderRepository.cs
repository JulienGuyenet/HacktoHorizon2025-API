using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le repository de lecteurs RFID
/// </summary>
public interface IRfidReaderRepository : IRepository<RfidReader>
{
    /// <summary>
    /// Recherche un lecteur par son identifiant
    /// </summary>
    Task<RfidReader?> GetByReaderIdAsync(string readerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des lecteurs par localisation
    /// </summary>
    Task<IEnumerable<RfidReader>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des lecteurs par statut
    /// </summary>
    Task<IEnumerable<RfidReader>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Met à jour le statut d'un lecteur
    /// </summary>
    Task UpdateStatusAsync(string readerId, string status, CancellationToken cancellationToken = default);
}
