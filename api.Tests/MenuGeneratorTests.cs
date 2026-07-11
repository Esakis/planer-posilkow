using TaniTydzien.Api.Dtos;
using TaniTydzien.Api.Models;
using TaniTydzien.Api.Services;

namespace TaniTydzien.Api.Tests;

public class MenuGeneratorTests
{
    private readonly MenuGenerator _generator = new(new PricingService());

    // ---------- pomocnicy do budowy danych ----------

    private static int _nextId = 1;

    /// <summary>Przepis z jednym dedykowanym składnikiem o zadanej cenie opakowania.</summary>
    private static Recipe MakeRecipe(
        string name, string tags, decimal packPrice, double proteinG = 20, double kcal = 500,
        bool promo = false, decimal promoPrice = 0m)
    {
        var id = _nextId++;
        var ingredient = new Ingredient { Id = id, Name = $"skladnik-{name}" };
        var product = new Product
        {
            Id = id,
            Store = Store.Biedronka,
            Ingredient = ingredient,
            IngredientId = id,
            BasePrice = packPrice,
            PackSizeG = 1000
        };
        if (promo)
        {
            // okno obejmujące PricingService.CurrentDate (2026-07-02)
            product.Promotions.Add(new Promotion
            {
                PromoPrice = promoPrice,
                From = PricingService.CurrentDate.AddDays(-3),
                To = PricingService.CurrentDate.AddDays(3)
            });
        }
        ingredient.Products.Add(product);

        var recipe = new Recipe
        {
            Id = id,
            Name = name,
            Tags = tags,
            ProteinG = proteinG,
            Kcal = kcal
        };
        recipe.Ingredients.Add(new RecipeIngredient
        {
            Recipe = recipe,
            RecipeId = id,
            Ingredient = ingredient,
            IngredientId = id,
            AmountPerServingG = 250 // 1000 g na 4 osoby = całe opakowanie
        });
        return recipe;
    }

    private List<Recipe> Generate(
        IReadOnlyList<Recipe> pool, int dinners,
        IEnumerable<string>? exclusions = null, IEnumerable<int>? alreadyChosen = null,
        MacroFilters? macro = null) =>
        _generator.Generate(pool, Store.Biedronka, people: 4, dinners, exclusions ?? Array.Empty<string>(),
            alreadyChosen, macro);

    // ---------- testy ----------

