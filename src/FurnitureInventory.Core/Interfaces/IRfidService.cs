using FurnitureInventory.Core.Entities;

namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour le service de gestion RFID
/// Permet l'intégration avec différents systèmes RFID
/// </summary>
public interface IRfidService
{
    /// <summary>
    /// Enregistre un nouveau tag RFID
    /// </summary>
    Task<RfidTag> RegisterTagAsync(string tagId, string? tagType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Associe un tag RFID à un meuble
    /// </summary>
    Task<bool> AssignTagToFurnitureAsync(string tagId, int furnitureId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Traite une lecture de tag RFID
    /// </summary>
    Task ProcessTagReadAsync(string tagId, string readerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère tous les tags actifs
    /// </summary>
    Task<IEnumerable<RfidTag>> GetActiveTagsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère un tag par son identifiant
    /// </summary>
    Task<RfidTag?> GetTagByIdAsync(string tagId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Désactive un tag RFID
    /// </summary>
    Task<bool> DeactivateTagAsync(string tagId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Enregistre un nouveau lecteur RFID
    /// </summary>
    Task<RfidReader> RegisterReaderAsync(string readerId, string name, int? locationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Met à jour le statut d'un lecteur
    /// </summary>
    Task UpdateReaderStatusAsync(string readerId, string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère tous les lecteurs actifs
    /// </summary>
    Task<IEnumerable<RfidReader>> GetActiveReadersAsync(CancellationToken cancellationToken = default);
}
