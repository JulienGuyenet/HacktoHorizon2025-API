namespace FurnitureInventory.Core.Interfaces;

/// <summary>
/// Interface pour l'importation de données Excel
/// </summary>
public interface IExcelImportService
{
    /// <summary>
    /// Importe des meubles depuis un fichier Excel
    /// </summary>
    /// <param name="filePath">Chemin du fichier Excel</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Nombre de meubles importés</returns>
    Task<int> ImportFurnitureFromExcelAsync(string filePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Importe des meubles depuis un stream Excel
    /// </summary>
    /// <param name="stream">Stream contenant les données Excel</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Nombre de meubles importés</returns>
    Task<int> ImportFurnitureFromExcelAsync(Stream stream, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Valide la structure d'un fichier Excel avant l'import
    /// </summary>
    /// <param name="filePath">Chemin du fichier Excel</param>
    /// <returns>True si le fichier est valide</returns>
    Task<bool> ValidateExcelFileAsync(string filePath);
}
