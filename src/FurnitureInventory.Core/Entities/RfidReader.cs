namespace FurnitureInventory.Core.Entities;

/// <summary>
/// Représente un lecteur RFID installé dans un bâtiment
/// </summary>
public class RfidReader
{
    public int Id { get; set; }
    
    /// <summary>
    /// Identifiant unique du lecteur
    /// </summary>
    public string ReaderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Nom du lecteur
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Modèle du lecteur RFID
    /// </summary>
    public string? Model { get; set; }
    
    /// <summary>
    /// Adresse IP du lecteur (si applicable)
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Port de communication
    /// </summary>
    public int? Port { get; set; }
    
    /// <summary>
    /// Statut du lecteur (En ligne, Hors ligne, En maintenance)
    /// </summary>
    public string Status { get; set; } = "Hors ligne";
    
    /// <summary>
    /// Puissance d'émission du lecteur (en dBm)
    /// </summary>
    public double? PowerLevel { get; set; }
    
    /// <summary>
    /// Portée approximative du lecteur (en mètres)
    /// </summary>
    public double? Range { get; set; }
    
    // Relations
    /// <summary>
    /// ID de la localisation du lecteur
    /// </summary>
    public int? LocationId { get; set; }
    
    /// <summary>
    /// Localisation du lecteur
    /// </summary>
    public Location? Location { get; set; }
    
    /// <summary>
    /// Tags RFID lus par ce lecteur
    /// </summary>
    public ICollection<RfidTag> RfidTags { get; set; } = new List<RfidTag>();
    
    // Métadonnées
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastSeenAt { get; set; }
}
