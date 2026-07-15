using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Data;

/// <summary>
/// Rozszerzenie danych startowych: dodatkowe składniki i promocje potrzebne,
/// by katalog liczył ≥200 różnorodnych przepisów (patrz SeedData.Recipes2/3).
/// </summary>
public static partial class SeedData
{
    private static List<IngDef> ExtraIngredients() => new()
    {
        //   nazwa                              dział               P     C     F     K     Biedr.  Lidl    Auchan  pack(g)
        // --- Mięso i ryby ---
        new("Karkówka wieprzowa",               "Mięso",            17,   0,    21,   260,  18.99m, 19.49m, 17.99m, 1000),
        new("Boczek surowy",                    "Mięso",            9,    0,    53,   520,  15.99m, 15.49m, 14.99m, 500),
        new("Żeberka wieprzowe",                "Mięso",            15,   0,    25,   290,  16.99m, 16.49m, 15.99m, 1000),
        new("Golonka wieprzowa",                "Mięso",            18,   0,    15,   210,  12.99m, 12.49m, 11.99m, 1000),
        new("Kiełbasa biała surowa",            "Mięso",            13,   1,    28,   300,  13.99m, 13.49m, 12.99m, 500),
        new("Boczek wędzony",                   "Mięso",            12,   1,    45,   460,  17.99m, 16.99m, 16.49m, 300),
        new("Parówki",                          "Mięso",            11,   2,    24,   270,  8.99m,  7.99m,  7.49m,  400),
        new("Szynka gotowana",                  "Mięso",            18,   1,    8,    150,  12.99m, 11.99m, 11.49m, 200),
        new("Wołowina gulaszowa",               "Mięso",            20,   0,    10,   180,  28.99m, 29.49m, 26.99m, 1000),
        new("Wołowina mielona",                 "Mięso",            20,   0,    12,   190,  26.99m, 27.49m, 24.99m, 1000),
        new("Wołowina na stek",                 "Mięso",            22,   0,    8,    170,  44.99m, 45.99m, 39.99m, 1000),
        new("Pierś z indyka",                   "Mięso",            24,   0,    1.5,  110,  24.99m, 24.49m, 22.99m, 1000),
        new("Filet z dorsza (mrożony)",         "Mięso",            18,   0,    0.7,  82,   32.99m, 31.99m, 29.99m, 1000),
        new("Łosoś (filet)",                    "Mięso",            20,   0,    13,   200,  39.99m, 38.99m, 36.99m, 500),
        new("Makrela wędzona",                  "Mięso",            21,   0,    16,   230,  15.99m, 14.99m, 14.49m, 300),
        new("Śledź solony",                     "Mięso",            17,   0,    12,   180,  12.99m, 11.99m, 11.49m, 500),
        new("Krewetki (mrożone)",               "Mięso",            18,   1,    1.5,  90,   24.99m, 22.99m, 21.99m, 300),

        // --- Warzywa ---
        new("Buraki",                           "Warzywa",          1.6,  10,   0.1,  43,   3.49m,  2.99m,  2.79m,  1000),
        new("Dynia",                            "Warzywa",          1,    7,    0.1,  26,   4.99m,  3.99m,  3.99m,  1000),
        new("Bakłażan",                         "Warzywa",          1,    6,    0.2,  25,   6.99m,  5.99m,  5.49m,  500),
        new("Fasolka szparagowa (mrożona)",     "Warzywa",          1.8,  7,    0.1,  31,   5.49m,  4.99m,  4.79m,  450),
        new("Groszek zielony (mrożony)",        "Warzywa",          5,    14,   0.4,  81,   4.49m,  3.99m,  3.79m,  450),
        new("Sałata lodowa",                    "Warzywa",          1,    3,    0.1,  14,   4.49m,  3.99m,  3.79m,  300),
        new("Rukola",                           "Warzywa",          2.6,  3.7,  0.7,  25,   5.99m,  5.49m,  5.29m,  100),
        new("Kapusta pekińska",                 "Warzywa",          1.2,  3,    0.2,  16,   4.99m,  4.49m,  3.99m,  700),
        new("Kapusta czerwona",                 "Warzywa",          1.4,  7,    0.2,  31,   4.49m,  3.99m,  3.49m,  1000),
        new("Kapusta włoska",                   "Warzywa",          2,    5,    0.2,  30,   4.99m,  4.49m,  3.99m,  1000),
        new("Ogórek świeży",                    "Warzywa",          0.7,  3.6,  0.1,  15,   4.99m,  4.49m,  3.99m,  400),
        new("Rzodkiewka",                       "Warzywa",          0.7,  3.4,  0.1,  16,   3.49m,  2.99m,  2.79m,  150),
        new("Bataty",                           "Warzywa",          1.6,  20,   0.1,  86,   7.99m,  6.99m,  6.49m,  1000),
        new("Cebula czerwona",                  "Warzywa",          1.2,  9,    0.1,  42,   4.49m,  3.99m,  3.79m,  500),
        new("Papryka zielona",                  "Warzywa",          0.9,  4.6,  0.2,  20,   5.49m,  4.99m,  4.79m,  500),
        new("Papryczka chili",                  "Warzywa",          2,    9,    0.4,  40,   3.99m,  3.49m,  3.29m,  50),
        new("Imbir",                            "Warzywa",          1.8,  18,   0.8,  80,   3.99m,  3.49m,  2.99m,  100),
        new("Pomidorki koktajlowe",             "Warzywa",          0.9,  3.9,  0.2,  18,   6.99m,  5.99m,  5.79m,  250),

        // --- Owoce ---
        new("Cytryna",                          "Owoce",            0.9,  9,    0.3,  29,   2.49m,  1.99m,  1.89m,  150),
        new("Gruszki",                          "Owoce",            0.4,  15,   0.1,  57,   5.49m,  4.99m,  4.49m,  1000),
        new("Truskawki (mrożone)",              "Owoce",            0.7,  7,    0.3,  32,   8.99m,  7.99m,  7.49m,  450),
        new("Śliwki",                           "Owoce",            0.7,  11,   0.3,  46,   5.99m,  4.99m,  4.79m,  1000),

        // --- Nabiał ---
        new("Ser feta",                         "Nabiał",           14,   4,    21,   260,  5.99m,  5.49m,  5.29m,  150),
        new("Serek wiejski",                    "Nabiał",           12,   3.5,  4,    98,   3.49m,  2.99m,  2.89m,  200),
        new("Jogurt grecki",                    "Nabiał",           5,    4,    9,    115,  4.99m,  4.49m,  4.29m,  400),
        new("Kefir",                            "Nabiał",           3.3,  4,    2,    50,   3.49m,  2.99m,  2.89m,  1000),
        new("Śmietana 30%",                     "Nabiał",           2.4,  3.5,  30,   292,  4.49m,  3.99m,  3.89m,  200),
        new("Parmezan",                         "Nabiał",           32,   4,    29,   400,  9.99m,  8.99m,  8.49m,  100),
        new("Ser pleśniowy",                    "Nabiał",           19,   0.5,  27,   330,  6.99m,  6.49m,  5.99m,  120),

        // --- Produkty sypkie ---
        new("Ryż basmati",                      "Produkty sypkie",  7,    78,   0.9,  360,  7.99m,  6.99m,  6.49m,  1000),
        new("Ryż brązowy",                      "Produkty sypkie",  7.5,  72,   2.7,  360,  6.99m,  5.99m,  5.49m,  1000),
        new("Kasza jaglana",                    "Produkty sypkie",  11,   73,   3.9,  378,  4.99m,  4.49m,  3.99m,  400),
        new("Kasza bulgur",                     "Produkty sypkie",  12,   63,   1.7,  342,  5.99m,  5.49m,  4.99m,  500),
        new("Kasza kuskus",                     "Produkty sypkie",  13,   72,   0.6,  376,  4.99m,  4.49m,  3.99m,  500),
        new("Makaron łazanki",                  "Produkty sypkie",  12,   72,   1.5,  350,  3.99m,  3.69m,  3.49m,  500),
        new("Makaron pełnoziarnisty",           "Produkty sypkie",  13,   66,   2.5,  340,  4.99m,  4.49m,  3.99m,  500),
        new("Makaron nitki",                    "Produkty sypkie",  12,   72,   1.5,  350,  2.99m,  2.79m,  2.59m,  250),
        new("Bułka tarta",                      "Produkty sypkie",  12,   72,   5,    380,  3.49m,  2.99m,  2.79m,  500),
        new("Mąka ziemniaczana",                "Produkty sypkie",  0.5,  83,   0.1,  340,  2.99m,  2.79m,  2.59m,  500),
        new("Kasza manna",                      "Produkty sypkie",  11,   73,   1,    350,  2.99m,  2.49m,  2.29m,  500),
        new("Orzechy włoskie",                  "Produkty sypkie",  15,   14,   65,   654,  12.99m, 11.99m, 10.99m, 200),
        new("Słonecznik łuskany",               "Produkty sypkie",  21,   20,   51,   584,  6.99m,  5.99m,  5.49m,  200),
        new("Rodzynki",                         "Produkty sypkie",  3,    79,   0.5,  299,  5.99m,  5.49m,  4.99m,  200),
        new("Miód",                             "Produkty sypkie",  0.3,  82,   0,    304,  15.99m, 14.99m, 13.99m, 400),
        new("Kakao",                            "Produkty sypkie",  20,   58,   11,   350,  6.99m,  5.99m,  5.49m,  200),

        // --- Konserwy i słoiki ---
        new("Passata pomidorowa",               "Konserwy",         1.5,  5,    0.2,  30,   4.49m,  3.99m,  3.79m,  500),
        new("Mleko kokosowe",                   "Konserwy",         2,    3,    20,   200,  5.99m,  4.99m,  4.79m,  400),
        new("Oliwki czarne",                    "Konserwy",         1,    6,    15,   150,  5.99m,  5.49m,  4.99m,  300),
        new("Pomidory suszone",                 "Konserwy",         3.5,  13,   12,   200,  9.99m,  8.99m,  8.49m,  280),
        new("Fasola czarna (puszka)",           "Konserwy",         8,    20,   0.5,  114,  4.49m,  3.99m,  3.89m,  400),
        new("Śliwki suszone",                   "Konserwy",         2,    64,   0.4,  240,  6.99m,  5.99m,  5.49m,  300),
        new("Ananas (puszka)",                  "Konserwy",         0.4,  13,   0.1,  55,   5.49m,  4.99m,  4.79m,  565),
        new("Groch łuskany",                    "Produkty sypkie",  23,   49,   1.4,  310,  4.99m,  4.49m,  4.29m,  500),

        // --- Tłuszcze ---
        new("Oliwa z oliwek",                   "Tłuszcze",         0,    0,    100,  900,  24.99m, 21.99m, 19.99m, 500),
        new("Smalec",                           "Tłuszcze",         0,    0,    99,   890,  5.99m,  4.99m,  4.79m,  250),

        // --- Przyprawy i sosy ---
        new("Oregano",                          "Przyprawy",        11,   65,   10,   265,  2.99m,  2.49m,  2.29m,  15),
        new("Bazylia suszona",                  "Przyprawy",        13,   48,   4,    250,  2.99m,  2.49m,  2.29m,  15),
        new("Tymianek",                         "Przyprawy",        9,    45,   7,    280,  2.99m,  2.49m,  2.29m,  15),
        new("Kmin rzymski",                     "Przyprawy",        18,   44,   22,   375,  3.99m,  3.49m,  3.29m,  20),
        new("Musztarda",                        "Przyprawy",        5,    6,    4,    90,   3.99m,  3.49m,  3.29m,  200),
        new("Ketchup",                          "Przyprawy",        1.2,  26,   0.2,  110,  4.99m,  3.99m,  3.79m,  450),
        new("Ocet jabłkowy",                    "Przyprawy",        0,    1,    0,    20,   4.99m,  3.99m,  3.79m,  500),
        new("Liść laurowy",                     "Przyprawy",        8,    48,   8,    310,  2.49m,  1.99m,  1.89m,  10),
        new("Ziele angielskie",                 "Przyprawy",        7,    51,   9,    315,  2.99m,  2.49m,  2.29m,  15),
        new("Kurkuma",                          "Przyprawy",        8,    65,   10,   354,  3.49m,  2.99m,  2.79m,  25),
        new("Chrzan tarty",                     "Przyprawy",        1.2,  11,   0.7,  48,   4.49m,  3.99m,  3.79m,  180),
        new("Cynamon",                          "Przyprawy",        4,    81,   1.2,  247,  3.49m,  2.99m,  2.79m,  20),
        new("Bulion drobiowy",                  "Przyprawy",        10,   20,   30,   250,  4.49m,  3.99m,  3.79m,  120),
        new("Wędzona papryka",                  "Przyprawy",        14,   54,   13,   340,  3.99m,  3.49m,  3.29m,  20),

        // --- Pieczywo (dział „Inne") ---
        new("Chleb pszenny",                    "Inne",             8,    49,   3,    265,  4.99m,  3.99m,  3.79m,  500),
        new("Bułka pszenna",                    "Inne",             9,    55,   3,    290,  0.99m,  0.89m,  0.79m,  60),
        new("Tortilla pszenna",                 "Inne",             8,    50,   7,    310,  6.99m,  5.99m,  5.79m,  320),
        new("Chleb żytni",                      "Inne",             7,    45,   1.5,  240,  5.99m,  4.99m,  4.79m,  500),
    };