    [Fact]
    public void Zwraca_zadana_liczbe_obiadow()
    {
        var pool = Enumerable.Range(0, 10).Select(i => MakeRecipe($"r{i}", "wege", 10m)).ToList();
        var result = Generate(pool, dinners: 5);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Nie_zwraca_wiecej_niz_dostepne_przepisy()
    {
        var pool = new List<Recipe> { MakeRecipe("a", "wege", 10m), MakeRecipe("b", "wege", 10m) };
        var result = Generate(pool, dinners: 7);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Pomija_przepisy_z_wykluczonym_tagiem()
    {
        var pool = new List<Recipe>
        {
            MakeRecipe("ryba", "ryby", 10m),
            MakeRecipe("schab", "wieprzowina", 10m),
            MakeRecipe("salatka", "wege", 10m)
        };
        var result = Generate(pool, dinners: 3, exclusions: new[] { "ryby", "wieprzowina" });
        var names = result.Select(r => r.Name).ToList();
        Assert.Equal(new[] { "salatka" }, names);
    }

    [Fact]
    public void Pomija_przepisy_juz_wybrane()
    {
        var a = MakeRecipe("a", "wege", 10m);
        var b = MakeRecipe("b", "wege", 10m);
        var result = Generate(new[] { a, b }, dinners: 2, alreadyChosen: new[] { a.Id });
        Assert.Equal(new[] { b.Id }, result.Select(r => r.Id).ToArray());
    }

    [Fact]
    public void Jest_deterministyczny_dla_tych_samych_danych()
    {
        var pool = Enumerable.Range(0, 12)
            .Select(i => MakeRecipe($"r{i}", i % 2 == 0 ? "drob" : "wege", 5m + i))
            .ToList();
        var first = Generate(pool, dinners: 5).Select(r => r.Id).ToArray();
        var second = Generate(pool, dinners: 5).Select(r => r.Id).ToArray();
        Assert.Equal(first, second);
    }

    [Fact]
    public void Preferuje_tansze_przepisy()
    {
        var cheap = MakeRecipe("tani", "wege", 5m);
        var expensive = MakeRecipe("drogi", "wege", 40m);
        var result = Generate(new[] { expensive, cheap }, dinners: 1);
        Assert.Equal("tani", result.Single().Name);
    }

    [Fact]
    public void Premiuje_przepis_z_aktywna_promocja()
    {
        // bez promocji: -12 zł; z promocją: -13 zł + 8 pkt premii → promo wygrywa
        var noPromo = MakeRecipe("zwykly", "wege", 12m);
        var withPromo = MakeRecipe("promocyjny", "wege", 15m, promo: true, promoPrice: 13m);
        var result = Generate(new[] { noPromo, withPromo }, dinners: 1);
        Assert.Equal("promocyjny", result.Single().Name);
    }

    [Fact]
    public void Ignoruje_promocje_spoza_biezacego_tygodnia()
    {
        var recipe = MakeRecipe("przeterminowana", "wege", 15m, promo: true, promoPrice: 1m);
        var promo = recipe.Ingredients[0].Ingredient.Products[0].Promotions[0];
        promo.From = PricingService.CurrentDate.AddDays(-30);
        promo.To = PricingService.CurrentDate.AddDays(-20);

        var cheaper = MakeRecipe("tanszy-bez-promo", "wege", 12m);
        var result = Generate(new[] { recipe, cheaper }, dinners: 1);
        Assert.Equal("tanszy-bez-promo", result.Single().Name);
    }

    [Fact]
    public void Roznicuje_bialko_zamiast_powtarzac_ten_sam_typ()
    {
        // dwa dania drobiowe i ryba w podobnej cenie — drugi wybór powinien zmienić białko
        var pool = new List<Recipe>
        {
            MakeRecipe("kurczak-1", "drob", 10m),
            MakeRecipe("kurczak-2", "drob", 10m),
            MakeRecipe("ryba", "ryby", 11m)
        };
        var result = Generate(pool, dinners: 2);
        var proteins = result.Select(r => r.Tags).ToList();
        Assert.Contains("ryby", proteins);
    }

    [Fact]
    public void Filtr_min_bialka_odrzuca_przepisy_ponizej_progu()
    {
        var pool = new List<Recipe>
        {
            MakeRecipe("malo-bialka", "wege", 10m, proteinG: 10),
            MakeRecipe("duzo-bialka", "drob", 10m, proteinG: 40)
        };
        var result = Generate(pool, dinners: 2, macro: new MacroFilters(MinProtein: 30));
        Assert.Equal(new[] { "duzo-bialka" }, result.Select(r => r.Name).ToArray());
    }

    [Fact]
    public void Filtr_max_kcal_odrzuca_przepisy_powyzej_progu()
    {
        var pool = new List<Recipe>
        {
            MakeRecipe("lekki", "wege", 10m, kcal: 400),
            MakeRecipe("ciezki", "wege", 10m, kcal: 800)
        };
        var result = Generate(pool, dinners: 2, macro: new MacroFilters(MaxKcal: 600));
        Assert.Equal(new[] { "lekki" }, result.Select(r => r.Name).ToArray());
    }

    [Fact]
    public void Filtr_zakresowy_laczy_wiele_makro_naraz()
    {
        var pool = new List<Recipe>
        {
            MakeRecipe("pasuje", "wege", 10m, proteinG: 35, kcal: 500),
            MakeRecipe("za-malo-bialka", "wege", 10m, proteinG: 10, kcal: 500),
            MakeRecipe("za-ciezki", "wege", 10m, proteinG: 35, kcal: 900)
        };
        var result = Generate(pool, dinners: 3,
            macro: new MacroFilters(MinProtein: 30, MaxProtein: 60, MaxKcal: 600));
        Assert.Equal(new[] { "pasuje" }, result.Select(r => r.Name).ToArray());
    }

    [Fact]
    public void Zbyt_ostre_filtry_daja_pusta_liste()
    {
        var pool = new List<Recipe> { MakeRecipe("a", "wege", 10m, proteinG: 20) };
        var result = Generate(pool, dinners: 1, macro: new MacroFilters(MinProtein: 100));
        Assert.Empty(result);
    }

    [Fact]
    public void FilterPool_liczy_pasujace_przepisy_bez_generowania()
    {
        var pool = new List<Recipe>
        {
            MakeRecipe("ryba", "ryby", 10m, proteinG: 40),
            MakeRecipe("kurczak", "drob", 10m, proteinG: 40),
            MakeRecipe("salatka", "wege", 10m, proteinG: 8)
        };
        var count = _generator.FilterPool(pool, new[] { "ryby" }, new MacroFilters(MinProtein: 30)).Count;
        Assert.Equal(1, count); // kurczak: ryba wykluczona, sałatka poniżej progu białka
    }
}
