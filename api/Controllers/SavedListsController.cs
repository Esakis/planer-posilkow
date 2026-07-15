using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Controllers;

/// <summary>Zapisane listy zakupów użytkownika — wiele nazwanych list z odhaczaniem pozycji.</summary>
[ApiController]
[Route("api/saved-lists")]
[Authorize]
public class SavedListsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SavedListsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<SavedListDto[]>> GetAll()
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var lists = await _db.SavedLists
            .Where(l => l.UserId == userId)
            .Include(l => l.Items).ThenInclude(i => i.Ingredient)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();

        return Ok(lists.Select(ToDto).ToArray());
    }

    [HttpPost]
    public async Task<ActionResult<SavedListDto>> Create([FromBody] CreateSavedListRequest req)
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var items = await BuildItemsAsync(req.Items ?? Array.Empty<SavedListItemInput>());
        if (items is null)
            return BadRequest(new { message = "Lista zawiera nieznany składnik — odśwież stronę i spróbuj ponownie." });

        var now = DateTime.UtcNow;
        var list = new SavedList
        {
            UserId = userId,
            Name = req.Name.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            Items = items
        };
        _db.SavedLists.Add(list);
        await _db.SaveChangesAsync();

        return Ok(ToDto(list));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SavedListDto>> Update(int id, [FromBody] UpdateSavedListRequest req)
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var list = await _db.SavedLists
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        if (list is null)
            return NotFound(new { message = "Ta lista nie istnieje lub została usunięta." });

        var items = await BuildItemsAsync(req.Items);
        if (items is null)
            return BadRequest(new { message = "Lista zawiera nieznany składnik — odśwież stronę i spróbuj ponownie." });

        _db.SavedListItems.RemoveRange(list.Items);
        list.Items = items;
        list.Name = req.Name.Trim();
        list.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ToDto(list));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var list = await _db.SavedLists.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        if (list is null)
            return NotFound(new { message = "Ta lista nie istnieje lub została usunięta." });

        _db.SavedLists.Remove(list);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Buduje pozycje listy z walidacją składników; null gdy któryś nie istnieje.</summary>
    private async Task<List<SavedListItem>?> BuildItemsAsync(SavedListItemInput[] inputs)
    {
        var ids = inputs.Select(i => i.IngredientId).Distinct().ToArray();
        var known = await _db.Ingredients
            .Where(i => ids.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);
        if (known.Count != ids.Length) return null;

        return inputs.Select(i => new SavedListItem
        {
            IngredientId = i.IngredientId,
            Ingredient = known[i.IngredientId],
            Grams = i.Grams,
            Checked = i.Checked
        }).ToList();
    }

    private static SavedListDto ToDto(SavedList l) => new(
        l.Id, l.Name, l.UpdatedAt,
        l.Items.Select(i => new SavedListItemDto(
            i.IngredientId, i.Ingredient.Name, i.Ingredient.Aisle, i.Grams, i.Checked)).ToArray());

    private int? CurrentUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
