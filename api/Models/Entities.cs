namespace TaniTydzien.Api.Models;

/// <summary>Dozwolone kategorie dań — jedno źródło prawdy dla seedów, walidacji i UI.</summary>
public static class RecipeCategories
{
    public static readonly string[] All =
    {
        "Zupy", "Drób", "Wieprzowina", "Wołowina", "Ryby", "Wegetariańskie",
        "Makarony", "Pierogi i mączne", "Jednogarnkowe i zapiekanki", "Sałatki", "Inne"
    };
}

/// <summary>Sieć sklepów, pod którą optymalizujemy koszyk.</summary>
public enum Store
{
    Biedronka = 0,
    Lidl = 1,
    Auchan = 2
}

/// <summary>Znormalizowany składnik z wartościami odżywczymi na 100 g.</summary>
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    /// <summary>Jednostka bazowa, w której trzymamy ilości w przepisach (zawsze gramy).</summary>
    public string Unit { get; set; } = "g";
    /// <summary>Dział sklepu — do grupowania listy zakupów.</summary>
    public string Aisle { get; set; } = "Inne";

    public double Protein100 { get; set; }
    public double Carbs100 { get; set; }
    public double Fat100 { get; set; }
    public double Kcal100 { get; set; }

    public List<Product> Products { get; set; } = new();
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
}

/// <summary>Skąd pochodzi cena bazowa produktu.</summary>
public enum PriceSource
{
    /// <summary>Szacunek z danych startowych — w UI na szaro.</summary>
    Predicted = 0,
    /// <summary>Wpisana przez użytkownika — w UI na niebiesko, obowiązuje wszystkich.</summary>
    User = 1
}

/// <summary>Konkretny produkt sklepowy realizujący dany składnik.</summary>
public class Product
{
    public int Id { get; set; }
    public Store Store { get; set; }
    public string Name { get; set; } = "";
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    /// <summary>Cena bazowa opakowania (zł).</summary>
    public decimal BasePrice { get; set; }
    public PriceSource BasePriceSource { get; set; } = PriceSource.Predicted;
    /// <summary>Gramatura opakowania (g), do policzenia ceny za gram.</summary>
    public double PackSizeG { get; set; }

    public List<Promotion> Promotions { get; set; } = new();
}

/// <summary>Promocja z gazetki na konkretny produkt.</summary>
public class Promotion
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal PromoPrice { get; set; }
    public string? Condition { get; set; }
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public string? Source { get; set; }
}

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Steps { get; set; } = "";
    public int TimeMin { get; set; }
    /// <summary>Tagi diety/cech, przecinkami: "wege", "bez-ryb", "lubiane-przez-dzieci"...</summary>
    public string Tags { get; set; } = "";
    /// <summary>Kategoria dania, np. "Zupy", "Drób", "Makarony" — do filtrowania katalogu.</summary>
    public string Category { get; set; } = "Inne";
    public int BaseServings { get; set; } = 4;
    /// <summary>Przepis dodany przez użytkownika (nie z seedów) — tylko takie można usuwać.</summary>
    public bool IsCustom { get; set; }
    /// <summary>Autor przepisu; null = przepis z seedów. Usuwać może tylko autor.</summary>
    public int? CreatedByUserId { get; set; }

    // Makro na porcję — cache wyliczone ze składników.
    public double ProteinG { get; set; }
    public double CarbsG { get; set; }
    public double FatG { get; set; }
    public double Kcal { get; set; }

    public List<RecipeIngredient> Ingredients { get; set; } = new();
}

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    /// <summary>Ilość składnika na jedną porcję (w gramach).</summary>
    public double AmountPerServingG { get; set; }
}

/// <summary>Profil użytkownika / ustawienia generatora.</summary>
public class User
{
    public int Id { get; set; }
    public int People { get; set; } = 4;
    public decimal WeeklyBudget { get; set; } = 150m;
    public Store Store { get; set; } = Store.Biedronka;
    /// <summary>Wykluczone tagi, przecinkami (np. "ryby,wieprzowina").</summary>
    public string Exclusions { get; set; } = "";
    public int Dinners { get; set; } = 5;
    /// <summary>Stan konta: "free" (przed/po trialu) lub "premium" (opłacona subskrypcja).
    /// Efektywny plan (free/trial/premium/expired) wylicza <c>PlanService</c>.</summary>
    public string Plan { get; set; } = "free";
    /// <summary>Zakresy suwaków makro jako JSON (v1.1).</summary>
    public string? MacroFilters { get; set; }

    // Konto / logowanie
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool EmailVerified { get; set; }
    /// <summary>Jednorazowy token z maila aktywacyjnego; null po aktywacji.</summary>
    public string? ActivationToken { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>Koniec 30-dniowego okresu próbnego (UTC); start w momencie aktywacji maila.</summary>
    public DateTime? TrialEndsAt { get; set; }

    // Stripe (subskrypcja 9 zł/mies.)
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
}

public class Menu
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Week { get; set; } = "";
    /// <summary>Id przepisów, przecinkami, w kolejności dni.</summary>
    public string RecipeIds { get; set; } = "";
    public decimal EstimatedCost { get; set; }
}

/// <summary>Ulubiony przepis użytkownika (serduszko).</summary>
public class FavoriteRecipe
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}

/// <summary>Zapisana lista zakupów użytkownika (może mieć ich wiele, każda z nazwą).</summary>
public class SavedList
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<SavedListItem> Items { get; set; } = new();
}

public class SavedListItem
{
    public int Id { get; set; }
    public int SavedListId { get; set; }
    public SavedList SavedList { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public double Grams { get; set; }
    /// <summary>Odhaczone w sklepie ("już kupione").</summary>
    public bool Checked { get; set; }
}
