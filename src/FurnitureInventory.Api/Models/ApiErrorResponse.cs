namespace FurnitureInventory.Api.Models;

/// <summary>
/// Standardized error response structure for API errors
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Error code that can be mapped to localized messages on the client side
    /// Examples: "FURNITURE_NOT_FOUND", "INVALID_FILE_FORMAT", "EMAIL_TAKEN"
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional error message in English (for debugging purposes)
    /// Should NOT be displayed directly to end users
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Optional additional details about the error
    /// </summary>
    public object? Details { get; set; }
}
