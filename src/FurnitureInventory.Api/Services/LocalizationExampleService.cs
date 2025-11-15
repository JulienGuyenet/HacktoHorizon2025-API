using Microsoft.Extensions.Localization;
using FurnitureInventory.Api.Resources;

namespace FurnitureInventory.Api.Services;

/// <summary>
/// Example service demonstrating how to use localization for server-generated content
/// This would be used for emails, notifications, PDF reports, etc.
/// </summary>
public class LocalizationExampleService
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LocalizationExampleService(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }

    /// <summary>
    /// Example: Send a welcome email with localized content
    /// </summary>
    public string GetWelcomeEmailSubject()
    {
        // The current culture is automatically used based on:
        // 1. Query string (?culture=fr)
        // 2. Cookie
        // 3. Accept-Language header
        return _localizer["Email.Welcome.Subject"].Value;
    }

    /// <summary>
    /// Example: Get localized notification message
    /// </summary>
    public string GetFurnitureAddedNotification()
    {
        return _localizer["Notification.FurnitureAdded"].Value;
    }

    /// <summary>
    /// Example: Get localized message with parameters
    /// </summary>
    public string GetImportSuccessMessage(int count)
    {
        // The {0} placeholder will be replaced with the count
        return _localizer["Notification.ImportSuccess", count].Value;
    }

    /// <summary>
    /// Example: Get localized message with multiple parameters
    /// </summary>
    public string GetImportUploadSuccessMessage(int count, string fileName)
    {
        // {0} = count, {1} = fileName
        return _localizer["Notification.ImportUploadSuccess", count, fileName].Value;
    }
}
