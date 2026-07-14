using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;
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

    /// <summary>Własny przepis użytkownika — ilości składników łączne, przeliczamy na porcję.</summary>
    [HttpPost]
    public async Task<ActionResult<CreateRecipeResponse>> Create([FromBody] CreateRecipeRequest req)
    {
        // duplikaty składników scalamy zamiast odrzucać
        var totals = req.Items
            .GroupBy(i => i.IngredientId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Grams));

        var ingredients = await _db.Ingredients
            .Where(i => totals.Keys.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        if (ingredients.Count != totals.Count)
            return BadRequest(new { message = "Lista zawiera nieznany składnik — odśwież stronę i spróbuj ponownie." });

        var name = req.Name.Trim();
        if (await _db.Recipes.AnyAsync(r => r.Name == name))
            return BadRequest(new { message = "Przepis o tej nazwie już istnieje." });

        // "własny" jako tag — znaczek widoczny wszędzie tam, gdzie pokazujemy chipy tagów
        var tags = (req.Tags ?? Array.Empty<string>())
            .Select(t => t.Trim().ToLowerInvariant().Replace(",", ""))
            .Where(t => t.Length > 0)
            .Append("własny")
            .Distinct()
            .ToArray();

        var steps = req.Steps
            .Select(s => s.Trim().Replace("|", " "))
            .Where(s => s.Length > 0)
            .ToArray();
        if (steps.Length == 0)
            return BadRequest(new { message = "Przepis musi mieć co najmniej 1 krok." });

        var recipe = new Recipe
        {
            Name = name,
            TimeMin = req.TimeMin,
            Tags = string.Join(",", tags),
            Steps = string.Join("|", steps),
            BaseServings = req.Servings,
            IsCustom = true
        };

        double p = 0, c = 0, f = 0, k = 0;
        foreach (var (ingId, grams) in totals)
        {
            var ing = ingredients[ingId];
            var perServing = grams / req.Servings;
            recipe.Ingredients.Add(new RecipeIngredient { Ingredient = ing, AmountPerServingG = perServing });
            p += perServing / 100.0 * ing.Protein100;
            c += perServing / 100.0 * ing.Carbs100;
            f += perServing / 100.0 * ing.Fat100;
            k += perServing / 100.0 * ing.Kcal100;
        }
        recipe.ProteinG = Math.Round(p, 1);
        recipe.CarbsG = Math.Round(c, 1);
        recipe.FatG = Math.Round(f, 1);
        recipe.Kcal = Math.Round(k);

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();

        return Ok(new CreateRecipeResponse(recipe.Id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id);
        if (recipe is null) return NotFound();
        if (!recipe.IsCustom)
            return BadRequest(new { message = "Można usuwać tylko własne przepisy." });

        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
