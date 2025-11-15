namespace FurnitureInventory.Core.Entities;

/// <summary>
/// Représente un tag RFID attaché à un meuble
/// </summary>
public class RfidTag
{
    public int Id { get; set; }
    
    /// <summary>
    /// Identifiant unique du tag RFID (EPC ou TID)
    /// </summary>
    public string TagId { get; set; } = string.Empty;
    
    /// <summary>
    /// Type de tag RFID (ex: EPC Gen2, ISO 15693)
    /// </summary>
    public string? TagType { get; set; }
    
    /// <summary>
    /// Statut du tag (Actif, Inactif, Perdu)
    /// </summary>
    public string Status { get; set; } = "Actif";
    
    /// <summary>
    /// Date d'activation du tag
    /// </summary>
    public DateTime? ActivationDate { get; set; }
    
    /// <summary>
    /// Dernière date de lecture du tag
    /// </summary>
    public DateTime? LastReadDate { get; set; }
    
    /// <summary>
    /// ID du dernier lecteur ayant lu le tag
    /// </summary>
    public int? LastReaderId { get; set; }
    
    /// <summary>
    /// Dernier lecteur ayant lu le tag
    /// </summary>
    public RfidReader? LastReader { get; set; }
    
    // Relations
    /// <summary>
    /// Meuble associé à ce tag
    /// </summary>
    public Furniture? Furniture { get; set; }
    
    // Métadonnées
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
