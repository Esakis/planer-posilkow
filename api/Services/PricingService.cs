using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Services;

/// <summary>
/// Liczy szacunkowe koszty składników i przepisów dla wybranej sieci,
/// z uwzględnieniem aktywnych promocji z gazetki.
/// </summary>
public class PricingService
{
    // „Bieżący tydzień" — stały punkt odniesienia, żeby demo zawsze pokazywało aktywne promocje.
    public static readonly DateOnly CurrentDate = new(2026, 7, 2);

    /// <summary>Efektywna cena za gram składnika w danej sieci (promo, jeśli aktywne).</summary>
    public (decimal pricePerG, bool onPromo, Promotion? promo) PricePerGram(Ingredient ing, Store store)
    {
        var product = ing.Products.FirstOrDefault(p => p.Store == store)
                      ?? ing.Products.First();

        var promo = product.Promotions.FirstOrDefault(pr => pr.From <= CurrentDate && CurrentDate <= pr.To);
        var effective = promo?.PromoPrice ?? product.BasePrice;
        var perG = product.PackSizeG > 0 ? effective / (decimal)product.PackSizeG : 0m;
        return (perG, promo is not null, promo);
    }

    public decimal BasePricePerGram(Ingredient ing, Store store)
    {
        var product = ing.Products.FirstOrDefault(p => p.Store == store) ?? ing.Products.First();
        return product.PackSizeG > 0 ? product.BasePrice / (decimal)product.PackSizeG : 0m;
    }

    /// <summary>Szacunkowy koszt całego dania dla podanej liczby osób (z promocjami).</summary>
    public decimal RecipeCost(Recipe r, Store store, int people)
    {
        decimal sum = 0m;
        foreach (var ri in r.Ingredients)
        {
            var (perG, _, _) = PricePerGram(ri.Ingredient, store);
            sum += perG * (decimal)(ri.AmountPerServingG * people);
        }
        return Math.Round(sum, 2);
    }

    /// <summary>Koszt dania bez promocji — do policzenia oszczędności.</summary>
    public decimal RecipeBaseCost(Recipe r, Store store, int people)
    {
        decimal sum = 0m;
        foreach (var ri in r.Ingredients)
            sum += BasePricePerGram(ri.Ingredient, store) * (decimal)(ri.AmountPerServingG * people);
        return Math.Round(sum, 2);
    }
}
