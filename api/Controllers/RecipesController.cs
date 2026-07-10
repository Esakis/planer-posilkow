using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PricingService _pricing;

    public RecipesController(AppDbContext db, PricingService pricing)
    {
        _db = db;
        _pricing = pricing;
    }

    /// <summary>Lista przepisów (przeglądarka), z kosztem i makro dla podanego kontekstu.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DishDto>>> List(
        [FromQuery] string store = "Biedronka",
        [FromQuery] int people = 4)
    {
        var s = Mapping.ParseStore(store);
        var recipes = await _db.Recipes
            .Include(r => r.Ingredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Products).ThenInclude(p => p.Promotions)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var dishes = recipes.Select(r => Mapping.ToDish(r, 0, s, people, _pricing));
        return Ok(dishes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeDetailDto>> Detail(int id, [FromQuery] int people = 4)
    {
        var recipe = await _db.Recipes
            .Include(r => r.Ingredients).ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null) return NotFound();
        return Ok(Mapping.ToDetail(recipe, people));
    }
}
