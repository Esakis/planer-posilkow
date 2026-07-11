using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly MenuGenerator _generator;
    private readonly PricingService _pricing;
    private readonly ShoppingListService _shopping;

    public MenuController(AppDbContext db, MenuGenerator generator, PricingService pricing, ShoppingListService shopping)
    {
        _db = db;
        _generator = generator;
        _pricing = pricing;
        _shopping = shopping;
    }

    private async Task<List<Recipe>> LoadPoolAsync() =>
        await _db.Recipes
            .Include(r => r.Ingredients).ThenInclude(ri => ri.Ingredient).ThenInclude(i => i.Products).ThenInclude(p => p.Promotions)
            .ToListAsync();

    [HttpPost("generate")]
    public async Task<ActionResult<MenuResponse>> Generate([FromBody] OnboardingRequest req)
    {
        // zakresy People/Dinners/budżetu egzekwuje walidacja [Range] na DTO (400 przed akcją)
        var store = Mapping.ParseStore(req.Store);

        var pool = await LoadPoolAsync();
        var chosen = _generator.Generate(pool, store, req.People, req.Dinners, req.Exclusions ?? Array.Empty<string>(),
            minProteinPerServing: req.MinProteinPerServing, maxKcalPerServing: req.MaxKcalPerServing);

        if (chosen.Count == 0)
            return BadRequest(new { message = "Brak przepisów pasujących do wykluczeń i filtrów makro. Poluzuj filtry." });

        return Ok(BuildResponse(chosen, store, req.People, req.WeeklyBudget));
    }

    [HttpPost("swap")]
    public async Task<ActionResult<MenuResponse>> Swap([FromBody] SwapRequest req)
    {
        var store = Mapping.ParseStore(req.Store);

        var pool = await LoadPoolAsync();
        var byId = pool.ToDictionary(r => r.Id);

        // nie filtrujemy po cichu — brakujące id przesunęłyby SwapIndex na inne danie
        var current = new List<Recipe>(req.RecipeIds.Length);
        foreach (var id in req.RecipeIds)
        {
            if (!byId.TryGetValue(id, out var recipe))
                return BadRequest(new { message = "Jadłospis zawiera nieaktualne przepisy — ułóż plan od nowa." });
            current.Add(recipe);
        }

        if (req.SwapIndex >= current.Count)
            return BadRequest(new { message = "Nieprawidłowy indeks dania do wymiany." });

        // wybierz 1 nowy przepis spoza obecnego zestawu
        var replacement = _generator
            .Generate(pool, store, req.People, 1, req.Exclusions ?? Array.Empty<string>(), req.RecipeIds,
                minProteinPerServing: req.MinProteinPerServing, maxKcalPerServing: req.MaxKcalPerServing)
            .FirstOrDefault();

        if (replacement is null)
            return BadRequest(new { message = "Brak innego przepisu do podmiany." });

        current[req.SwapIndex] = replacement;
        return Ok(BuildResponse(current, store, req.People, req.WeeklyBudget));
    }

    [HttpPost("shopping-list")]
    public async Task<ActionResult<ShoppingListDto>> ShoppingList([FromBody] ShoppingListRequest req)
    {
        var store = Mapping.ParseStore(req.Store);

        var pool = await LoadPoolAsync();
        var byId = pool.ToDictionary(r => r.Id);
        var recipes = req.RecipeIds.Where(byId.ContainsKey).Select(id => byId[id]).ToList();
        if (recipes.Count == 0) return BadRequest(new { message = "Pusty jadłospis." });

        return Ok(_shopping.Build(recipes, store, req.People));
    }

    /// <summary>Porównuje koszt tego samego jadłospisu w każdej sieci — „gdzie kupić najtaniej".</summary>
    [HttpPost("compare")]
    public async Task<ActionResult<CompareResponse>> Compare([FromBody] CompareRequest req)
    {
        var pool = await LoadPoolAsync();
        var byId = pool.ToDictionary(r => r.Id);
        var recipes = req.RecipeIds.Where(byId.ContainsKey).Select(id => byId[id]).ToList();
        if (recipes.Count == 0) return BadRequest(new { message = "Pusty jadłospis." });

        var stores = Enum.GetValues<Store>().Select(store =>
        {
            var list = _shopping.Build(recipes, store, req.People);
            var promoItems = list.Groups.Sum(g => g.Items.Count(i => i.OnPromo));
            var (kind, note) = StoreInfo(store);
            return new StoreCostDto(
                store.ToString(), kind, note,
                list.Total, Math.Round(list.Total + list.PromoSavings, 2),
                list.PromoSavings, promoItems,
                Cheapest: false, DiffToCheapest: 0m);
        }).ToList();

        var cheapest = stores.Min(s => s.Total);
        var dearest = stores.Max(s => s.Total);

        var ranked = stores
            .Select(s => s with { Cheapest = s.Total == cheapest, DiffToCheapest = Math.Round(s.Total - cheapest, 2) })
            .OrderBy(s => s.Total)
            .ToArray();

        return Ok(new CompareResponse(
            req.People, ranked,
            ranked.First().Store,
            Math.Round(dearest - cheapest, 2)));
    }

    private static (string kind, string note) StoreInfo(Store store) => store switch
    {
        Store.Auchan => ("hipermarket", "Duży wybór i tanie zakupy hurtem — ale trzeba dojechać."),
        Store.Lidl => ("osiedlowy", "Blisko, dojdziesz pieszo. Mocne promocje Lidl Plus."),
        _ => ("osiedlowy", "Blisko, dojdziesz pieszo. Gęsta sieć sklepów.")
    };

    private MenuResponse BuildResponse(List<Recipe> chosen, Store store, int people, decimal budget)
    {
        var dishes = chosen
            .Select((r, i) => Mapping.ToDish(r, i + 1, store, people, _pricing))
            .ToArray();

        var estimated = dishes.Sum(d => d.Cost);
        var baseline = chosen.Sum(r => _pricing.RecipeBaseCost(r, store, people));
        var savings = Math.Round(baseline - estimated, 2);
        var overBudget = budget > 0 && estimated > budget;

        var n = Math.Max(1, dishes.Length);
        var perDay = new MacroSummary(
            Math.Round(dishes.Sum(d => d.Kcal) / n),
            Math.Round(dishes.Sum(d => d.Protein) / n, 1),
            Math.Round(dishes.Sum(d => d.Carbs) / n, 1),
            Math.Round(dishes.Sum(d => d.Fat) / n, 1));

        string? note = overBudget
            ? $"Ten tydzień wychodzi ~{estimated:0.00} zł (budżet: {budget:0.00} zł). " +
              "Zwiększ budżet albo wymień najdroższe danie."
            : null;

        return new MenuResponse(
            Week: "29.06 – 05.07.2026",
            Store: store.ToString(),
            People: people,
            Dinners: dishes.Length,
            Budget: budget,
            EstimatedCost: Math.Round(estimated, 2),
            BaselineCost: Math.Round(baseline, 2),
            Savings: savings,
            OverBudget: overBudget,
            BudgetNote: note,
            PerDayAvg: perDay,
            Dishes: dishes);
    }
}
