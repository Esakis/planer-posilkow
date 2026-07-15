namespace TaniTydzien.Api.Data;

/// <summary>Dodatkowe przepisy, część 2: wegetariańskie, makarony, mączne, zapiekanki, sałatki.</summary>
public static partial class SeedData
{
    private static List<RecDef> ExtraRecipes3() => new()
    {
        // ---------------- WEGETARIAŃSKIE ----------------
        new("Ratatouille", 45, "wege",
            "1. Bakłażan, cukinię i paprykę pokrój w kostkę.|2. Zeszklij cebulę z czosnkiem, dodaj warzywa.|3. Wlej pomidory, dopraw tymiankiem, duś 25 min.",
            new[]{("Bakłażan",120.0),("Cukinia",120),("Papryka czerwona",80),("Pomidory krojone (puszka)",120),("Cebula",50),("Czosnek",6),("Oliwa z oliwek",15),("Tymianek",1)}, "Wegetariańskie"),

        new("Kotlety z kaszy jaglanej", 40, "wege",
            "1. Kaszę ugotuj.|2. Zetrzyj marchew, podsmaż z cebulą.|3. Wymieszaj z kaszą, jajkiem i natką, formuj kotlety, panieruj i smaż.",
            new[]{("Kasza jaglana",70.0),("Jajka",25),("Marchew",60),("Cebula",40),("Bułka tarta",40),("Natka pietruszki",5),("Olej rzepakowy",15)}, "Wegetariańskie"),

        new("Falafel z ciecierzycy", 35, "wege",
            "1. Ciecierzycę zblenduj z czosnkiem, cebulą i przyprawami.|2. Dodaj mąkę i natkę, uformuj kulki.|3. Usmaż na złoto, podawaj z sosem jogurtowym.",
            new[]{("Ciecierzyca (puszka)",180.0),("Czosnek",6),("Natka pietruszki",8),("Kmin rzymski",3),("Mąka pszenna",25),("Cebula",40),("Olej rzepakowy",18)}, "Wegetariańskie"),

        new("Papryka faszerowana kaszą", 55, "wege",
            "1. Kaszę ugotuj, wymieszaj z podsmażonymi pieczarkami i cebulą.|2. Napełnij papryki, ułóż w naczyniu.|3. Zalej passatą, posyp serem, zapiekaj 30 min.",
            new[]{("Papryka czerwona",180.0),("Kasza gryczana",70),("Pieczarki",80),("Cebula",40),("Ser żółty gouda",30),("Passata pomidorowa",100),("Czosnek",4)}, "Wegetariańskie"),

        new("Curry z batatów i ciecierzycy", 40, "wege",
            "1. Cebulę, imbir i curry podsmaż.|2. Dodaj bataty i ciecierzycę, wlej mleko kokosowe.|3. Duś 20 min, na koniec dorzuć szpinak.",
            new[]{("Bataty",180.0),("Ciecierzyca (puszka)",120),("Mleko kokosowe",120),("Cebula",40),("Imbir",6),("Przyprawa curry",4),("Szpinak mrożony",60)}, "Wegetariańskie"),

        new("Kotlety z brokuła i sera", 35, "wege",
            "1. Brokuł ugotuj i rozdrobnij.|2. Wymieszaj z jajkiem, startym serem i bułką tartą.|3. Formuj kotlety, smaż z czosnkiem na oleju.",
            new[]{("Brokuł",180.0),("Jajka",30),("Ser żółty gouda",40),("Bułka tarta",40),("Czosnek",5),("Olej rzepakowy",15)}, "Wegetariańskie"),

        new("Leczo z bakłażanem", 40, "wege",
            "1. Bakłażan, cukinię i paprykę podsmaż.|2. Dodaj cebulę i pomidory.|3. Dopraw papryką, duś 20 min, podawaj z kaszą.",
            new[]{("Bakłażan",120.0),("Papryka czerwona",70),("Cukinia",100),("Cebula",40),("Pomidory krojone (puszka)",120),("Papryka słodka mielona",3),("Kasza jęczmienna",60)}, "Wegetariańskie"),

        new("Pieczone warzywa z kaszą kuskus", 40, "wege",
            "1. Cukinię, bakłażan i paprykę upiecz z oliwą.|2. Kuskus zalej wrzątkiem.|3. Wymieszaj z warzywami i cebulą, posyp fetą.",
            new[]{("Cukinia",120.0),("Bakłażan",100),("Papryka czerwona",80),("Cebula czerwona",40),("Kasza kuskus",70),("Oliwa z oliwek",15),("Ser feta",50)}, "Wegetariańskie"),

        new("Risotto z dynią", 40, "wege",
            "1. Dynię podduś, cebulę zeszklij, dodaj ryż.|2. Podlewaj bulionem, mieszając 18 min.|3. Dodaj dynię, parmezan, masło i tymianek.",
            new[]{("Ryż biały",85.0),("Dynia",150),("Cebula",40),("Parmezan",25),("Masło",15),("Bulion warzywny",6),("Tymianek",1)}, "Wegetariańskie"),

        new("Gołąbki wegetariańskie z kaszą", 60, "wege",
            "1. Kaszę ugotuj, wymieszaj z pieczarkami, marchwią i cebulą.|2. Zawiń w sparzone liście kapusty.|3. Zalej passatą z czosnkiem, duś 30 min.",
            new[]{("Kapusta biała",180.0),("Kasza gryczana",60),("Pieczarki",80),("Cebula",40),("Marchew",40),("Passata pomidorowa",100),("Czosnek",4)}, "Wegetariańskie"),

        new("Zapiekane bataty z czarną fasolą", 45, "wege",
            "1. Bataty upiecz w plastrach.|2. Fasolę i kukurydzę podsmaż z cebulą i kminem.|3. Wyłóż na bataty, posyp serem, zapiekaj 10 min.",
            new[]{("Bataty",250.0),("Fasola czarna (puszka)",120),("Kukurydza (puszka)",60),("Ser żółty gouda",40),("Cebula czerwona",40),("Kmin rzymski",2)}, "Wegetariańskie"),

        new("Frytki z batata z dipem", 30, "wege,lubiane-przez-dzieci",
            "1. Bataty pokrój w słupki, wymieszaj z oliwą i wędzoną papryką.|2. Piecz 25 min w 210°C.|3. Podawaj z dipem z jogurtu greckiego i czosnku oraz rukolą.",
            new[]{("Bataty",250.0),("Oliwa z oliwek",15),("Jogurt grecki",60),("Czosnek",5),("Wędzona papryka",3),("Rukola",30)}, "Wegetariańskie"),

        new("Jajka faszerowane pieczarkami", 30, "wege",
            "1. Jajka ugotuj na twardo, przekrój, wyjmij żółtka.|2. Pieczarki i cebulę podsmaż, wymieszaj z żółtkami i koperkiem.|3. Napełnij jajka, obtocz w bułce i podsmaż.",
            new[]{("Jajka",120.0),("Pieczarki",100),("Cebula",30),("Jogurt grecki",30),("Koperek",4),("Bułka tarta",25)}, "Wegetariańskie"),

        new("Omlet ze szpinakiem i fetą", 20, "wege",
            "1. Szpinak podsmaż z cebulą.|2. Jajka roztrzep z mlekiem, wlej na patelnię.|3. Dodaj szpinak i pokruszoną fetę, smaż pod przykryciem.",
            new[]{("Jajka",110.0),("Szpinak mrożony",80),("Ser feta",40),("Cebula",25),("Mleko",20),("Oliwa z oliwek",8)}, "Wegetariańskie"),

        new("Curry z kalafiora", 35, "wege",
            "1. Cebulę, imbir i curry podsmaż.|2. Dodaj kalafior i groszek, wlej mleko kokosowe.|3. Duś 18 min, podawaj z ryżem basmati.",
            new[]{("Kalafior",200.0),("Mleko kokosowe",120),("Cebula",40),("Imbir",6),("Przyprawa curry",4),("Groszek zielony (mrożony)",60),("Ryż basmati",70)}, "Wegetariańskie"),

        new("Fasola po meksykańsku", 30, "wege",
            "1. Cebulę i paprykę podsmaż z chili.|2. Dodaj fasolę, kukurydzę i passatę, duś 15 min.|3. Podawaj z ugotowanym ryżem.",
            new[]{("Fasola czerwona (puszka)",180.0),("Kukurydza (puszka)",70),("Papryka czerwona",60),("Cebula",40),("Passata pomidorowa",120),("Ryż biały",60),("Papryczka chili",4)}, "Wegetariańskie"),

        new("Pieczony kalafior z ciecierzycą", 40, "wege",
            "1. Kalafior i ciecierzycę wymieszaj z oliwą, kurkumą i czosnkiem.|2. Piecz 30 min w 210°C.|3. Skrop jogurtem, posyp natką.",
            new[]{("Kalafior",200.0),("Ciecierzyca (puszka)",120),("Oliwa z oliwek",15),("Kurkuma",2),("Czosnek",5),("Jogurt grecki",50),("Natka pietruszki",5)}, "Wegetariańskie"),

        new("Dynia pieczona z kaszą i fetą", 40, "wege",
            "1. Dynię upiecz w kostce z oliwą.|2. Kaszę jaglaną ugotuj.|3. Wymieszaj z dynią, fetą, rukolą, cebulą i słonecznikiem.",
            new[]{("Dynia",200.0),("Kasza jaglana",70),("Ser feta",50),("Rukola",30),("Cebula czerwona",30),("Oliwa z oliwek",12),("Słonecznik łuskany",15)}, "Wegetariańskie"),

        // ---------------- MAKARONY ----------------
        new("Spaghetti carbonara", 25, "mięso,wieprzowina",
            "1. Makaron ugotuj al dente.|2. Boczek podsmaż z czosnkiem.|3. Jajka roztrzep z parmezanem i śmietaną, połącz z gorącym makaronem i boczkiem.",
            new[]{("Makaron spaghetti",90.0),("Boczek wędzony",60),("Jajka",50),("Parmezan",25),("Czosnek",4),("Śmietana 18%",30)}, "Makarony"),

        new("Penne arrabiata", 20, "wege",
            "1. Makaron ugotuj.|2. Czosnek i chili podsmaż na oliwie.|3. Wlej passatę, dopraw bazylią, duś 10 min, wymieszaj z makaronem.",
            new[]{("Makaron penne",90.0),("Passata pomidorowa",150),("Czosnek",6),("Papryczka chili",4),("Oliwa z oliwek",12),("Bazylia suszona",2)}, "Makarony"),

        new("Makaron z sosem serowo-orzechowym", 25, "wege",
            "1. Makaron ugotuj.|2. Por podsmaż, dodaj ser pleśniowy i śmietanę, roztop.|3. Wymieszaj z makaronem, posyp orzechami.",
            new[]{("Makaron penne",90.0),("Ser pleśniowy",60),("Śmietana 30%",50),("Orzechy włoskie",20),("Czosnek",4),("Por",40)}, "Makarony"),

        new("Lasagne bolognese", 60, "mięso,wołowina",
            "1. Mięso podsmaż z cebulą, wlej passatę, duś.|2. Zrób beszamel z mleka i mąki.|3. Układaj warstwy makaronu, sosu i sera, zapiekaj 30 min.",
            new[]{("Makaron łazanki",90.0),("Mięso mielone wieprzowo-wołowe",110),("Passata pomidorowa",150),("Ser żółty gouda",40),("Cebula",40),("Mleko",100),("Mąka pszenna",15)}, "Makarony"),

        new("Spaghetti aglio e olio", 15, "wege",
            "1. Makaron ugotuj.|2. Czosnek i chili podsmaż na oliwie.|3. Wymieszaj z makaronem, natką i parmezanem.",
            new[]{("Makaron spaghetti",90.0),("Czosnek",8),("Oliwa z oliwek",20),("Papryczka chili",4),("Natka pietruszki",6),("Parmezan",20)}, "Makarony"),

        new("Makaron z brokułem i czosnkiem", 25, "wege",
            "1. Makaron ugotuj, brokuł sparz.|2. Podsmaż czosnek na oliwie, dodaj brokuł.|3. Wymieszaj z makaronem, posyp parmezanem i słonecznikiem.",
            new[]{("Makaron penne",90.0),("Brokuł",120),("Czosnek",6),("Oliwa z oliwek",15),("Parmezan",25),("Słonecznik łuskany",15)}, "Makarony"),

        new("Makaron z tuńczykiem i oliwkami", 25, "ryby",
            "1. Makaron ugotuj.|2. Cebulę i czosnek podsmaż, wlej passatę.|3. Dodaj tuńczyka i oliwki, wymieszaj z makaronem.",
            new[]{("Makaron świderki",90.0),("Tuńczyk (puszka)",70),("Oliwki czarne",30),("Passata pomidorowa",120),("Czosnek",5),("Cebula",40)}, "Makarony"),

        new("Spaghetti z pesto szpinakowym", 25, "wege",
            "1. Makaron ugotuj.|2. Zblenduj szpinak z orzechami, parmezanem, czosnkiem i oliwą.|3. Wymieszaj pesto z makaronem.",
            new[]{("Makaron spaghetti",90.0),("Szpinak mrożony",80),("Orzechy włoskie",25),("Parmezan",25),("Czosnek",4),("Oliwa z oliwek",15)}, "Makarony"),

        new("Makaron z cukinią i boczkiem", 25, "mięso,wieprzowina",
            "1. Makaron ugotuj.|2. Boczek podsmaż, dodaj cukinię i czosnek.|3. Wlej śmietanę, wymieszaj z makaronem, posyp parmezanem.",
            new[]{("Makaron penne",90.0),("Cukinia",120),("Boczek wędzony",50),("Śmietana 18%",40),("Czosnek",5),("Parmezan",20)}, "Makarony"),

        new("Makaron zapiekany z warzywami", 45, "wege",
            "1. Makaron ugotuj.|2. Cukinię i paprykę podsmaż z cebulą, wlej passatę.|3. Wymieszaj z makaronem, posyp mozzarellą, zapiekaj 20 min.",
            new[]{("Makaron świderki",90.0),("Cukinia",100),("Papryka czerwona",70),("Passata pomidorowa",120),("Ser mozzarella",50),("Cebula",40),("Bazylia suszona",2)}, "Makarony"),

        new("Makaron z kurczakiem i suszonymi pomidorami", 30, "mięso,drob",
            "1. Makaron ugotuj.|2. Kurczaka obsmaż z czosnkiem, dodaj suszone pomidory.|3. Wlej śmietanę, dorzuć szpinak, wymieszaj z makaronem.",
            new[]{("Makaron penne",90.0),("Pierś z kurczaka",110),("Pomidory suszone",40),("Śmietana 30%",50),("Szpinak mrożony",60),("Czosnek",5)}, "Makarony"),

        new("Makaron z łososiem w sosie śmietanowym", 25, "ryby",
            "1. Makaron ugotuj.|2. Łososia podsmaż z czosnkiem, rozdrobnij.|3. Wlej śmietanę, dodaj koperek i cytrynę, wymieszaj z makaronem.",
            new[]{("Makaron penne",90.0),("Łosoś (filet)",120),("Śmietana 30%",50),("Koperek",6),("Cytryna",15),("Czosnek",4)}, "Makarony"),

        new("Makaron z pieczarkami i szpinakiem", 25, "wege",
            "1. Makaron ugotuj.|2. Pieczarki i cebulę podsmaż, dodaj szpinak i czosnek.|3. Wlej śmietanę, wymieszaj z makaronem.",
            new[]{("Makaron świderki",90.0),("Pieczarki",120),("Szpinak mrożony",80),("Śmietana 18%",40),("Czosnek",4),("Cebula",30)}, "Makarony"),

        new("Makaron po chińsku z warzywami", 25, "wege",
            "1. Makaron ugotuj.|2. Marchew, paprykę i kapustę pekińską smaż na dużym ogniu z imbirem.|3. Dodaj makaron i sos sojowy, wymieszaj.",
            new[]{("Makaron nitki",80.0),("Marchew",60),("Papryka czerwona",60),("Kapusta pekińska",80),("Sos sojowy",15),("Imbir",5),("Cebula",30)}, "Makarony"),

        new("Makaron z kurczakiem po włosku", 30, "mięso,drob",
            "1. Makaron ugotuj.|2. Kurczaka i cebulę obsmaż, wlej passatę z bazylią.|3. Dodaj marchew, duś 10 min, wymieszaj z makaronem.",
            new[]{("Makaron świderki",90.0),("Pierś z kurczaka",110),("Passata pomidorowa",130),("Cebula",40),("Czosnek",5),("Bazylia suszona",2),("Marchew",40)}, "Makarony"),

        new("Makaron z fasolką szparagową i fetą", 25, "wege",
            "1. Makaron ugotuj, fasolkę sparz.|2. Podsmaż czosnek z pomidorkami koktajlowymi.|3. Wymieszaj z makaronem, dodaj fetę i oliwę.",
            new[]{("Makaron penne",90.0),("Fasolka szparagowa (mrożona)",100),("Ser feta",50),("Pomidorki koktajlowe",80),("Oliwa z oliwek",12),("Czosnek",5)}, "Makarony"),

        new("Makaron z dynią i orzechami", 30, "wege",
            "1. Makaron ugotuj.|2. Dynię podduś z czosnkiem, zblenduj ze śmietaną.|3. Wymieszaj z makaronem, posyp parmezanem i orzechami.",
            new[]{("Makaron penne",90.0),("Dynia",150),("Śmietana 30%",50),("Parmezan",25),("Czosnek",4),("Orzechy włoskie",20)}, "Makarony"),

        // ---------------- PIEROGI I MĄCZNE ----------------
        new("Pierogi z mięsem", 75, "mięso,wieprzowina",
            "1. Zagnieć ciasto z mąki i wody.|2. Mięso ugotuj, zmiel z podsmażoną cebulą.|3. Lep pierogi, gotuj w osolonej wodzie.",
            new[]{("Mąka pszenna",100.0),("Mięso mielone wieprzowo-wołowe",90),("Cebula",40),("Jajka",15),("Bulion warzywny",4),("Olej rzepakowy",8)}, "Pierogi i mączne"),

        new("Pierogi z kapustą i grzybami", 75, "wege",
            "1. Zagnieć ciasto z mąki i wody.|2. Kapustę kiszoną uduś z pieczarkami i cebulą.|3. Lep pierogi, gotuj w osolonej wodzie.",
            new[]{("Mąka pszenna",100.0),("Kapusta kiszona",150),("Pieczarki",80),("Cebula",40),("Olej rzepakowy",10)}, "Pierogi i mączne"),

        new("Kopytka z masłem", 45, "wege,lubiane-przez-dzieci",
            "1. Ugotowane ziemniaki przeciśnij, wymieszaj z mąką i jajkiem.|2. Formuj wałki, krój kopytka, gotuj.|3. Podsmaż na maśle z bułką tartą.",
            new[]{("Ziemniaki",300.0),("Mąka pszenna",80),("Jajka",25),("Masło",20),("Bułka tarta",20)}, "Pierogi i mączne"),

        new("Kluski śląskie", 45, "wege,lubiane-przez-dzieci",
            "1. Ugotowane ziemniaki przeciśnij.|2. Dodaj mąkę ziemniaczaną, jajko i mąkę, zagnieć.|3. Formuj kluski z dziurką, gotuj w osolonej wodzie.",
            new[]{("Ziemniaki",320.0),("Mąka ziemniaczana",60),("Jajka",25),("Mąka pszenna",30)}, "Pierogi i mączne"),

        new("Leniwe pierogi", 30, "wege,lubiane-przez-dzieci",
            "1. Twaróg zmiel, wymieszaj z jajkiem, mąką i cukrem.|2. Formuj wałki, krój kluski, gotuj.|3. Podsmaż bułkę na maśle i posyp.",
            new[]{("Twaróg półtłusty",200.0),("Mąka pszenna",70),("Jajka",30),("Bułka tarta",25),("Masło",20),("Cukier",15)}, "Pierogi i mączne"),

        new("Naleśniki z pieczarkami", 35, "wege",
            "1. Usmaż naleśniki z mąki, mleka i jajek.|2. Pieczarki i cebulę podsmaż, wymieszaj z serem.|3. Nadziewaj, zawijaj i podsmaż.",
            new[]{("Mąka pszenna",60.0),("Mleko",120),("Jajka",30),("Pieczarki",100),("Cebula",40),("Ser żółty gouda",30),("Olej rzepakowy",10)}, "Pierogi i mączne"),

        new("Knedle ze śliwkami", 50, "wege,lubiane-przez-dzieci",
            "1. Z ziemniaków, mąki i jajka zagnieć ciasto.|2. Zawiń śliwki w ciasto, gotuj.|3. Podsmaż bułkę na maśle, posyp cukrem z cynamonem.",
            new[]{("Ziemniaki",250.0),("Mąka pszenna",90),("Jajka",25),("Śliwki",120),("Bułka tarta",20),("Cukier",15),("Masło",15),("Cynamon",1)}, "Pierogi i mączne"),

        new("Placki z dyni", 30, "wege,lubiane-przez-dzieci",
            "1. Dynię zetrzyj, odciśnij.|2. Wymieszaj z mąką, jajkiem, cukrem i cynamonem.|3. Smaż placki na oleju.",
            new[]{("Dynia",250.0),("Mąka pszenna",50),("Jajka",25),("Cynamon",1),("Cukier",15),("Olej rzepakowy",15)}, "Pierogi i mączne"),

        new("Tarta warzywna", 55, "wege",
            "1. Z mąki, masła i jajka zagnieć spód, podpiecz.|2. Cukinię podsmaż, ułóż z serem i pomidorkami.|3. Zalej śmietaną z jajkiem, zapiekaj 25 min.",
            new[]{("Mąka pszenna",120.0),("Masło",60),("Jajka",50),("Cukinia",100),("Ser żółty gouda",40),("Śmietana 18%",50),("Pomidorki koktajlowe",80)}, "Pierogi i mączne"),

        new("Pizza domowa", 40, "mięso,wieprzowina",
            "1. Z mąki, wody i oliwy zagnieć ciasto, rozwałkuj.|2. Posmaruj passatą, ułóż szynkę i pieczarki.|3. Posyp mozzarellą i oregano, piecz 12 min w 250°C.",
            new[]{("Mąka pszenna",120.0),("Passata pomidorowa",80),("Ser mozzarella",60),("Szynka gotowana",50),("Pieczarki",50),("Oliwa z oliwek",10),("Oregano",2)}, "Pierogi i mączne"),

        new("Krokiety z kapustą i grzybami", 50, "wege",
            "1. Usmaż naleśniki z mąki, mleka i jajek.|2. Kapustę kiszoną uduś z pieczarkami.|3. Zawiń farsz, panieruj w bułce, usmaż.",
            new[]{("Mąka pszenna",60.0),("Mleko",120),("Jajka",30),("Kapusta kiszona",120),("Pieczarki",70),("Bułka tarta",40),("Olej rzepakowy",15)}, "Pierogi i mączne"),

        new("Pyzy z mięsem", 60, "mięso,wieprzowina",
            "1. Ugotowane ziemniaki wymieszaj z mąką ziemniaczaną i pszenną.|2. Napełnij podsmażonym mięsem z cebulą.|3. Formuj pyzy, gotuj na parze lub w wodzie.",
            new[]{("Ziemniaki",300.0),("Mąka ziemniaczana",50),("Mięso mielone wieprzowo-wołowe",90),("Cebula",40),("Mąka pszenna",40),("Bułka tarta",15)}, "Pierogi i mączne"),

        new("Placki z marchewki z rodzynkami", 30, "wege,lubiane-przez-dzieci",
            "1. Marchew zetrzyj na drobno.|2. Wymieszaj z mąką, jajkiem, cukrem, rodzynkami i bułką tartą.|3. Smaż małe placki na oleju.",
            new[]{("Marchew",250.0),("Mąka pszenna",60),("Jajka",25),("Cukier",12),("Rodzynki",20),("Bułka tarta",20),("Olej rzepakowy",15)}, "Pierogi i mączne"),

        // ---------------- JEDNOGARNKOWE I ZAPIEKANKI ----------------
        new("Zapiekanka ziemniaczana z pieczarkami", 55, "wege",
            "1. Ziemniaki ugotuj, pokrój w plastry.|2. Pieczarki, cebulę i por podsmaż.|3. Układaj warstwami, zalej śmietaną z czosnkiem, posyp serem, zapiekaj.",
            new[]{("Ziemniaki",280.0),("Pieczarki",120),("Cebula",40),("Por",40),("Ser żółty gouda",40),("Śmietana 18%",50),("Czosnek",4)}, "Jednogarnkowe i zapiekanki"),

        new("Musaka z bakłażanem", 65, "mięso,wołowina",
            "1. Bakłażan i ziemniaki podpiecz w plastrach.|2. Mięso uduś z cebulą, czosnkiem i passatą.|3. Układaj warstwy, zalej beszamelem z mleka i mąki, posyp serem, zapiekaj 30 min.",
            new[]{("Bakłażan",180.0),("Ziemniaki",150),("Mięso mielone wieprzowo-wołowe",120),("Passata pomidorowa",120),("Ser żółty gouda",40),("Cebula",40),("Mleko",100),("Mąka pszenna",15),("Czosnek",5)}, "Jednogarnkowe i zapiekanki"),

        new("Zapiekanka z kaszy i warzyw", 45, "wege",
            "1. Kaszę ugotuj.|2. Cukinię i paprykę podsmaż z cebulą, wlej pomidory.|3. Wymieszaj z kaszą i jajkiem, posyp serem, zapiekaj 25 min.",
            new[]{("Kasza gryczana",70.0),("Cukinia",100),("Papryka czerwona",70),("Ser żółty gouda",40),("Pomidory krojone (puszka)",100),("Cebula",40),("Jajka",30)}, "Jednogarnkowe i zapiekanki"),

        new("Gulasz z ciecierzycy i warzyw", 40, "wege",
            "1. Cebulę, batat i paprykę podsmaż z kminem.|2. Dodaj ciecierzycę, cukinię i passatę.|3. Duś 20 min pod przykryciem.",
            new[]{("Ciecierzyca (puszka)",150.0),("Cukinia",100),("Papryka czerwona",60),("Passata pomidorowa",120),("Cebula",40),("Kmin rzymski",3),("Bataty",120)}, "Jednogarnkowe i zapiekanki"),

        new("Jambalaya z kiełbasą", 40, "mięso,wieprzowina",
            "1. Kiełbasę, cebulę i paprykę podsmaż.|2. Wsyp ryż, wlej passatę i wodę.|3. Dopraw wędzoną papryką i chili, gotuj 20 min.",
            new[]{("Ryż biały",80.0),("Kiełbasa śląska",100),("Papryka czerwona",60),("Cebula",40),("Passata pomidorowa",100),("Wędzona papryka",3),("Papryczka chili",4)}, "Jednogarnkowe i zapiekanki"),

        new("Enchiladas z kurczakiem", 50, "mięso,drob",
            "1. Kurczaka podsmaż, wymieszaj z fasolą i kukurydzą.|2. Nakładaj na tortille, zawijaj, ułóż w naczyniu.|3. Zalej passatą, posyp serem, zapiekaj 20 min.",
            new[]{("Tortilla pszenna",120.0),("Pierś z kurczaka",120),("Fasola czerwona (puszka)",80),("Kukurydza (puszka)",50),("Passata pomidorowa",120),("Ser żółty gouda",40),("Cebula",30)}, "Jednogarnkowe i zapiekanki"),

        new("Ziemniaki po francusku", 55, "mięso,wieprzowina",
            "1. Ziemniaki ugotuj, pokrój w plastry.|2. Kiełbasę i cebulę podsmaż.|3. Układaj warstwami z jajkiem, zalej śmietaną, posyp serem, zapiekaj.",
            new[]{("Ziemniaki",280.0),("Kiełbasa śląska",100),("Cebula",50),("Jajka",50),("Ser żółty gouda",40),("Śmietana 18%",50)}, "Jednogarnkowe i zapiekanki"),

        new("Makaron jednogarnkowy z warzywami", 30, "wege",
            "1. Do garnka włóż makaron, cukinię, pomidorki i cebulę.|2. Zalej bulionem, gotuj 12 min mieszając.|3. Dodaj szpinak i czosnek, odparuj.",
            new[]{("Makaron świderki",90.0),("Cukinia",100),("Pomidorki koktajlowe",100),("Szpinak mrożony",60),("Czosnek",5),("Bulion warzywny",5),("Cebula",40)}, "Jednogarnkowe i zapiekanki"),

        new("Zapiekanka kalafiorowa z szynką", 50, "mięso,wieprzowina",
            "1. Kalafior ugotuj na półtwardo.|2. Ułóż z szynką, zalej śmietaną z jajkiem.|3. Posyp serem i bułką, zapiekaj 25 min.",
            new[]{("Kalafior",250.0),("Szynka gotowana",100),("Ser żółty gouda",40),("Śmietana 18%",50),("Jajka",30),("Bułka tarta",20)}, "Jednogarnkowe i zapiekanki"),

        new("Chili sin carne", 35, "wege",
            "1. Cebulę i paprykę podsmaż z kminem.|2. Dodaj obie fasole, kukurydzę i passatę.|3. Duś 20 min, podawaj z ryżem lub pieczywem.",
            new[]{("Fasola czerwona (puszka)",120.0),("Fasola czarna (puszka)",120),("Kukurydza (puszka)",60),("Passata pomidorowa",150),("Papryka czerwona",60),("Cebula",40),("Kmin rzymski",3)}, "Jednogarnkowe i zapiekanki"),

        new("Zapiekane naleśniki z mięsem", 55, "mięso,wołowina",
            "1. Usmaż naleśniki z mąki, mleka i jajek.|2. Mięso uduś z cebulą, czosnkiem i passatą, nadziewaj naleśniki.|3. Ułóż w naczyniu, posyp serem i natką, zapiekaj 20 min.",
            new[]{("Mąka pszenna",60.0),("Mleko",120),("Jajka",30),("Mięso mielone wieprzowo-wołowe",110),("Passata pomidorowa",100),("Ser żółty gouda",40),("Cebula",40),("Czosnek",5),("Natka pietruszki",5)}, "Jednogarnkowe i zapiekanki"),

        new("Warzywa zapiekane z fetą i kaszą bulgur", 45, "wege",
            "1. Cukinię, bakłażan i pomidorki upiecz z oliwą.|2. Bulgur zalej wrzątkiem.|3. Wymieszaj, posyp fetą, dopiecz 10 min.",
            new[]{("Cukinia",120.0),("Bakłażan",100),("Pomidorki koktajlowe",100),("Ser feta",50),("Kasza bulgur",70),("Oliwa z oliwek",15),("Cebula czerwona",40)}, "Jednogarnkowe i zapiekanki"),

        new("Biryani z kurczakiem", 50, "mięso,drob",
            "1. Kurczaka zamarynuj w jogurcie z curry.|2. Podsmaż z cebulą i marchwią, dodaj rodzynki.|3. Wymieszaj z ryżem basmati, duś pod przykryciem 15 min.",
            new[]{("Ryż basmati",90.0),("Pierś z kurczaka",130),("Jogurt naturalny",40),("Cebula",50),("Przyprawa curry",4),("Rodzynki",20),("Marchew",40)}, "Jednogarnkowe i zapiekanki"),

        new("Fasolka szparagowa zapiekana z jajkiem", 35, "wege",
            "1. Fasolkę sparz, ułóż w naczyniu z cebulą i pomidorkami.|2. Zalej jajkami roztrzepanymi ze śmietaną.|3. Posyp serem, zapiekaj 20 min.",
            new[]{("Fasolka szparagowa (mrożona)",180.0),("Jajka",100),("Ser żółty gouda",40),("Pomidorki koktajlowe",80),("Cebula",40),("Śmietana 18%",40)}, "Jednogarnkowe i zapiekanki"),

        new("Ziemniaki zapiekane z boczkiem i porem", 55, "mięso,wieprzowina",
            "1. Ziemniaki pokrój w plastry.|2. Boczek i por podsmaż.|3. Układaj warstwami, zalej śmietaną z czosnkiem, posyp serem, zapiekaj.",
            new[]{("Ziemniaki",280.0),("Boczek wędzony",60),("Por",60),("Ser żółty gouda",40),("Śmietana 30%",50),("Czosnek",4)}, "Jednogarnkowe i zapiekanki"),

        // ---------------- SAŁATKI ----------------
        new("Sałatka grecka", 15, "wege",
            "1. Pomidory, ogórka i paprykę pokrój w kostkę.|2. Dodaj cebulę, oliwki i pokruszoną fetę.|3. Skrop oliwą, dopraw oregano.",
            new[]{("Pomidory świeże",120.0),("Ogórek świeży",100),("Papryka czerwona",60),("Cebula czerwona",30),("Ser feta",60),("Oliwki czarne",30),("Oliwa z oliwek",15)}, "Sałatki"),

        new("Sałatka gyros", 20, "mięso,drob",
            "1. Kurczaka usmaż w przyprawach, ostudź.|2. Poszatkuj sałatę, dodaj kukurydzę, ogórek i paprykę.|3. Wymieszaj z sosem z jogurtu, posyp serem.",
            new[]{("Pierś z kurczaka",120.0),("Sałata lodowa",60),("Kukurydza (puszka)",60),("Ogórek kiszony",50),("Papryka czerwona",50),("Jogurt naturalny",50),("Ser żółty gouda",30)}, "Sałatki"),

        new("Sałatka jarzynowa", 40, "wege",
            "1. Ziemniaki, marchew i jajka ugotuj, pokrój w kostkę.|2. Dodaj groszek i posiekany ogórek kiszony.|3. Wymieszaj z jogurtem i musztardą.",
            new[]{("Ziemniaki",150.0),("Marchew",60),("Groszek (puszka)",60),("Ogórek kiszony",60),("Jajka",50),("Jogurt naturalny",60),("Musztarda",10)}, "Sałatki"),

        new("Sałatka z tuńczykiem i fasolą", 15, "ryby",
            "1. Fasolę odsącz i przepłucz.|2. Wymieszaj z tuńczykiem, kukurydzą, papryką i cebulą.|3. Skrop oliwą, dopraw.",
            new[]{("Tuńczyk (puszka)",80.0),("Fasola czerwona (puszka)",120),("Kukurydza (puszka)",60),("Cebula czerwona",30),("Papryka czerwona",60),("Oliwa z oliwek",12)}, "Sałatki"),

        new("Sałatka caprese", 10, "wege",
            "1. Pomidory i mozzarellę pokrój w plastry.|2. Ułóż na przemian z rukolą.|3. Skrop oliwą, posyp bazylią.",
            new[]{("Pomidory świeże",150.0),("Ser mozzarella",100),("Bazylia suszona",2),("Oliwa z oliwek",15),("Rukola",30)}, "Sałatki"),

        new("Sałatka z rukolą i gruszką", 15, "wege",
            "1. Rukolę ułóż na talerzu.|2. Dodaj pokrojoną gruszkę, ser pleśniowy i orzechy.|3. Skrop oliwą i miodem.",
            new[]{("Rukola",60.0),("Gruszki",120),("Ser pleśniowy",50),("Orzechy włoskie",20),("Oliwa z oliwek",12),("Miód",10)}, "Sałatki"),

        new("Sałatka z buraka i fety", 20, "wege",
            "1. Buraki ugotuj i pokrój w kostkę.|2. Wymieszaj z fetą, cebulą i rukolą.|3. Posyp słonecznikiem, skrop oliwą.",
            new[]{("Buraki",180.0),("Ser feta",60),("Rukola",40),("Słonecznik łuskany",20),("Oliwa z oliwek",12),("Cebula czerwona",30)}, "Sałatki"),

        new("Sałatka bulgur z warzywami (tabbouleh)", 20, "wege",
            "1. Bulgur zalej wrzątkiem, ostudź.|2. Dodaj pomidory, ogórek, natkę i cebulę.|3. Skrop cytryną i oliwą.",
            new[]{("Kasza bulgur",70.0),("Pomidory świeże",100),("Ogórek świeży",80),("Natka pietruszki",10),("Cytryna",20),("Oliwa z oliwek",12),("Cebula czerwona",30)}, "Sałatki"),

        new("Sałatka z ciecierzycy", 15, "wege",
            "1. Ciecierzycę odsącz.|2. Dodaj pomidorki, ogórek, paprykę i cebulę.|3. Skrop oliwą, posyp natką.",
            new[]{("Ciecierzyca (puszka)",150.0),("Pomidorki koktajlowe",100),("Ogórek świeży",80),("Papryka czerwona",60),("Cebula czerwona",30),("Oliwa z oliwek",12),("Natka pietruszki",5)}, "Sałatki"),

        new("Sałatka cezar z kurczakiem", 25, "mięso,drob",
            "1. Kurczaka usmaż i pokrój.|2. Chleb podpiecz na grzanki.|3. Sałatę wymieszaj z pomidorkami, sosem z jogurtu i czosnku, posyp parmezanem.",
            new[]{("Pierś z kurczaka",120.0),("Sałata lodowa",60),("Pomidorki koktajlowe",80),("Chleb pszenny",50),("Jogurt grecki",50),("Czosnek",4),("Parmezan",20)}, "Sałatki"),

        new("Sałatka makaronowa z szynką", 25, "mięso,wieprzowina",
            "1. Makaron ugotuj i ostudź.|2. Dodaj paprykę, kukurydzę, ogórka i szynkę.|3. Wymieszaj z jogurtem i cebulą.",
            new[]{("Makaron penne",90.0),("Papryka czerwona",60),("Kukurydza (puszka)",60),("Ogórek świeży",70),("Szynka gotowana",60),("Jogurt naturalny",50),("Cebula czerwona",30)}, "Sałatki"),

        new("Surówka coleslaw", 15, "wege",
            "1. Kapustę białą i czerwoną poszatkuj.|2. Zetrzyj marchew.|3. Wymieszaj z sosem z jogurtu, musztardy i octu.",
            new[]{("Kapusta biała",150.0),("Marchew",70),("Kapusta czerwona",80),("Jogurt naturalny",50),("Musztarda",10),("Ocet jabłkowy",8)}, "Sałatki"),

        new("Sałatka z brokułem i słonecznikiem", 20, "wege",
            "1. Brokuł sparz i ostudź.|2. Wymieszaj z rodzynkami, słonecznikiem, cebulą i serem.|3. Polej sosem z jogurtu greckiego.",
            new[]{("Brokuł",180.0),("Rodzynki",20),("Słonecznik łuskany",20),("Jogurt grecki",50),("Cebula czerwona",30),("Ser żółty gouda",40)}, "Sałatki"),

        new("Sałatka ziemniaczana z jajkiem", 30, "wege",
            "1. Ziemniaki i jajka ugotuj, pokrój.|2. Dodaj cebulę i ogórka kiszonego.|3. Wymieszaj z jogurtem, musztardą i koperkiem.",
            new[]{("Ziemniaki",200.0),("Jajka",50),("Ogórek kiszony",50),("Cebula",40),("Jogurt naturalny",50),("Koperek",5),("Musztarda",10)}, "Sałatki"),

        new("Sałatka owocowa z jogurtem", 10, "wege,lubiane-przez-dzieci",
            "1. Jabłka, banany i gruszki pokrój w kostkę.|2. Zalej jogurtem z miodem.|3. Posyp orzechami.",
            new[]{("Jabłka",100.0),("Banany",100),("Gruszki",100),("Jogurt naturalny",80),("Miód",10),("Orzechy włoskie",15)}, "Sałatki"),
    };
}
