using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Data;

/// <summary>
/// Wypełnia bazę realistycznymi danymi startowymi (przepisy, składniki, ceny,
/// promocje z „gazetki"). W produkcji przepisy powstają offline (LLM + redakcja),
/// tutaj mamy reprezentatywny wycinek ~50 obiadów pod polski budżet.
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
            new("Majeranek",                           "Przyprawy", 13,   60,   7,    330,  2.49m,  1.99m,  2.19m, 10),
            new("Udka z kurczaka",                     "Mięso",     17,   0,    12,   200,  9.99m,  9.49m,  8.99m, 1000),
            new("Filet z mintaja (mrożony)",           "Mięso",     17,   0,    1,    80,   24.99m, 23.99m, 22.99m, 1000),
            new("Schab wieprzowy",                     "Mięso",     21,   0,    10,   180,  17.99m, 18.49m, 16.99m, 1000),
            new("Wątróbka drobiowa",                   "Mięso",     19,   1,    5,    125,  8.99m,  8.49m,  7.99m, 500),
            new("Kapusta biała",                       "Warzywa",   1.3,  6,    0.1,  25,   3.99m,  3.49m,  2.99m, 2000),
            new("Kapusta kiszona",                     "Warzywa",   1.1,  4,    0.2,  19,   6.99m,  6.49m,  5.99m, 900),
            new("Kalafior",                            "Warzywa",   2,    5,    0.3,  25,   6.99m,  5.99m,  5.49m, 1000),
            new("Cukinia",                             "Warzywa",   1.2,  3,    0.3,  17,   6.99m,  5.99m,  5.99m, 1000),
            new("Pomidory świeże",                     "Warzywa",   0.9,  3.9,  0.2,  18,   7.99m,  6.99m,  6.49m, 1000),
            new("Por",                                 "Warzywa",   1.5,  14,   0.3,  61,   3.49m,  2.99m,  2.79m, 300),
            new("Seler korzeniowy",                    "Warzywa",   1.5,  9,    0.3,  42,   4.99m,  4.49m,  3.99m, 500),
            new("Brokuł",                              "Warzywa",   2.8,  7,    0.4,  34,   5.99m,  4.99m,  4.49m, 500),
            new("Czosnek",                             "Warzywa",   6,    33,   0.5,  149,  2.99m,  2.49m,  2.29m, 100),
            new("Natka pietruszki",                    "Warzywa",   3,    6,    0.8,  36,   2.49m,  1.99m,  1.99m, 50),
            new("Koperek",                             "Warzywa",   3,    7,    0.8,  40,   2.49m,  1.99m,  1.99m, 50),
            new("Szpinak mrożony",                     "Warzywa",   2.9,  1.4,  0.4,  23,   4.99m,  4.49m,  4.29m, 450),
            new("Banany",                              "Owoce",     1.1,  23,   0.3,  89,   5.99m,  5.49m,  4.99m, 1000),
            new("Soczewica czerwona",                  "Produkty sypkie", 24, 60, 1.5, 350, 8.99m,  8.49m,  7.99m, 500),
            new("Makaron penne",                       "Produkty sypkie", 12, 72, 1.5, 350, 3.99m,  3.69m,  3.49m, 500),
            new("Kasza jęczmienna",                    "Produkty sypkie", 8,  74, 2,   350, 3.99m,  3.79m,  3.49m, 500),
            new("Płatki owsiane",                      "Produkty sypkie", 13, 60, 7,   370, 3.49m,  2.99m,  2.89m, 500),
            new("Ciecierzyca (puszka)",                "Konserwy",  7,    14,   1.3,  105,  4.49m,  3.99m,  3.89m, 400),
            new("Kukurydza (puszka)",                  "Konserwy",  3,    19,   1.2,  97,   3.99m,  3.49m,  3.39m, 400),
            new("Groszek (puszka)",                    "Konserwy",  4.5,  10,   0.5,  65,   3.79m,  3.49m,  3.29m, 400),
            new("Jogurt naturalny",                    "Nabiał",    4,    6,    2,    60,   2.99m,  2.79m,  2.69m, 400),
            new("Ser mozzarella",                      "Nabiał",    18,   1,    17,   240,  4.99m,  4.49m,  4.39m, 125),
            new("Masło",                               "Tłuszcze",  0.7,  0.7,  82,   740,  7.99m,  7.49m,  6.99m, 200),
            new("Sos sojowy",                          "Przyprawy", 8,    8,    0,    60,   5.99m,  5.49m,  4.99m, 150),
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
            new("Udka z kurczaka",                Store.Biedronka, 6.99m,  null),
            new("Brokuł",                         Store.Biedronka, 3.49m,  null),
            new("Jogurt naturalny",               Store.Biedronka, 1.99m,  "przy zakupie 3 szt."),
            new("Filet z mintaja (mrożony)",      Store.Lidl,      19.99m, null),
            new("Masło",                          Store.Lidl,      5.99m,  "Lidl Plus"),
            new("Soczewica czerwona",             Store.Lidl,      6.99m,  null),
            new("Cukinia",                        Store.Auchan,    3.99m,  null),
            new("Kapusta kiszona",                Store.Auchan,    4.99m,  null),
            new("Schab wieprzowy",                Store.Auchan,    13.99m, null),
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

            new("Udka pieczone z ziemniakami i surówką", 65, "mięso,drob,lubiane-przez-dzieci",
                "1. Udka natrzyj olejem, papryką i majerankiem.|2. Piecz 50 min w 190°C razem z ziemniakami.|3. Kapustę i marchew zetrzyj, dopraw — podawaj jako surówkę.",
                new[]{("Udka z kurczaka",250.0),("Ziemniaki",250),("Kapusta biała",80),("Marchew",30),("Olej rzepakowy",10),("Papryka słodka mielona",3),("Majeranek",1)}),

            new("Kurczak w sosie pomidorowym z kaszą", 40, "mięso,drob",
                "1. Kurczaka pokrój i obsmaż z cebulą.|2. Dodaj pomidory, koncentrat i czosnek, duś 15 min.|3. Podawaj z ugotowaną kaszą jęczmienną.",
                new[]{("Pierś z kurczaka",130.0),("Kasza jęczmienna",70),("Pomidory krojone (puszka)",120),("Cebula",40),("Czosnek",5),("Koncentrat pomidorowy",10),("Olej rzepakowy",8)}),

            new("Kurczak stir-fry z sosem sojowym", 25, "mięso,drob",
                "1. Ryż ugotuj.|2. Kurczaka pokrój w paski, smaż na dużym ogniu.|3. Dodaj paprykę, marchew i por, smaż 5 min.|4. Wlej sos sojowy, wymieszaj z ryżem.",
                new[]{("Pierś z kurczaka",130.0),("Ryż biały",75),("Papryka czerwona",50),("Marchew",40),("Por",30),("Sos sojowy",12),("Olej rzepakowy",10)}),

            new("Ryż z kurczakiem i groszkiem", 30, "mięso,drob,lubiane-przez-dzieci",
                "1. Ryż ugotuj.|2. Kurczaka pokrój w kostkę, podsmaż z cebulą.|3. Dodaj groszek i kukurydzę, chwilę podgrzej.|4. Wymieszaj z ryżem, dopraw.",
                new[]{("Pierś z kurczaka",120.0),("Ryż biały",75),("Groszek (puszka)",60),("Kukurydza (puszka)",50),("Cebula",30),("Olej rzepakowy",8),("Bulion warzywny",3)}),

            new("Penne z kurczakiem i brokułem", 30, "mięso,drob",
                "1. Makaron ugotuj, brokuła sparz we wrzątku.|2. Kurczaka podsmaż z czosnkiem.|3. Dodaj śmietanę i brokuła, zagotuj.|4. Wymieszaj z makaronem, posyp serem.",
                new[]{("Makaron penne",85.0),("Pierś z kurczaka",110),("Brokuł",90),("Śmietana 18%",35),("Czosnek",4),("Ser żółty gouda",20),("Olej rzepakowy",6)}),

            new("Pulpety drobiowe w sosie koperkowym", 45, "mięso,drob,lubiane-przez-dzieci",
                "1. Kurczaka zmiel lub drobno posiekaj, wymieszaj z jajkiem i mąką, uformuj pulpety.|2. Gotuj w wodzie z bulionem 15 min.|3. Sos: śmietana z mąką i koperkiem, zagotuj w wywarze.|4. Podawaj z ziemniakami.",
                new[]{("Pierś z kurczaka",130.0),("Ziemniaki",250),("Jajka",15),("Mąka pszenna",20),("Śmietana 18%",30),("Koperek",6),("Bulion warzywny",5)}),

            new("Wątróbka smażona z cebulą", 25, "mięso,drob",
                "1. Wątróbkę oprósz mąką.|2. Smaż z cebulą na maśle ok. 6-8 min.|3. Dopraw majerankiem.|4. Podawaj z ziemniakami i ogórkiem kiszonym.",
                new[]{("Wątróbka drobiowa",140.0),("Ziemniaki",250),("Cebula",60),("Mąka pszenna",10),("Masło",12),("Majeranek",1),("Ogórek kiszony",40)}),

            new("Schab smażony z kapustą zasmażaną", 50, "mięso,wieprzowina",
                "1. Schab pokrój w plastry, rozbij, oprósz mąką i usmaż.|2. Kapustę poszatkuj, uduś z cebulą i zasmaż z mąką.|3. Podawaj z ziemniakami.",
                new[]{("Schab wieprzowy",130.0),("Kapusta biała",150),("Ziemniaki",250),("Cebula",30),("Mąka pszenna",15),("Olej rzepakowy",15)}),

            new("Bigos z kiełbasą", 80, "mięso,wieprzowina",
                "1. Kapustę kiszoną przepłucz, gotuj z liściem i majerankiem 40 min.|2. Kiełbasę i cebulę podsmaż, dodaj do kapusty.|3. Dodaj koncentrat, duś jeszcze 20 min.|4. Podawaj z ziemniakami lub chlebem.",
                new[]{("Kapusta kiszona",200.0),("Kiełbasa śląska",90),("Cebula",40),("Koncentrat pomidorowy",10),("Olej rzepakowy",6),("Majeranek",1),("Ziemniaki",150)}),

            new("Łazanki z kapustą i kiełbasą", 40, "mięso,wieprzowina",
                "1. Makaron ugotuj.|2. Kapustę poszatkuj i uduś z cebulą.|3. Kiełbasę podsmaż, połącz wszystko i chwilę razem smaż.",
                new[]{("Makaron świderki",80.0),("Kapusta biała",150),("Kiełbasa śląska",80),("Cebula",40),("Olej rzepakowy",10)}),

            new("Gołąbki bez zawijania", 50, "mięso,wieprzowina,lubiane-przez-dzieci",
                "1. Mięso podsmaż z cebulą.|2. Dodaj poszatkowaną kapustę i podduś.|3. Wsyp ryż, wlej pomidory i wodę, duś pod przykryciem 25 min.",
                new[]{("Mięso mielone wieprzowo-wołowe",110.0),("Kapusta biała",150),("Ryż biały",50),("Pomidory krojone (puszka)",120),("Cebula",40),("Koncentrat pomidorowy",10),("Olej rzepakowy",8)}),

            new("Zapiekanka ziemniaczana z mięsem", 60, "mięso,wieprzowina,lubiane-przez-dzieci",
                "1. Ziemniaki ugotuj i pokrój w plastry.|2. Mięso podsmaż z cebulą i czosnkiem.|3. Układaj warstwami, zalej śmietaną, posyp serem.|4. Zapiekaj 25 min w 200°C.",
                new[]{("Ziemniaki",280.0),("Mięso mielone wieprzowo-wołowe",100),("Cebula",30),("Czosnek",4),("Śmietana 18%",40),("Ser żółty gouda",30)}),

            new("Filet z mintaja z ziemniakami", 35, "ryby",
                "1. Rybę oprósz mąką, dopraw i usmaż na maśle.|2. Ziemniaki ugotuj, posyp koperkiem.|3. Podawaj z surówką z kapusty i marchwi.",
                new[]{("Filet z mintaja (mrożony)",150.0),("Ziemniaki",250),("Mąka pszenna",12),("Masło",12),("Koperek",5),("Kapusta biała",70),("Marchew",30)}),

            new("Ryba w sosie pomidorowym z ryżem", 35, "ryby",
                "1. Rybę pokrój w kawałki i podsmaż.|2. Cebulę zeszklij, dodaj pomidory i koncentrat, duś 10 min.|3. Włóż rybę do sosu, duś 5 min.|4. Podawaj z ryżem.",
                new[]{("Filet z mintaja (mrożony)",140.0),("Ryż biały",70),("Pomidory krojone (puszka)",120),("Cebula",40),("Koncentrat pomidorowy",10),("Olej rzepakowy",8)}),

            new("Sałatka z tuńczykiem, ryżem i kukurydzą", 25, "ryby",
                "1. Ryż ugotuj i ostudź.|2. Wymieszaj z tuńczykiem, kukurydzą, groszkiem i posiekanym ogórkiem kiszonym.|3. Dopraw jogurtem i pieprzem.",
                new[]{("Ryż biały",70.0),("Tuńczyk (puszka)",55),("Kukurydza (puszka)",60),("Groszek (puszka)",40),("Ogórek kiszony",40),("Jogurt naturalny",40)}),

            new("Zupa jarzynowa", 40, "wege",
                "1. Por, marchew, seler i ziemniaki pokrój.|2. Gotuj w wodzie z bulionem 20 min.|3. Dodaj groszek i kalafior, gotuj 10 min.|4. Posyp natką.",
                new[]{("Ziemniaki",150.0),("Marchew",50),("Por",30),("Seler korzeniowy",30),("Kalafior",80),("Groszek (puszka)",40),("Bulion warzywny",6),("Natka pietruszki",5)}),

            new("Krupnik z kaszą jęczmienną", 45, "wege",
                "1. Kaszę przepłucz.|2. Gotuj z marchwią, selerem, porem i ziemniakami w wodzie z bulionem 30 min.|3. Dopraw, posyp natką.",
                new[]{("Kasza jęczmienna",50.0),("Ziemniaki",120),("Marchew",50),("Por",30),("Seler korzeniowy",30),("Bulion warzywny",6),("Natka pietruszki",5)}),

            new("Krem z brokułów", 35, "wege",
                "1. Brokuła, ziemniaki i cebulę gotuj w wodzie z bulionem 15 min.|2. Zblenduj na krem.|3. Dodaj śmietanę, dopraw.|4. Podawaj z grzankami lub pestkami.",
                new[]{("Brokuł",180.0),("Ziemniaki",120),("Cebula",30),("Śmietana 18%",25),("Bulion warzywny",5),("Czosnek",3)}),

            new("Zupa z soczewicy", 40, "wege",
                "1. Cebulę, czosnek i marchew podsmaż.|2. Dodaj soczewicę, pomidory i wodę z bulionem.|3. Gotuj 20 min, dopraw papryką i kuminem/curry.",
                new[]{("Soczewica czerwona",60.0),("Pomidory krojone (puszka)",100),("Marchew",40),("Cebula",40),("Czosnek",4),("Bulion warzywny",5),("Przyprawa curry",2),("Olej rzepakowy",6)}),

            new("Dhal z soczewicy z ryżem", 35, "wege",
                "1. Cebulę i czosnek podsmaż z curry.|2. Dodaj soczewicę i pomidory, wlej wodę.|3. Gotuj 20 min do rozpadnięcia soczewicy.|4. Podawaj z ryżem i jogurtem.",
                new[]{("Soczewica czerwona",70.0),("Ryż biały",60),("Pomidory krojone (puszka)",100),("Cebula",40),("Czosnek",4),("Przyprawa curry",3),("Jogurt naturalny",30),("Olej rzepakowy",6)}),

            new("Curry z ciecierzycy ze szpinakiem", 30, "wege",
                "1. Cebulę i czosnek podsmaż z curry.|2. Dodaj ciecierzycę i pomidory, duś 10 min.|3. Dodaj szpinak, zagotuj.|4. Podawaj z ryżem.",
                new[]{("Ciecierzyca (puszka)",150.0),("Szpinak mrożony",80),("Ryż biały",60),("Pomidory krojone (puszka)",100),("Cebula",40),("Czosnek",4),("Przyprawa curry",3),("Olej rzepakowy",8)}),

            new("Kotleciki z ciecierzycy", 35, "wege",
                "1. Ciecierzycę rozgnieć z jajkiem, mąką i przyprawami.|2. Formuj kotleciki i smaż na oleju.|3. Podawaj z jogurtem czosnkowym i surówką z pomidorów.",
                new[]{("Ciecierzyca (puszka)",180.0),("Jajka",25),("Mąka pszenna",20),("Cebula",30),("Jogurt naturalny",50),("Czosnek",4),("Pomidory świeże",80),("Olej rzepakowy",12)}),

            new("Gulasz warzywny z soczewicą i kaszą", 45, "wege",
                "1. Cebulę, paprykę i cukinię podsmaż.|2. Dodaj soczewicę, pomidory i koncentrat, wlej wodę z bulionem.|3. Duś 20 min, dopraw papryką.|4. Podawaj z kaszą.",
                new[]{("Soczewica czerwona",50.0),("Kasza gryczana",60),("Cukinia",100),("Papryka czerwona",50),("Pomidory krojone (puszka)",100),("Cebula",40),("Koncentrat pomidorowy",10),("Bulion warzywny",4),("Olej rzepakowy",8),("Papryka słodka mielona",2)}),

            new("Leczo wegetariańskie z cukinią", 35, "wege",
                "1. Cebulę i paprykę podsmaż.|2. Dodaj cukinię i pieczarki, smaż 5 min.|3. Wlej pomidory i koncentrat, dopraw papryką, duś 15 min.|4. Podawaj z kaszą lub pieczywem.",
                new[]{("Cukinia",150.0),("Papryka czerwona",70),("Pieczarki",80),("Pomidory krojone (puszka)",100),("Cebula",40),("Koncentrat pomidorowy",10),("Olej rzepakowy",8),("Papryka słodka mielona",2),("Kasza gryczana",50)}),

            new("Penne z cukinią i mozzarellą", 25, "wege",
                "1. Makaron ugotuj.|2. Cukinię i czosnek podsmaż na oliwie.|3. Dodaj pomidory, duś 5 min.|4. Wymieszaj z makaronem, dodaj porwaną mozzarellę.",
                new[]{("Makaron penne",85.0),("Cukinia",120),("Pomidory krojone (puszka)",80),("Ser mozzarella",40),("Czosnek",4),("Olej rzepakowy",8)}),

            new("Makaron ze szpinakiem i twarogiem", 25, "wege",
                "1. Makaron ugotuj.|2. Szpinak rozmroź na patelni z czosnkiem.|3. Dodaj śmietanę i twaróg, zagotuj.|4. Wymieszaj z makaronem, dopraw.",
                new[]{("Makaron świderki",85.0),("Szpinak mrożony",100),("Twaróg półtłusty",60),("Śmietana 18%",25),("Czosnek",5),("Olej rzepakowy",6)}),

            new("Risotto z pieczarkami", 35, "wege",
                "1. Pieczarki podsmaż na maśle, odłóż.|2. Cebulę zeszklij, dodaj ryż, smaż chwilę.|3. Podlewaj bulionem, mieszając ok. 18 min.|4. Dodaj pieczarki, masło i ser, wymieszaj.",
                new[]{("Ryż biały",85.0),("Pieczarki",100),("Cebula",40),("Masło",15),("Ser żółty gouda",20),("Bulion warzywny",6)}),

            new("Placki z cukinii", 30, "wege",
                "1. Cukinię zetrzyj, posól i odciśnij.|2. Wymieszaj z jajkiem, mąką i czosnkiem.|3. Smaż placki z obu stron.|4. Podawaj z jogurtem.",
                new[]{("Cukinia",250.0),("Mąka pszenna",30),("Jajka",25),("Czosnek",4),("Jogurt naturalny",40),("Olej rzepakowy",15)}),

            new("Omlet z warzywami i serem", 20, "wege",
                "1. Jajka roztrzep z odrobiną mleka.|2. Paprykę, pomidora i cebulę podsmaż.|3. Zalej jajkami, posyp serem, smaż pod przykryciem do ścięcia.",
                new[]{("Jajka",110.0),("Papryka czerwona",50),("Pomidory świeże",60),("Cebula",25),("Ser żółty gouda",25),("Mleko",20),("Olej rzepakowy",8)}),

            new("Frittata z ziemniakami i szpinakiem", 35, "wege",
                "1. Ugotowane ziemniaki pokrój w plastry i podsmaż.|2. Dodaj szpinak i czosnek.|3. Zalej roztrzepanymi jajkami, posyp serem.|4. Dopiecz w piekarniku 10 min w 180°C.",
                new[]{("Jajka",100.0),("Ziemniaki",180),("Szpinak mrożony",70),("Czosnek",4),("Ser żółty gouda",25),("Olej rzepakowy",10)}),

            new("Naleśniki ze szpinakiem i serem", 35, "wege",
                "1. Usmaż naleśniki z mąki, mleka i jajek.|2. Szpinak podsmaż z czosnkiem, wymieszaj z twarogiem.|3. Nakładaj farsz, zawijaj i podsmaż lub zapiecz.",
                new[]{("Mąka pszenna",60.0),("Mleko",120),("Jajka",30),("Szpinak mrożony",80),("Twaróg półtłusty",70),("Czosnek",4),("Olej rzepakowy",10)}),

            new("Racuchy owsiane z bananem", 25, "wege,lubiane-przez-dzieci",
                "1. Płatki zalej mlekiem i odstaw na 10 min.|2. Dodaj jajko, mąkę i rozgniecionego banana.|3. Smaż małe placki na oleju.|4. Podawaj z jogurtem lub startym jabłkiem.",
                new[]{("Płatki owsiane",50.0),("Mleko",80),("Jajka",25),("Banany",80),("Mąka pszenna",20),("Cukier",10),("Olej rzepakowy",10)}),

            new("Kalafior zapiekany z serem i ziemniakami", 45, "wege",
                "1. Kalafiora i ziemniaki ugotuj na półtwardo.|2. Przełóż do naczynia, zalej śmietaną z czosnkiem.|3. Posyp serem i zapiekaj 20 min w 200°C.",
                new[]{("Kalafior",200.0),("Ziemniaki",200),("Śmietana 18%",40),("Ser żółty gouda",35),("Czosnek",4),("Masło",8)}),
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
