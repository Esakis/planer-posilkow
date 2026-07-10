using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Data;

/// <summary>
/// Wypełnia bazę realistycznymi danymi startowymi (przepisy, składniki, ceny,
/// promocje z „gazetki"). W produkcji przepisy powstają offline (LLM + redakcja),
/// tutaj mamy reprezentatywny wycinek ~18 obiadów pod polski budżet.
/// </summary>
public static class SeedData
{
    // Definicja składnika: makro na 100 g, dział sklepu, cena/gramatura opakowania w obu sieciach.
    private record IngDef(
        string Name, string Aisle,
        double P, double C, double F, double K,
        decimal PriceBiedronka, decimal PriceLidl, decimal PriceAuchan, double PackG);

    private record RecDef(
        string Name, int TimeMin, string Tags, string Steps,
        (string ing, double g)[] Items);

    // Promocja tygodnia: składnik + sieć + cena + warunek.
    private record PromoDef(string Ing, Store Store, decimal Price, string? Condition);

    public static void Seed(AppDbContext db)
    {
        if (db.Recipes.Any()) return;

        var week = new List<IngDef>
        {
            //           nazwa                         dział         P     C     F     K     Biedr.  Lidl    Auchan  pack(g)
            new("Pierś z kurczaka",                    "Mięso",     31,   0,    3.6,  165,  22.99m, 23.49m, 21.99m, 1000),
            new("Mięso mielone wieprzowo-wołowe",      "Mięso",     17,   0,    20,   250,  18.99m, 19.49m, 18.49m, 1000),
            new("Łopatka wieprzowa",                   "Mięso",     18,   0,    17,   230,  15.99m, 16.49m, 15.49m, 1000),
            new("Kiełbasa śląska",                     "Mięso",     12,   1,    30,   320,  12.99m, 12.49m, 13.29m, 500),
            new("Ryż biały",                           "Produkty sypkie", 7, 78, 0.7, 360, 5.99m,  5.79m,  5.49m, 1000),
            new("Makaron spaghetti",                   "Produkty sypkie", 12, 72, 1.5, 350, 3.99m,  3.79m,  3.69m, 500),
            new("Makaron świderki",                    "Produkty sypkie", 12, 72, 1.5, 350, 3.49m,  3.29m,  3.19m, 500),
            new("Kasza gryczana",                      "Produkty sypkie", 13, 68, 3,   340, 4.99m,  4.79m,  4.59m, 500),
            new("Mąka pszenna",                        "Produkty sypkie", 10, 76, 1,   360, 3.29m,  2.99m,  2.79m, 1000),
            new("Cukier",                              "Produkty sypkie", 0, 100, 0,   400, 3.99m,  3.79m,  3.59m, 1000),
            new("Ziemniaki",                           "Warzywa",   2,    17,   0.1,  77,   6.99m,  6.49m,  5.99m, 2500),
            new("Cebula",                              "Warzywa",   1,    9,    0.1,  40,   3.49m,  3.29m,  2.99m, 1000),
            new("Marchew",                             "Warzywa",   0.9,  10,   0.2,  41,   3.29m,  2.99m,  2.89m, 1000),
            new("Papryka czerwona",                    "Warzywa",   1,    6,    0.3,  31,   5.49m,  4.99m,  5.29m, 500),
            new("Pieczarki",                           "Warzywa",   3,    3,    0.3,  22,   4.49m,  3.99m,  4.29m, 400),
            new("Ogórek kiszony",                      "Warzywa",   0.6,  2,    0.1,  11,   8.99m,  8.49m,  8.29m, 900),
            new("Jabłka",                              "Owoce",     0.3,  14,   0.2,  52,   4.49m,  3.99m,  3.79m, 1000),
            new("Pomidory krojone (puszka)",           "Konserwy",  1.6,  4,    0.2,  25,   3.29m,  2.99m,  2.89m, 400),
            new("Koncentrat pomidorowy",               "Konserwy",  4,    19,   0.5,  95,   3.99m,  3.79m,  3.69m, 200),
            new("Fasola biała (puszka)",               "Konserwy",  7,    20,   0.5,  110,  3.99m,  3.79m,  3.69m, 400),
            new("Fasola czerwona (puszka)",            "Konserwy",  8,    20,   0.5,  115,  4.29m,  3.99m,  3.89m, 400),
            new("Tuńczyk (puszka)",                    "Konserwy",  25,   0,    1,    115,  5.99m,  5.49m,  5.79m, 170),
            new("Jajka",                               "Nabiał",    13,   1,    11,   155,  9.99m,  9.49m,  9.29m, 600),
            new("Twaróg półtłusty",                    "Nabiał",    18,   3,    5,    130,  3.79m,  3.49m,  3.59m, 250),
            new("Ser żółty gouda",                     "Nabiał",    25,   1,    27,   350,  12.99m, 11.99m, 11.49m, 400),
            new("Mleko",                               "Nabiał",    3.2,  4.8,  3.5,  64,   3.49m,  3.29m,  3.19m, 1000),
            new("Śmietana 18%",                        "Nabiał",    2.5,  4,    18,   185,  3.49m,  3.29m,  3.39m, 400),
            new("Olej rzepakowy",                      "Tłuszcze",  0,    0,    100,  900,  8.99m,  7.99m,  7.79m, 1000),
            new("Przyprawa curry",                     "Przyprawy", 12,   55,   14,   350,  3.49m,  2.99m,  3.29m, 30),
            new("Papryka słodka mielona",              "Przyprawy", 14,   54,   13,   340,  2.99m,  2.49m,  2.79m, 20),
            new("Bulion warzywny",                     "Przyprawy", 10,   20,   30,   250,  4.99m,  4.49m,  4.79m, 120),
        };

        var ingByName = new Dictionary<string, Ingredient>();
        foreach (var d in week)
        {
            var ing = new Ingredient
            {
                Name = d.Name, Aisle = d.Aisle, Unit = "g",
                Protein100 = d.P, Carbs100 = d.C, Fat100 = d.F, Kcal100 = d.K,
                Products = new List<Product>
                {
                    new() { Store = Store.Biedronka, Name = d.Name, BasePrice = d.PriceBiedronka, PackSizeG = d.PackG },
                    new() { Store = Store.Lidl,      Name = d.Name, BasePrice = d.PriceLidl,      PackSizeG = d.PackG },
                    new() { Store = Store.Auchan,    Name = d.Name, BasePrice = d.PriceAuchan,    PackSizeG = d.PackG },
                }
            };
            ingByName[d.Name] = ing;
            db.Ingredients.Add(ing);
        }

        // Promocje tygodnia (aktywne w dacie „dziś" seedowania — tydzień 29.06–05.07.2026).
        var from = new DateOnly(2026, 6, 29);
        var to = new DateOnly(2026, 7, 5);
        var promos = new List<PromoDef>
        {
            new("Pierś z kurczaka",               Store.Biedronka, 17.99m, "przy zakupie 2 opakowań"),
            new("Mięso mielone wieprzowo-wołowe", Store.Biedronka, 14.99m, null),
            new("Ser żółty gouda",                Store.Biedronka, 9.99m,  null),
            new("Papryka czerwona",               Store.Biedronka, 3.99m,  null),
            new("Pieczarki",                      Store.Biedronka, 2.99m,  null),
            new("Makaron spaghetti",              Store.Lidl,      2.49m,  null),
            new("Kasza gryczana",                 Store.Lidl,      3.49m,  null),
            new("Kiełbasa śląska",                Store.Lidl,      9.99m,  "Lidl Plus"),
            new("Ziemniaki",                      Store.Auchan,    3.99m,  "worek 2,5 kg"),
            new("Olej rzepakowy",                 Store.Auchan,    6.49m,  null),
            new("Pierś z kurczaka",               Store.Auchan,    18.99m, null),
            new("Mleko",                          Store.Auchan,    2.79m,  "przy zakupie 4 szt."),
        };
        foreach (var p in promos)
        {
            var product = ingByName[p.Ing].Products.First(x => x.Store == p.Store);
            product.Promotions.Add(new Promotion
            {
                PromoPrice = p.Price, Condition = p.Condition,
                From = from, To = to, Source = "Gazetka 26/2026"
            });
        }

        var recipes = new List<RecDef>
        {
            new("Kurczak w sosie curry z ryżem", 30, "mięso,drob,lubiane-przez-dzieci",
                "1. Ryż ugotuj na sypko.|2. Kurczaka pokrój w kostkę i podsmaż na oleju z cebulą.|3. Dodaj curry, wlej trochę wody z bulionem, duś 10 min.|4. Dodaj śmietanę, zagotuj.|5. Podawaj z ryżem.",
                new[]{("Pierś z kurczaka",150.0),("Ryż biały",80),("Cebula",40),("Śmietana 18%",30),("Przyprawa curry",3),("Olej rzepakowy",8),("Bulion warzywny",4)}),

            new("Spaghetti bolognese", 35, "mięso,wieprzowina,lubiane-przez-dzieci",
                "1. Makaron ugotuj al dente.|2. Cebulę zeszklij, dodaj mięso mielone, smaż do zrumienienia.|3. Wlej pomidory i koncentrat, dopraw, duś 15 min.|4. Wymieszaj z makaronem.",
                new[]{("Makaron spaghetti",90.0),("Mięso mielone wieprzowo-wołowe",100),("Pomidory krojone (puszka)",120),("Cebula",40),("Koncentrat pomidorowy",15),("Olej rzepakowy",8),("Bulion warzywny",4)}),

            new("Naleśniki z serem", 30, "wege,lubiane-przez-dzieci",
                "1. Wymieszaj mąkę, mleko i jajka na gładkie ciasto.|2. Smaż cienkie naleśniki.|3. Twaróg wymieszaj z cukrem.|4. Nakładaj farsz i zawijaj.",
                new[]{("Mąka pszenna",60.0),("Mleko",120),("Jajka",30),("Twaróg półtłusty",100),("Cukier",15),("Olej rzepakowy",8)}),

            new("Placki ziemniaczane", 40, "wege",
                "1. Ziemniaki i cebulę zetrzyj na tarce, odciśnij.|2. Dodaj jajko i mąkę, wymieszaj.|3. Smaż placki na rozgrzanym oleju z obu stron.",
                new[]{("Ziemniaki",300.0),("Mąka pszenna",25),("Jajka",25),("Cebula",20),("Olej rzepakowy",20)}),

            new("Zupa pomidorowa z ryżem", 30, "wege",
                "1. Podsmaż cebulę i marchew.|2. Wlej pomidory, koncentrat i wodę z bulionem, gotuj 15 min.|3. Zaciągnij śmietaną.|4. Podawaj z ugotowanym ryżem.",
                new[]{("Pomidory krojone (puszka)",100.0),("Koncentrat pomidorowy",20),("Marchew",40),("Ryż biały",25),("Śmietana 18%",20),("Cebula",20),("Bulion warzywny",6)}),

            new("Gulasz wieprzowy z kaszą", 60, "mięso,wieprzowina",
                "1. Łopatkę pokrój w kostkę, obsmaż.|2. Dodaj cebulę i paprykę, smaż chwilę.|3. Wlej wodę z bulionem i koncentrat, dopraw papryką, duś 40 min.|4. Podawaj z ugotowaną kaszą.",
                new[]{("Łopatka wieprzowa",130.0),("Kasza gryczana",70),("Cebula",50),("Papryka czerwona",60),("Koncentrat pomidorowy",15),("Olej rzepakowy",8),("Papryka słodka mielona",3),("Bulion warzywny",4)}),

            new("Kotlety mielone z ziemniakami", 45, "mięso,wieprzowina",
                "1. Mięso wymieszaj z jajkiem, mąką i startą cebulą, dopraw.|2. Formuj kotlety, smaż na oleju.|3. Ziemniaki ugotuj.|4. Podawaj z ogórkiem kiszonym.",
                new[]{("Mięso mielone wieprzowo-wołowe",110.0),("Jajka",15),("Mąka pszenna",15),("Ziemniaki",250),("Cebula",20),("Olej rzepakowy",15),("Ogórek kiszony",40)}),

            new("Ryż z warzywami (stir-fry)", 25, "wege",
                "1. Ryż ugotuj.|2. Marchew, paprykę i cebulę pokrój w paski, smaż na dużym ogniu z olejem.|3. Dodaj curry i ryż, wymieszaj, podsmaż razem.",
                new[]{("Ryż biały",80.0),("Marchew",60),("Papryka czerwona",60),("Cebula",40),("Olej rzepakowy",10),("Przyprawa curry",2),("Bulion warzywny",4)}),

            new("Leczo z kiełbasą", 35, "mięso,wieprzowina",
                "1. Kiełbasę pokrój i obsmaż.|2. Dodaj cebulę i paprykę, smaż 5 min.|3. Wlej pomidory i koncentrat, dopraw papryką, duś 15 min.",
                new[]{("Kiełbasa śląska",100.0),("Papryka czerwona",80),("Pomidory krojone (puszka)",100),("Cebula",50),("Koncentrat pomidorowy",10),("Olej rzepakowy",8),("Papryka słodka mielona",3)}),

            new("Makaron z tuńczykiem", 25, "ryby",
                "1. Makaron ugotuj.|2. Cebulę zeszklij, dodaj pomidory i koncentrat, duś 10 min.|3. Dodaj tuńczyka, wymieszaj z makaronem.",
                new[]{("Makaron świderki",90.0),("Tuńczyk (puszka)",60),("Pomidory krojone (puszka)",100),("Cebula",40),("Koncentrat pomidorowy",10),("Olej rzepakowy",8)}),

            new("Pierogi ruskie", 70, "wege,lubiane-przez-dzieci",
                "1. Z mąki, wody i szczypty soli zagnieć ciasto.|2. Ziemniaki ugotuj, wymieszaj z twarogiem i podsmażoną cebulą.|3. Formuj pierogi, gotuj w osolonej wodzie.",
                new[]{("Mąka pszenna",100.0),("Ziemniaki",120),("Twaróg półtłusty",60),("Cebula",30),("Olej rzepakowy",8)}),

            new("Fasolka po bretońsku", 35, "mięso,wieprzowina",
                "1. Kiełbasę i cebulę podsmaż.|2. Dodaj fasolę z zalewą, pomidory i koncentrat.|3. Dopraw papryką, duś 15 min.",
                new[]{("Fasola biała (puszka)",200.0),("Kiełbasa śląska",60),("Pomidory krojone (puszka)",80),("Cebula",40),("Koncentrat pomidorowy",15),("Olej rzepakowy",6),("Papryka słodka mielona",2)}),

            new("Zupa ogórkowa", 40, "wege",
                "1. Ziemniaki i marchew ugotuj w wodzie z bulionem.|2. Dodaj starte ogórki kiszone, gotuj 10 min.|3. Zaciągnij śmietaną.",
                new[]{("Ogórek kiszony",100.0),("Ziemniaki",150),("Marchew",50),("Śmietana 18%",25),("Cebula",20),("Bulion warzywny",6)}),

            new("Kasza gryczana z pieczarkami", 30, "wege",
                "1. Kaszę ugotuj.|2. Pieczarki i cebulę pokrój, smaż na oleju do zrumienienia.|3. Dodaj śmietanę, zagotuj, wymieszaj z kaszą.",
                new[]{("Kasza gryczana",80.0),("Pieczarki",120),("Cebula",50),("Olej rzepakowy",10),("Śmietana 18%",20),("Bulion warzywny",4)}),

            new("Chili con carne", 40, "mięso,wieprzowina",
                "1. Cebulę i mięso podsmaż.|2. Dodaj paprykę, fasolę, pomidory i koncentrat.|3. Dopraw papryką, duś 20 min.",
                new[]{("Mięso mielone wieprzowo-wołowe",100.0),("Fasola czerwona (puszka)",120),("Pomidory krojone (puszka)",120),("Papryka czerwona",50),("Cebula",40),("Koncentrat pomidorowy",10),("Olej rzepakowy",8)}),

            new("Racuchy z jabłkami", 25, "wege,lubiane-przez-dzieci",
                "1. Wymieszaj mąkę, mleko, jajko i cukier.|2. Dodaj starte jabłka.|3. Smaż małe placki na oleju.",
                new[]{("Mąka pszenna",70.0),("Mleko",100),("Jajka",25),("Jabłka",80),("Cukier",20),("Olej rzepakowy",10)}),

            new("Zapiekanka makaronowa z kurczakiem", 45, "mięso,drob",
                "1. Makaron ugotuj.|2. Kurczaka i cebulę podsmaż, dodaj pomidory i śmietanę.|3. Wymieszaj z makaronem, przełóż do naczynia, posyp serem.|4. Zapiekaj 20 min w 200°C.",
                new[]{("Makaron świderki",90.0),("Pierś z kurczaka",100),("Ser żółty gouda",40),("Śmietana 18%",40),("Pomidory krojone (puszka)",80),("Cebula",30),("Olej rzepakowy",6)}),

            new("Kurczak pieczony z ziemniakami", 55, "mięso,drob,lubiane-przez-dzieci",
                "1. Kurczaka natrzyj olejem i papryką.|2. Ziemniaki i marchew pokrój, wymieszaj z olejem.|3. Piecz wszystko razem 40 min w 200°C.",
                new[]{("Pierś z kurczaka",160.0),("Ziemniaki",300),("Marchew",60),("Olej rzepakowy",12),("Papryka słodka mielona",3),("Bulion warzywny",3)}),
        };

        foreach (var r in recipes)
        {
            var recipe = new Recipe
            {
                Name = r.Name, TimeMin = r.TimeMin, Tags = r.Tags, Steps = r.Steps, BaseServings = 4
            };
            double p = 0, c = 0, f = 0, k = 0;
            foreach (var (ingName, grams) in r.Items)
            {
                var ing = ingByName[ingName];
                recipe.Ingredients.Add(new RecipeIngredient { Ingredient = ing, AmountPerServingG = grams });
                p += grams / 100.0 * ing.Protein100;
                c += grams / 100.0 * ing.Carbs100;
                f += grams / 100.0 * ing.Fat100;
                k += grams / 100.0 * ing.Kcal100;
            }
            recipe.ProteinG = Math.Round(p, 1);
            recipe.CarbsG = Math.Round(c, 1);
            recipe.FatG = Math.Round(f, 1);
            recipe.Kcal = Math.Round(k);
            db.Recipes.Add(recipe);
        }

        db.SaveChanges();
    }
}
