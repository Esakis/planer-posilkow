using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    /// <summary>Działy sklepu znane liście zakupów (ShoppingListService.AisleOrder).</summary>
    private static readonly string[] Aisles =
        { "Warzywa", "Owoce", "Mięso", "Nabiał", "Produkty sypkie", "Konserwy", "Tłuszcze", "Przyprawy", "Inne" };

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

    /// <summary>Nowy produkt użytkownika — cena w min. 1 sklepie; dla pozostałych działa fallback wyceny.</summary>
    [HttpPost]
    public async Task<ActionResult<IngredientDto>> Create([FromBody] CreateIngredientRequest req)
    {
        var name = req.Name.Trim();
        if (await _db.Ingredients.AnyAsync(i => i.Name.ToLower() == name.ToLower()))
            return BadRequest(new { message = "Produkt o tej nazwie już istnieje — wyszukaj go na liście." });

        if (!Aisles.Contains(req.Aisle))
            return BadRequest(new { message = "Nieznany dział sklepu." });

        var products = new List<Product>();
        foreach (var price in req.Prices)
        {
            if (!Enum.TryParse<Store>(price.Store, ignoreCase: true, out var store))
                return BadRequest(new { message = $"Nieznany sklep: {price.Store}." });
            if (products.Any(p => p.Store == store))
                return BadRequest(new { message = "Cena dla tego samego sklepu podana dwukrotnie." });

            products.Add(new Product
            {
                Store = store,
                Name = name,
                BasePrice = price.BasePrice,
                PackSizeG = price.PackSizeG
            });
        }

        var ingredient = new Ingredient
        {
            Name = name,
            Aisle = req.Aisle,
            Protein100 = req.Protein100 ?? 0,
            Carbs100 = req.Carbs100 ?? 0,
            Fat100 = req.Fat100 ?? 0,
            Kcal100 = req.Kcal100 ?? 0,
            Products = products
        };

        _db.Ingredients.Add(ingredient);
        await _db.SaveChangesAsync();

        return Ok(new IngredientDto(ingredient.Id, ingredient.Name, ingredient.Unit, ingredient.Aisle,
            ingredient.Protein100, ingredient.Carbs100, ingredient.Fat100, ingredient.Kcal100));
    }
}
