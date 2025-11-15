using Microsoft.AspNetCore.Mvc;
using FurnitureInventory.Core.Entities;
using FurnitureInventory.Core.Interfaces;

namespace FurnitureInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FurnitureController : ControllerBase
{
    private readonly IFurnitureService _furnitureService;
    private readonly ILogger<FurnitureController> _logger;

    public FurnitureController(IFurnitureService furnitureService, ILogger<FurnitureController> logger)
    {
        _furnitureService = furnitureService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les meubles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Furniture>>> GetAll(CancellationToken cancellationToken)
    {
        var furnitures = await _furnitureService.GetAllAsync(cancellationToken);
        return Ok(furnitures);
    }

    /// <summary>
    /// Récupère un meuble par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Furniture>> GetById(int id, CancellationToken cancellationToken)
    {
        var furniture = await _furnitureService.GetByIdAsync(id, cancellationToken);
        if (furniture == null)
            return NotFound();

        return Ok(furniture);
    }

    /// <summary>
    /// Recherche des meubles par code barre
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<Furniture>> GetByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var furniture = await _furnitureService.GetByBarcodeAsync(barcode, cancellationToken);
        if (furniture == null)
            return NotFound();

        return Ok(furniture);
    }

    /// <summary>
    /// Recherche des meubles avec des critères
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Furniture>>> Search(
        [FromQuery] string? reference,
        [FromQuery] string? famille,
        [FromQuery] string? site,
        CancellationToken cancellationToken)
    {
        var furnitures = await _furnitureService.SearchAsync(reference, famille, site, cancellationToken);
        return Ok(furnitures);
    }

    /// <summary>
    /// Crée un nouveau meuble
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Furniture>> Create([FromBody] Furniture furniture, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _furnitureService.CreateAsync(furniture, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Met à jour un meuble
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Furniture>> Update(int id, [FromBody] Furniture furniture, CancellationToken cancellationToken)
    {
        if (id != furniture.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _furnitureService.UpdateAsync(furniture, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Supprime un meuble
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _furnitureService.DeleteAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Assigne une localisation à un meuble
    /// </summary>
    [HttpPost("{id}/location/{locationId}")]
    public async Task<ActionResult> AssignLocation(int id, int locationId, CancellationToken cancellationToken)
    {
        var result = await _furnitureService.AssignLocationAsync(id, locationId, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Assigne un tag RFID à un meuble
    /// </summary>
    [HttpPost("{id}/rfid/{rfidTagId}")]
    public async Task<ActionResult> AssignRfidTag(int id, int rfidTagId, CancellationToken cancellationToken)
    {
        var result = await _furnitureService.AssignRfidTagAsync(id, rfidTagId, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
