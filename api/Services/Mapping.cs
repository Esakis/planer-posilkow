using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Services;

public static class Mapping
{
    public static string[] TagList(string tags) =>
        tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public static DishDto ToDish(Recipe r, int day, Store store, int people, PricingService pricing)
    {
        bool hasPromo = r.Ingredients.Any(ri => pricing.PricePerGram(ri.Ingredient, store).onPromo);
        return new DishDto(
            day, r.Id, r.Name, r.TimeMin, TagList(r.Tags),
            r.Kcal, r.ProteinG, r.CarbsG, r.FatG,
            pricing.RecipeCost(r, store, people), hasPromo);
    }

    public static RecipeDetailDto ToDetail(Recipe r, int people)
    {
        var factor = people / (double)r.BaseServings;
        var ings = r.Ingredients
            .OrderBy(ri => ri.Ingredient.Aisle)
            .Select(ri => new RecipeIngredientDto(
                ri.Ingredient.Name,
                ShoppingListService.DisplayQty(ri.Ingredient, ri.AmountPerServingG * people),
                ri.Ingredient.Aisle))
            .ToArray();

        return new RecipeDetailDto(
            r.Id, r.Name, r.TimeMin, TagList(r.Tags), people,
            new MacroSummary(r.Kcal, r.ProteinG, r.CarbsG, r.FatG),
            r.Steps.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            ings);
    }

    public static Store ParseStore(string? s) => s?.Trim().ToLowerInvariant() switch
    {
        "lidl" => Store.Lidl,
        "auchan" => Store.Auchan,
        _ => Store.Biedronka
    };
}
