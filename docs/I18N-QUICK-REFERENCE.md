# Backend i18n Quick Reference

## For API Error Responses

### DON'T ❌
```csharp
// Don't return hardcoded messages
return NotFound("Furniture not found");
return BadRequest("Invalid file format");
```

### DO ✅
```csharp
// Return error codes with optional debug message
return NotFound(new ApiErrorResponse 
{ 
    ErrorCode = ErrorCodes.FURNITURE_NOT_FOUND,
    Message = "Furniture not found", // For debugging only
    Details = new { id = furnitureId }
});

return BadRequest(new ApiErrorResponse
{
    ErrorCode = ErrorCodes.INVALID_FILE_FORMAT,
    Message = "Unsupported file format",
    Details = new { supportedFormats = new[] { ".xlsx", ".xls", ".csv" } }
});
```

## For Server-Generated Content (Emails, Notifications, PDFs)

### Setup in Controller/Service
```csharp
public class MyController : ControllerBase
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    
    public MyController(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }
}
```

### Usage
```csharp
// Simple translation
var message = _localizer["Notification.FurnitureAdded"].Value;

// With parameters
var message = _localizer["Notification.ImportSuccess", count].Value;
var message = _localizer["Notification.ImportUploadSuccess", count, fileName].Value;
```

## Available Error Codes

```csharp
ErrorCodes.FURNITURE_NOT_FOUND
ErrorCodes.LOCATION_NOT_FOUND
ErrorCodes.RFID_TAG_NOT_FOUND
ErrorCodes.FILE_NOT_PROVIDED
ErrorCodes.INVALID_FILE_FORMAT
ErrorCodes.FILE_NOT_FOUND
ErrorCodes.IMPORT_ERROR
ErrorCodes.VALIDATION_ERROR
ErrorCodes.VALIDATION_FAILED
ErrorCodes.ID_MISMATCH
ErrorCodes.OPERATION_FAILED
ErrorCodes.INVALID_REQUEST
ErrorCodes.INTERNAL_SERVER_ERROR
```

## Testing Different Languages

```bash
# Default (English)
curl http://localhost:5000/api/Furniture/999

# French via Accept-Language header
curl -H "Accept-Language: fr" http://localhost:5000/api/Furniture/999

# French via query string
curl http://localhost:5000/api/Furniture/999?culture=fr
```

## Adding New Translations

1. **Add Error Code** (if needed) to `Models/ErrorCodes.cs`
2. **Add to English** `.resx`: `Resources/SharedResources.resx`
3. **Add to French** `.resx`: `Resources/SharedResources.fr.resx`
4. **Update Frontend** translation files to map the error code

## Resource File Keys Pattern

- `Email.Welcome.Subject`
- `Email.Welcome.Body`
- `Notification.FurnitureAdded`
- `Notification.ImportSuccess`
- `Server.FileValidation.Valid`
- `Server.FileValidation.Invalid`

## Language Detection Priority

1. Query string: `?culture=fr`
2. Cookie: `.AspNetCore.Culture`
3. Accept-Language header
4. Default: `en`
