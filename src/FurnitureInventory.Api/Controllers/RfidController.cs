using Microsoft.AspNetCore.Mvc;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;

namespace FurnitureInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RfidController : ControllerBase
{
    private readonly IRfidService _rfidService;
    private readonly ILogger<RfidController> _logger;

    public RfidController(IRfidService rfidService, ILogger<RfidController> logger)
    {
        _rfidService = rfidService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les tags RFID actifs
    /// </summary>
    [HttpGet("tags")]
    public async Task<ActionResult<IEnumerable<RfidTag>>> GetActiveTags(CancellationToken cancellationToken)
    {
        var tags = await _rfidService.GetActiveTagsAsync(cancellationToken);
        return Ok(tags);
    }

    /// <summary>
    /// Récupère un tag RFID par son identifiant
    /// </summary>
    [HttpGet("tags/{tagId}")]
    public async Task<ActionResult<RfidTag>> GetTag(string tagId, CancellationToken cancellationToken)
    {
        var tag = await _rfidService.GetTagByIdAsync(tagId, cancellationToken);
        if (tag == null)
            return NotFound();

        return Ok(tag);
    }

    /// <summary>
    /// Enregistre un nouveau tag RFID
    /// </summary>
    [HttpPost("tags")]
    public async Task<ActionResult<RfidTag>> RegisterTag(
        [FromBody] RegisterTagRequest request,
        CancellationToken cancellationToken)
    {
        var tag = await _rfidService.RegisterTagAsync(request.TagId, request.TagType, cancellationToken);
        return CreatedAtAction(nameof(GetTag), new { tagId = tag.TagId }, tag);
    }

    /// <summary>
    /// Associe un tag RFID à un meuble
    /// </summary>
    [HttpPost("tags/{tagId}/assign/{furnitureId}")]
    public async Task<ActionResult> AssignTag(string tagId, int furnitureId, CancellationToken cancellationToken)
    {
        var result = await _rfidService.AssignTagToFurnitureAsync(tagId, furnitureId, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Traite une lecture de tag RFID
    /// </summary>
    [HttpPost("read")]
    public async Task<ActionResult> ProcessTagRead(
        [FromBody] TagReadRequest request,
        CancellationToken cancellationToken)
    {
        await _rfidService.ProcessTagReadAsync(request.TagId, request.ReaderId, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Désactive un tag RFID
    /// </summary>
    [HttpPost("tags/{tagId}/deactivate")]
    public async Task<ActionResult> DeactivateTag(string tagId, CancellationToken cancellationToken)
    {
        var result = await _rfidService.DeactivateTagAsync(tagId, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Récupère tous les lecteurs RFID actifs
    /// </summary>
    [HttpGet("readers")]
    public async Task<ActionResult<IEnumerable<RfidReader>>> GetActiveReaders(CancellationToken cancellationToken)
    {
        var readers = await _rfidService.GetActiveReadersAsync(cancellationToken);
        return Ok(readers);
    }

    /// <summary>
    /// Enregistre un nouveau lecteur RFID
    /// </summary>
    [HttpPost("readers")]
    public async Task<ActionResult<RfidReader>> RegisterReader(
        [FromBody] RegisterReaderRequest request,
        CancellationToken cancellationToken)
    {
        var reader = await _rfidService.RegisterReaderAsync(
            request.ReaderId,
            request.Name,
            request.LocationId,
            cancellationToken);

        return Ok(reader);
    }

    /// <summary>
    /// Met à jour le statut d'un lecteur RFID
    /// </summary>
    [HttpPost("readers/{readerId}/status")]
    public async Task<ActionResult> UpdateReaderStatus(
        string readerId,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _rfidService.UpdateReaderStatusAsync(readerId, request.Status, cancellationToken);
        return Ok();
    }
}

// Request DTOs
public record RegisterTagRequest(string TagId, string? TagType);
public record TagReadRequest(string TagId, string ReaderId);
public record RegisterReaderRequest(string ReaderId, string Name, int? LocationId);
public record UpdateStatusRequest(string Status);
