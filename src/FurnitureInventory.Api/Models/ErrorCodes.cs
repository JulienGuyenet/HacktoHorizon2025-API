namespace FurnitureInventory.Api.Models;

/// <summary>
/// Standard error codes used across the API
/// These codes should be mapped to localized messages on the client side
/// </summary>
public static class ErrorCodes
{
    // General errors
    public const string INVALID_REQUEST = "INVALID_REQUEST";
    public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    
    // Resource not found errors
    public const string FURNITURE_NOT_FOUND = "FURNITURE_NOT_FOUND";
    public const string LOCATION_NOT_FOUND = "LOCATION_NOT_FOUND";
    public const string RFID_TAG_NOT_FOUND = "RFID_TAG_NOT_FOUND";
    
    // Import errors
    public const string FILE_NOT_PROVIDED = "FILE_NOT_PROVIDED";
    public const string INVALID_FILE_FORMAT = "INVALID_FILE_FORMAT";
    public const string FILE_NOT_FOUND = "FILE_NOT_FOUND";
    public const string IMPORT_ERROR = "IMPORT_ERROR";
    public const string VALIDATION_FAILED = "VALIDATION_FAILED";
    
    // Business logic errors
    public const string ID_MISMATCH = "ID_MISMATCH";
    public const string OPERATION_FAILED = "OPERATION_FAILED";
}
