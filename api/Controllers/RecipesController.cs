using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Controllers;

[ApiController]
[Route("api/recipes")]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PricingService _pricing;

    public RecipesController(AppDbContext db, PricingService pricing)
    {
        _db = db;
        _pricing = pricing;
    }

    /// <summary>Katalog przepisów (wspólny dla wszystkich), z kosztem/makro i serduszkami użytkownika.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeCardDto>>> List(
        [FromQuery] string store = "Biedronka",
        [FromQuery] int people = 4)
    {
        var userId = CurrentUserId();
        var s = Mapping.ParseStore(store);
        var recipes = await _db.Recipes
            .Include(r => r.Ingredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Products).ThenInclude(p => p.Promotions)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var favorites = await FavoriteIdsAsync(userId);

        var cards = recipes.Select(r =>
        {
            var d = Mapping.ToDish(r, 0, s, people, _pricing);
            return new RecipeCardDto(
                r.Id, r.Name, r.TimeMin, d.Tags,
                d.Kcal, d.Protein, d.Carbs, d.Fat, d.Cost, d.HasPromo,
                r.IsCustom,
                Mine: r.CreatedByUserId == userId,
                Favorite: favorites.Contains(r.Id),
                Category: r.Category);
        });
        return Ok(cards);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeDetailDto>> Detail(int id, [FromQuery] int people = 4)
    {
        var userId = CurrentUserId();
        var recipe = await _db.Recipes
            .Include(r => r.Ingredients).ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null) return NotFound();

        var favorite = userId is { } uid &&
            await _db.FavoriteRecipes.AnyAsync(f => f.UserId == uid && f.RecipeId == id);

        return Ok(Mapping.ToDetail(recipe, people) with
        {
            Mine = recipe.CreatedByUserId == userId,
            Favorite = favorite
        });
    }

    /// <summary>Id ulubionych przepisów zalogowanego użytkownika.</summary>
    [HttpGet("favorites")]
    public async Task<ActionResult<int[]>> Favorites() =>
        Ok((await FavoriteIdsAsync(CurrentUserId())).ToArray());

    /// <summary>Dodaje serduszko (idempotentne).</summary>
    [HttpPost("{id:int}/favorite")]
    public async Task<IActionResult> AddFavorite(int id)
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });
        if (!await _db.Recipes.AnyAsync(r => r.Id == id))
            return NotFound(new { message = "Ten przepis nie istnieje." });

        if (!await _db.FavoriteRecipes.AnyAsync(f => f.UserId == userId && f.RecipeId == id))
        {
            _db.FavoriteRecipes.Add(new FavoriteRecipe { UserId = userId, RecipeId = id });
            await _db.SaveChangesAsync();
        }
        return NoContent();
    }

    /// <summary>Zdejmuje serduszko (idempotentne).</summary>
    [HttpDelete("{id:int}/favorite")]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var fav = await _db.FavoriteRecipes.FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == id);
        if (fav is not null)
        {
            _db.FavoriteRecipes.Remove(fav);
            await _db.SaveChangesAsync();
        }
        return NoContent();
    }

    /// <summary>Własny przepis użytkownika — ilości składników łączne, przeliczamy na porcję.
    /// Przepis od razu trafia do wspólnego katalogu i puli generatora.</summary>
    [HttpPost]
    public async Task<ActionResult<CreateRecipeResponse>> Create([FromBody] CreateRecipeRequest req)
    {
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

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
            IsCustom = true,
            CreatedByUserId = userId,
            // nieznana/pusta kategoria spada do "Inne" — nie odrzucamy przepisu z tego powodu
            Category = RecipeCategories.All.Contains(req.Category) ? req.Category! : "Inne"
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
        if (CurrentUserId() is not { } userId)
            return Unauthorized(new { message = "Sesja wygasła. Zaloguj się ponownie." });

        var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id);
        if (recipe is null) return NotFound();
        if (!recipe.IsCustom || recipe.CreatedByUserId is null)
            return BadRequest(new { message = "Przepisów z bazy TaniTydzień nie można usuwać." });
        if (recipe.CreatedByUserId != userId)
            return BadRequest(new { message = "Możesz usuwać tylko przepisy, które sam dodałeś." });

        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<HashSet<int>> FavoriteIdsAsync(int? userId)
    {
        if (userId is not { } uid) return new HashSet<int>();
        var ids = await _db.FavoriteRecipes
            .Where(f => f.UserId == uid)
            .Select(f => f.RecipeId)
            .ToListAsync();
        return ids.ToHashSet();
    }

    private int? CurrentUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
