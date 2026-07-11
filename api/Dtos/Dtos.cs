using System.ComponentModel.DataAnnotations;

namespace TaniTydzien.Api.Dtos;

public record OnboardingRequest(
    [Range(1, 12, ErrorMessage = "Liczba osób musi być między 1 a 12.")] int People,
    [Range(0, 100_000, ErrorMessage = "Budżet musi być między 0 a 100 000 zł.")] decimal WeeklyBudget,
    string Store,             // "Biedronka" | "Lidl"
    string[] Exclusions,     // tagi do wykluczenia: "ryby","wieprzowina","mięso"
    [Range(1, 7, ErrorMessage = "Liczba obiadów musi być między 1 a 7.")] int Dinners,
    // filtry makro (v1.1) — null = bez limitu
    [Range(0, 200, ErrorMessage = "Min. białka musi być między 0 a 200 g.")] double? MinProteinPerServing = null,
    [Range(1, 3000, ErrorMessage = "Maks. kcal musi być między 1 a 3000.")] double? MaxKcalPerServing = null);

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
    [Range(0, 200, ErrorMessage = "Min. białka musi być między 0 a 200 g.")] double? MinProteinPerServing = null,
    [Range(1, 3000, ErrorMessage = "Maks. kcal musi być między 1 a 3000.")] double? MaxKcalPerServing = null);

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
