using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Tests;

/// <summary>
/// Strażnik jakości danych startowych: rozmiar bazy przepisów, unikalność,
/// kategorie i zakaz "wariacji" różniących się 1–2 składnikami.
/// </summary>
public class SeedDataTests : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly AppDbContext _db;

    public SeedDataTests()
    {
        _conn = new SqliteConnection("DataSource=:memory:");
        _conn.Open();
        var opts = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conn).Options;
        _db = new AppDbContext(opts);
        _db.Database.EnsureCreated();
        SeedData.Seed(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _conn.Dispose();
    }

    [Fact]
    public void Baza_ma_co_najmniej_200_przepisow()
    {
        var count = _db.Recipes.Count();
        Assert.True(count >= 200, $"W seedach jest {count} przepisów, oczekiwane ≥200.");
    }

    [Fact]
    public void Nazwy_przepisow_sa_unikalne()
    {
        var duplicates = _db.Recipes.AsEnumerable()
            .GroupBy(r => r.Name.Trim().ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        Assert.True(duplicates.Count == 0, "Zduplikowane nazwy: " + string.Join(", ", duplicates));
    }

    [Fact]
    public void Kazdy_przepis_ma_dozwolona_kategorie()
    {
        var bad = _db.Recipes.AsEnumerable()
            .Where(r => !RecipeCategories.All.Contains(r.Category))
            .Select(r => $"{r.Name} → '{r.Category}'")
            .ToList();
        Assert.True(bad.Count == 0, "Nieznane kategorie: " + string.Join("; ", bad));
    }

    [Fact]
    public void Kategorie_sa_faktycznie_uzywane()
    {
        // co najmniej 8 z 10 merytorycznych kategorii ma przepisy — bez tego filtr jest atrapą
        var used = _db.Recipes.Select(r => r.Category).Distinct().Count();
        Assert.True(used >= 8, $"Używanych kategorii: {used}, oczekiwane ≥8.");
    }

    [Fact]
    public void Kazdy_przepis_ma_sensowna_zawartosc()
    {
        var bad = _db.Recipes.Include(r => r.Ingredients).AsEnumerable()
            .Where(r => r.Ingredients.Count < 3 || r.Kcal <= 0 || r.TimeMin <= 0 ||
                        string.IsNullOrWhiteSpace(r.Steps))
            .Select(r => r.Name)
            .ToList();
        Assert.True(bad.Count == 0, "Przepisy bez min. 3 składników/makro/kroków: " + string.Join(", ", bad));
    }

    [Fact]
    public void Kazdy_skladnik_ma_ceny_we_wszystkich_sieciach()
    {
        var storeCount = Enum.GetValues<Store>().Length;
        var bad = _db.Ingredients.Include(i => i.Products).AsEnumerable()
            .Where(i => i.Products.Select(p => p.Store).Distinct().Count() != storeCount)
            .Select(i => i.Name)
            .ToList();
        Assert.True(bad.Count == 0, "Składniki bez kompletu cen: " + string.Join(", ", bad));
    }

    [Fact]
    public void Zadne_dwa_przepisy_nie_roznia_sie_tylko_1_lub_2_skladnikami()
    {
        var recipes = _db.Recipes.Include(r => r.Ingredients).AsEnumerable()
            .Select(r => (r.Name, Ids: r.Ingredients.Select(ri => ri.IngredientId).ToHashSet()))
            .ToList();

        var offenders = new List<string>();
        for (var i = 0; i < recipes.Count; i++)
            for (var j = i + 1; j < recipes.Count; j++)
            {
                var a = recipes[i];
                var b = recipes[j];
                var symDiff = a.Ids.Count(id => !b.Ids.Contains(id)) + b.Ids.Count(id => !a.Ids.Contains(id));
                if (symDiff < 3)
                    offenders.Add($"„{a.Name}\" vs „{b.Name}\" (różnica: {symDiff} składników)");
            }

        Assert.True(offenders.Count == 0,
            $"Wariacje ({offenders.Count}):\n" + string.Join("\n", offenders.Take(30)));
    }
}