    private static List<PromoDef> ExtraPromos() => new()
    {
        new("Karkówka wieprzowa",       Store.Biedronka, 14.99m, null),
        new("Pierś z indyka",           Store.Biedronka, 19.99m, "Moja Biedronka"),
        new("Ser feta",                 Store.Biedronka, 4.49m,  null),
        new("Passata pomidorowa",       Store.Biedronka, 2.99m,  "przy zakupie 2 szt."),
        new("Bataty",                   Store.Biedronka, 5.99m,  null),
        new("Łosoś (filet)",            Store.Lidl,      29.99m, "Lidl Plus"),
        new("Wołowina gulaszowa",       Store.Lidl,      23.99m, null),
        new("Jogurt grecki",            Store.Lidl,      3.49m,  null),
        new("Bakłażan",                 Store.Lidl,      4.99m,  null),
        new("Oliwa z oliwek",           Store.Lidl,      19.99m, "Lidl Plus"),
        new("Filet z dorsza (mrożony)", Store.Auchan,    26.99m, null),
        new("Boczek wędzony",           Store.Auchan,    13.99m, null),
        new("Dynia",                    Store.Auchan,    2.99m,  "sezon"),
        new("Parmezan",                 Store.Auchan,    7.99m,  null),
        new("Kasza jaglana",            Store.Auchan,    3.49m,  null),
        new("Krewetki (mrożone)",       Store.Biedronka, 19.99m, null),
    };
}
