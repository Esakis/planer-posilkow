using System.ComponentModel.DataAnnotations;

namespace TaniTydzien.Api.Dtos;

/// <summary>Zakresy makro na porcję (v1.1, sekcja 3a planu) — null = bez limitu.</summary>
public record MacroFilters(
    [Range(0, 200, ErrorMessage = "Min. białka musi być między 0 a 200 g.")] double? MinProtein = null,
    [Range(1, 200, ErrorMessage = "Maks. białka musi być między 1 a 200 g.")] double? MaxProtein = null,
    [Range(0, 300, ErrorMessage = "Min. węgli musi być między 0 a 300 g.")] double? MinCarbs = null,
    [Range(1, 300, ErrorMessage = "Maks. węgli musi być między 1 a 300 g.")] double? MaxCarbs = null,
    [Range(0, 150, ErrorMessage = "Min. tłuszczów musi być między 0 a 150 g.")] double? MinFat = null,
    [Range(1, 150, ErrorMessage = "Maks. tłuszczów musi być między 1 a 150 g.")] double? MaxFat = null,
    [Range(0, 3000, ErrorMessage = "Min. kcal musi być między 0 a 3000.")] double? MinKcal = null,
    [Range(1, 3000, ErrorMessage = "Maks. kcal musi być między 1 a 3000.")] double? MaxKcal = null)
{
    public bool Any =>
        MinProtein is not null || MaxProtein is not null ||
        MinCarbs is not null || MaxCarbs is not null ||
        MinFat is not null || MaxFat is not null ||
        MinKcal is not null || MaxKcal is not null;

    /// <summary>Czy przepis (makro per porcja) mieści się we wszystkich zakresach.</summary>
    public bool Matches(Models.Recipe r) =>
        (MinProtein is null || r.ProteinG >= MinProtein.Value) &&
        (MaxProtein is null || r.ProteinG <= MaxProtein.Value) &&
        (MinCarbs is null || r.CarbsG >= MinCarbs.Value) &&
        (MaxCarbs is null || r.CarbsG <= MaxCarbs.Value) &&
        (MinFat is null || r.FatG >= MinFat.Value) &&
        (MaxFat is null || r.FatG <= MaxFat.Value) &&
        (MinKcal is null || r.Kcal >= MinKcal.Value) &&
        (MaxKcal is null || r.Kcal <= MaxKcal.Value);
}

public record OnboardingRequest(
    [Range(1, 12, ErrorMessage = "Liczba osób musi być między 1 a 12.")] int People,
    [Range(0, 100_000, ErrorMessage = "Budżet musi być między 0 a 100 000 zł.")] decimal WeeklyBudget,
    string Store,             // "Biedronka" | "Lidl"
    string[] Exclusions,     // tagi do wykluczenia: "ryby","wieprzowina","mięso"
    [Range(1, 7, ErrorMessage = "Liczba obiadów musi być między 1 a 7.")] int Dinners,
    MacroFilters? Macro = null);

public record DishDto(
    int Day,
    int RecipeId,
    string Name,
    int TimeMin,
    string[] Tags,
    double Kcal,
    double Protein,
    double Carbs,
    double Fat,
    decimal Cost,
    bool HasPromo);

public record MacroSummary(double Kcal, double Protein, double Carbs, double Fat);

public record MenuResponse(
    string Week,
    string Store,
    int People,
    int Dinners,
    decimal Budget,
    decimal EstimatedCost,
    decimal BaselineCost,   // ten sam koszyk bez promocji
    decimal Savings,
    bool OverBudget,
    string? BudgetNote,
    MacroSummary PerDayAvg,
    DishDto[] Dishes);

public record IngredientLineDto(
    string Ingredient,
    string Product,
    double Grams,
    string DisplayQty,
    decimal Cost,
    bool OnPromo,
    string? PromoNote);

public record AisleGroupDto(string Aisle, IngredientLineDto[] Items, decimal Subtotal);

public record ShoppingListDto(
    string Store,
    AisleGroupDto[] Groups,
    decimal Total,
    decimal PromoSavings);

public record RecipeIngredientDto(string Name, string DisplayQty, string Aisle);

public record RecipeDetailDto(
    int Id,
    string Name,
    int TimeMin,
    string[] Tags,
    int Servings,
    MacroSummary MacroPerServing,
    string[] Steps,
    RecipeIngredientDto[] Ingredients);

public record SwapRequest(
    [Required, MinLength(1, ErrorMessage = "Jadłospis nie może być pusty.")] int[] RecipeIds,
    [Range(0, int.MaxValue, ErrorMessage = "Indeks dania nie może być ujemny.")] int SwapIndex,
    [Range(1, 12, ErrorMessage = "Liczba osób musi być między 1 a 12.")] int People,
    [Range(0, 100_000, ErrorMessage = "Budżet musi być między 0 a 100 000 zł.")] decimal WeeklyBudget,
    string Store,
    string[] Exclusions,
    MacroFilters? Macro = null);

/// <summary>Ilu przepisów pasuje do wykluczeń i filtrów makro — licznik na żywo pod suwakami.</summary>
public record PoolCountRequest(
    string[] Exclusions,
    MacroFilters? Macro = null);

public record PoolCountResponse(int Count);

public record ShoppingListRequest(
    [Required, MinLength(1, ErrorMessage = "Jadłospis nie może być pusty.")] int[] RecipeIds,
    [Range(1, 12, ErrorMessage = "Liczba osób musi być między 1 a 12.")] int People,
    string Store);

public record CompareRequest(
    [Required, MinLength(1, ErrorMessage = "Jadłospis nie może być pusty.")] int[] RecipeIds,
    [Range(1, 12, ErrorMessage = "Liczba osób musi być między 1 a 12.")] int People);

public record StoreCostDto(
    string Store,
    string Kind,             // "osiedlowy" | "hipermarket"
    string Note,             // np. "blisko, dojdziesz pieszo"
    decimal Total,           // koszt tego koszyka w tej sieci (z promocjami)
    decimal Baseline,        // ten sam koszyk bez promocji
    decimal PromoSavings,    // ile ścinają promocje
    int PromoItems,          // ile pozycji na promocji
    bool Cheapest,           // najtańszy z porównywanych
    decimal DiffToCheapest); // o ile drożej niż najtańszy (0 dla najtańszego)

public record CompareResponse(
    int People,
    StoreCostDto[] Stores,   // posortowane od najtańszego
    string CheapestStore,
    decimal MaxSaving);      // różnica najdroższy − najtańszy
