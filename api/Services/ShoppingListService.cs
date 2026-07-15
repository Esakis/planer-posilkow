using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Services;

/// <summary>Agreguje składniki z przepisów w listę zakupów pogrupowaną po działach sklepu.</summary>
public class ShoppingListService
{
    private readonly PricingService _pricing;
    public ShoppingListService(PricingService pricing) => _pricing = pricing;

    public ShoppingListDto Build(IEnumerable<Recipe> recipes, Store store, int people)
    {
        // sumujemy gramaturę per składnik
        var totals = new Dictionary<int, (Ingredient ing, double grams)>();
        foreach (var r in recipes)
            foreach (var ri in r.Ingredients)
            {
                var need = ri.AmountPerServingG * people;
                if (totals.TryGetValue(ri.IngredientId, out var cur))
                    totals[ri.IngredientId] = (cur.ing, cur.grams + need);
                else
                    totals[ri.IngredientId] = (ri.Ingredient, need);
            }

        return BuildFromTotals(totals, store);
    }

    /// <summary>Lista z własnych pozycji użytkownika — ilości absolutne, bez mnożenia przez liczbę osób.</summary>
    public ShoppingListDto Build(IEnumerable<(Ingredient ing, double grams)> items, Store store)
    {
        var totals = new Dictionary<int, (Ingredient ing, double grams)>();
        foreach (var (ing, grams) in items)
        {
            if (totals.TryGetValue(ing.Id, out var cur))
                totals[ing.Id] = (cur.ing, cur.grams + grams);
            else
                totals[ing.Id] = (ing, grams);
        }

        return BuildFromTotals(totals, store);
    }

    private ShoppingListDto BuildFromTotals(Dictionary<int, (Ingredient ing, double grams)> totals, Store store)
    {
        decimal total = 0m, promoSavings = 0m;
        var groups = new List<AisleGroupDto>();

        foreach (var aisle in totals.Values
                     .GroupBy(v => v.ing.Aisle)
                     .OrderBy(g => AisleOrder(g.Key)))
        {
            var lines = new List<IngredientLineDto>();
            decimal subtotal = 0m;

            foreach (var (ing, grams) in aisle.OrderBy(v => v.ing.Name))
            {
                var (perG, onPromo, promo) = _pricing.PricePerGram(ing, store);
                var cost = Math.Round(perG * (decimal)grams, 2);
                subtotal += cost;

                if (onPromo && promo is not null)
                {
                    var baseG = _pricing.BasePricePerGram(ing, store);
                    promoSavings += Math.Round((baseG - perG) * (decimal)grams, 2);
                }

                var product = ing.Products.FirstOrDefault(p => p.Store == store) ?? ing.Products.First();
                // promocja z gazetki = cena zweryfikowana; poza promocją decyduje źródło ceny bazowej
                var source = onPromo ? "verified"
                    : product.BasePriceSource == PriceSource.User ? "user"
                    : "predicted";
                lines.Add(new IngredientLineDto(
                    ing.Name,
                    product.Name,
                    Math.Round(grams),
                    DisplayQty(ing, grams),
                    cost,
                    onPromo,
                    onPromo && promo is not null ? PromoNote(promo) : null,
                    source,
                    ing.Id));
            }

            total += subtotal;
            groups.Add(new AisleGroupDto(aisle.Key, lines.ToArray(), Math.Round(subtotal, 2)));
        }

        return new ShoppingListDto(store.ToString(), groups.ToArray(), Math.Round(total, 2), Math.Round(promoSavings, 2));
    }

    public static string DisplayQty(Ingredient ing, double grams)
    {
        var g = Math.Round(grams);
        if (ing.Name.Contains("Jajka", StringComparison.OrdinalIgnoreCase))
            return $"~{Math.Max(1, Math.Round(grams / 60.0))} szt";
        if (ing.Name.Contains("puszka", StringComparison.OrdinalIgnoreCase))
        {
            var pack = ing.Products.FirstOrDefault()?.PackSizeG ?? 400;
            return $"~{Math.Max(1, Math.Ceiling(grams / pack))} puszki ({g} g)";
        }
        return g >= 1000 ? $"{g / 1000.0:0.##} kg" : $"{g} g";
    }

    private static string PromoNote(Promotion promo)
    {
        var to = promo.To.ToString("dd.MM");
        return promo.Condition is null
            ? $"promocja w gazetce do {to}"
            : $"{promo.Condition} · do {to}";
    }

    private static int AisleOrder(string aisle) => aisle switch
    {
        "Warzywa" => 0,
        "Owoce" => 1,
        "Mięso" => 2,
        "Nabiał" => 3,
        "Produkty sypkie" => 4,
        "Konserwy" => 5,
        "Tłuszcze" => 6,
        "Przyprawy" => 7,
        _ => 99
    };
}
