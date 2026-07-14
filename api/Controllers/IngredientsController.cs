using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;

namespace TaniTydzien.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    private readonly AppDbContext _db;
    public IngredientsController(AppDbContext db) => _db = db;

    /// <summary>Wszystkie składniki z bazy — do wyszukiwarki w formularzu przepisu i własnej liście zakupów.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> List()
    {
        var items = await _db.Ingredients
            .OrderBy(i => i.Name)
            .Select(i => new IngredientDto(i.Id, i.Name, i.Unit, i.Aisle, i.Protein100, i.Carbs100, i.Fat100, i.Kcal100))
            .ToListAsync();
        return Ok(items);
    }
}
