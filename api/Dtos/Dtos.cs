namespace TaniTydzien.Api.Dtos;

public record OnboardingRequest(
    int People,
    decimal WeeklyBudget,
    string Store,             // "Biedronka" | "Lidl"
    string[] Exclusions,     // tagi do wykluczenia: "ryby","wieprzowina","mięso"
    int Dinners);

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
    int[] RecipeIds,
    int SwapIndex,
    int People,
    decimal WeeklyBudget,
    string Store,
    string[] Exclusions);

public record ShoppingListRequest(
    int[] RecipeIds,
    int People,
    string Store);

public record CompareRequest(
    int[] RecipeIds,
    int People);

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
