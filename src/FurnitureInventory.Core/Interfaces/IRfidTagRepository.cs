using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le repository de tags RFID
/// </summary>
public interface IRfidTagRepository : IRepository<RfidTag>
{
    /// <summary>
    /// Recherche un tag par son identifiant RFID
    /// </summary>
    Task<RfidTag?> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des tags par statut
    /// </summary>
    Task<IEnumerable<RfidTag>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recherche des tags non assignés
    /// </summary>
    Task<IEnumerable<RfidTag>> GetUnassignedTagsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Met à jour la dernière lecture d'un tag
    /// </summary>
    Task UpdateLastReadAsync(string tagId, int readerId, CancellationToken cancellationToken = default);
}
