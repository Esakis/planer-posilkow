namespace TaniTydzien.Api.Models;

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
    public int BaseServings { get; set; } = 4;
    /// <summary>Przepis dodany przez użytkownika (nie z seedów) — tylko takie można usuwać.</summary>
    public bool IsCustom { get; set; }

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
    public string Plan { get; set; } = "free";
    /// <summary>Zakresy suwaków makro jako JSON (v1.1).</summary>
    public string? MacroFilters { get; set; }
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
