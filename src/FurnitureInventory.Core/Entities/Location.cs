namespace FurnitureInventory.Core.Entities;

/// <summary>
/// Représente une localisation dans un bâtiment
/// </summary>
public class Location
{
    public int Id { get; set; }
    
    /// <summary>
    /// Nom du bâtiment
    /// </summary>
    public string BuildingName { get; set; } = string.Empty;
    
    /// <summary>
    /// Numéro ou nom de l'étage
    /// </summary>
    public string? Floor { get; set; }
    
    /// <summary>
    /// Numéro ou nom de la salle/pièce
    /// </summary>
    public string? Room { get; set; }
    
    /// <summary>
    /// Zone spécifique dans la pièce (ex: "près de la fenêtre")
    /// </summary>
    public string? Zone { get; set; }
    
    /// <summary>
    /// Description complète de la localisation
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Coordonnées GPS - Latitude
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Coordonnées GPS - Longitude
    /// </summary>
    public double? Longitude { get; set; }
    
    // Relations
    /// <summary>
    /// Meubles présents à cette localisation
    /// </summary>
    public ICollection<Furniture> Furnitures { get; set; } = new List<Furniture>();
    
    /// <summary>
    /// Lecteurs RFID à cette localisation
    /// </summary>
    public ICollection<RfidReader> RfidReaders { get; set; } = new List<RfidReader>();
    
    // Métadonnées
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
