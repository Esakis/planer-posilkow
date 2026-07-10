using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Services;

/// <summary>
/// Deterministyczny generator jadłospisu: zachłanny dobór przepisów pod budżet,
/// premiujący promocje, współdzielone (drogie) składniki i różnorodność białka.
/// Brak LLM w locie — powtarzalność i zerowy koszt (zgodnie z planem, sekcja 4).
/// </summary>
public class MenuGenerator
{
    private readonly PricingService _pricing;
    public MenuGenerator(PricingService pricing) => _pricing = pricing;

    // Składniki „znaczące" (drogie) — ich współdzielenie realnie obniża rachunek.
    private const decimal SignificantPricePerG = 0.01m;

    /// <summary>Zwraca przepisy w kolejności doboru; pomija te z wykluczonym tagiem oraz already-selected.</summary>
    public List<Recipe> Generate(
        IReadOnlyList<Recipe> pool, Store store, int people, int dinners,
        IEnumerable<string> exclusions, IEnumerable<int>? alreadyChosen = null)
    {
        var excluded = new HashSet<string>(exclusions.Select(Norm));
        var chosen = new List<Recipe>();
        var chosenIds = new HashSet<int>(alreadyChosen ?? Enumerable.Empty<int>());

        var candidates = pool
            .Where(r => !chosenIds.Contains(r.Id))
            .Where(r => !RecipeTags(r).Any(excluded.Contains))
            .ToList();

        // Składniki „w koszyku" (drogie), które już kupujemy — do premii za współdzielenie.
        var basketSignificant = new HashSet<int>();
        var proteinCounts = new Dictionary<string, int>();

        while (chosen.Count < dinners && candidates.Count > 0)
        {
            Recipe? best = null;
            double bestScore = double.NegativeInfinity;

            foreach (var r in candidates)
            {
                var score = Score(r, store, people, basketSignificant, proteinCounts);
                // remis rozstrzygamy po Id — pełna determinizm.
                if (score > bestScore || (score == bestScore && (best is null || r.Id < best.Id)))
                {
                    bestScore = score;
                    best = r;
                }
            }

            if (best is null) break;
            chosen.Add(best);
            candidates.Remove(best);

            foreach (var ri in best.Ingredients)
                if (_pricing.BasePricePerGram(ri.Ingredient, store) >= SignificantPricePerG)
                    basketSignificant.Add(ri.IngredientId);

            var pk = MainProtein(best);
            proteinCounts[pk] = proteinCounts.GetValueOrDefault(pk) + 1;
        }

        return chosen;
    }

    private double Score(
        Recipe r, Store store, int people,
        HashSet<int> basketSignificant, Dictionary<string, int> proteinCounts)
    {
        var cost = (double)_pricing.RecipeCost(r, store, people);

        bool hasPromo = r.Ingredients.Any(ri => _pricing.PricePerGram(ri.Ingredient, store).onPromo);

        int shared = r.Ingredients.Count(ri =>
            basketSignificant.Contains(ri.IngredientId) &&
            _pricing.BasePricePerGram(ri.Ingredient, store) >= SignificantPricePerG);

        int repeat = proteinCounts.GetValueOrDefault(MainProtein(r));

        double score = -cost;                 // taniej = lepiej
        if (hasPromo) score += 8;             // premia za wykorzystanie promocji
        score += shared * 2.0;                // współdzielenie drogich składników
        score -= repeat * 6.0;                // różnorodność białka w tygodniu
        if (RecipeTags(r).Contains("lubiane-przez-dzieci")) score += 1.5;
        return score;
    }

    private static string MainProtein(Recipe r)
    {
        var tags = RecipeTags(r);
        if (tags.Contains("ryby")) return "ryby";
        if (tags.Contains("drob")) return "drob";
        if (tags.Contains("wieprzowina")) return "wieprzowina";
        if (tags.Contains("wege")) return "wege";
        return "inne";
    }

    private static IEnumerable<string> RecipeTags(Recipe r) =>
        r.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
              .Select(Norm);

    private static string Norm(string s) => s.Trim().ToLowerInvariant();
}
