namespace FurnitureInventory.Core.Entities;

/// <summary>
/// Représente un meuble dans l'inventaire
/// </summary>
public class Furniture
{
    public int Id { get; set; }
    
    /// <summary>
    /// Référence unique du meuble
    /// </summary>
    public string Reference { get; set; } = string.Empty;
    
    /// <summary>
    /// Désignation (nom/description) du meuble
    /// </summary>
    public string Designation { get; set; } = string.Empty;
    
    /// <summary>
    /// Famille du meuble (ex: Bureau, Chaise, Armoire)
    /// </summary>
    public string? Famille { get; set; }
    
    /// <summary>
    /// Type de meuble
    /// </summary>
    public string? Type { get; set; }
    
    /// <summary>
    /// Fournisseur du meuble
    /// </summary>
    public string? Fournisseur { get; set; }
    
    /// <summary>
    /// Utilisateur actuel du meuble
    /// </summary>
    public string? Utilisateur { get; set; }
    
    /// <summary>
    /// Code barre pour identification
    /// </summary>
    public string? CodeBarre { get; set; }
    
    /// <summary>
    /// Numéro de série du meuble
    /// </summary>
    public string? NumeroSerie { get; set; }
    
    /// <summary>
    /// Informations complémentaires
    /// </summary>
    public string? Informations { get; set; }
    
    /// <summary>
    /// Site où se trouve le meuble
    /// </summary>
    public string? Site { get; set; }
    
    /// <summary>
    /// Date de livraison du meuble
    /// </summary>
    public DateTime? DateLivraison { get; set; }
    
    // Relations
    /// <summary>
    /// ID de la localisation actuelle
    /// </summary>
    public int? LocationId { get; set; }
    
    /// <summary>
    /// Localisation actuelle du meuble
    /// </summary>
    public Location? Location { get; set; }
    
    /// <summary>
    /// ID du tag RFID associé
    /// </summary>
    public int? RfidTagId { get; set; }
    
    /// <summary>
    /// Tag RFID associé au meuble
    /// </summary>
    public RfidTag? RfidTag { get; set; }
    
    // Métadonnées
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
