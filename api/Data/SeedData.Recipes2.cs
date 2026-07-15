namespace TaniTydzien.Api.Data;

/// <summary>Dodatkowe przepisy, część 1: zupy, drób, wieprzowina, wołowina, ryby.</summary>
public static partial class SeedData
{
    private static List<RecDef> ExtraRecipes2() => new()
    {
        // ---------------- ZUPY ----------------
        new("Żurek na białej kiełbasie", 60, "mięso,wieprzowina",
            "1. Kiełbasę białą i boczek podsmaż, zalej wodą z majerankiem i czosnkiem.|2. Dodaj ziemniaki, gotuj 20 min.|3. Zakwaś octem, zaciągnij śmietaną.|4. Podawaj z połówką jajka.",
            new[]{("Kiełbasa biała surowa",90.0),("Boczek wędzony",30),("Ziemniaki",150),("Jajka",50),("Śmietana 18%",30),("Majeranek",1),("Czosnek",5),("Ocet jabłkowy",5)}, "Zupy"),

        new("Barszcz czerwony", 55, "wege",
            "1. Buraki, marchew, seler i por gotuj w wodzie z bulionem 40 min.|2. Odcedź, dopraw octem i czosnkiem.|3. Zaciągnij śmietaną, podawaj z ziemniakami.",
            new[]{("Buraki",200.0),("Marchew",50),("Seler korzeniowy",40),("Por",40),("Czosnek",5),("Ocet jabłkowy",6),("Bulion warzywny",6),("Śmietana 18%",20)}, "Zupy"),

        new("Barszcz ukraiński", 60, "wege",
            "1. Buraki, kapustę, marchew i ziemniaki gotuj w wodzie z bulionem.|2. Dodaj fasolę i koncentrat, gotuj 20 min.|3. Dopraw, podawaj ze śmietaną.",
            new[]{("Buraki",120.0),("Fasola czerwona (puszka)",100),("Kapusta biała",100),("Ziemniaki",120),("Marchew",40),("Koncentrat pomidorowy",15),("Bulion warzywny",6)}, "Zupy"),

        new("Grochówka z boczkiem", 70, "mięso,wieprzowina",
            "1. Groch namocz i ugotuj do miękkości.|2. Boczek, cebulę i marchew podsmaż, dodaj do grochu.|3. Wrzuć ziemniaki, gotuj 20 min, dopraw majerankiem.",
            new[]{("Groch łuskany",80.0),("Boczek wędzony",40),("Ziemniaki",120),("Marchew",50),("Cebula",40),("Majeranek",2),("Bulion warzywny",6)}, "Zupy"),

        new("Krem z dyni z imbirem", 40, "wege",
            "1. Dynię, marchew i cebulę podduś na oleju.|2. Zalej wodą z bulionem, gotuj 20 min.|3. Zblenduj z imbirem, dodaj śmietanę.",
            new[]{("Dynia",250.0),("Marchew",50),("Cebula",40),("Śmietana 30%",30),("Imbir",8),("Bulion warzywny",5),("Olej rzepakowy",8)}, "Zupy"),

        new("Krem z pieczarek", 35, "wege",
            "1. Pieczarki i cebulę podsmaż na maśle.|2. Dodaj ziemniaki i wodę z bulionem, gotuj 15 min.|3. Zblenduj, wmieszaj śmietanę, posyp natką.",
            new[]{("Pieczarki",200.0),("Ziemniaki",100),("Cebula",40),("Śmietana 18%",25),("Bulion warzywny",5),("Masło",12),("Natka pietruszki",5)}, "Zupy"),

        new("Rosół drobiowy z makaronem", 90, "mięso,drob,lubiane-przez-dzieci",
            "1. Udka zalej wodą, zbierz szumowiny.|2. Dodaj marchew, seler, por i przyprawy, gotuj 60 min.|3. Makaron ugotuj osobno, zalej rosołem, posyp natką.",
            new[]{("Udka z kurczaka",180.0),("Marchew",60),("Seler korzeniowy",40),("Por",40),("Makaron nitki",40),("Natka pietruszki",5),("Ziele angielskie",1),("Liść laurowy",1)}, "Zupy"),

        new("Zupa kalafiorowa", 40, "wege,lubiane-przez-dzieci",
            "1. Kalafiora podziel na różyczki.|2. Gotuj z ziemniakami i marchwią w wodzie z bulionem 20 min.|3. Zaciągnij śmietaną, dopraw koperkiem.",
            new[]{("Kalafior",200.0),("Ziemniaki",120),("Marchew",50),("Śmietana 18%",30),("Koperek",5),("Bulion warzywny",6),("Masło",10)}, "Zupy"),

        new("Kapuśniak z kiszonej kapusty", 55, "mięso,wieprzowina",
            "1. Kapustę kiszoną gotuj z liściem laurowym.|2. Boczek i cebulę podsmaż, dodaj do garnka.|3. Wrzuć ziemniaki i marchew, gotuj 20 min.",
            new[]{("Kapusta kiszona",180.0),("Ziemniaki",120),("Marchew",50),("Boczek wędzony",40),("Cebula",40),("Liść laurowy",1),("Bulion warzywny",6)}, "Zupy"),

        new("Zupa fasolowa", 50, "mięso,wieprzowina",
            "1. Boczek i cebulę podsmaż.|2. Dodaj fasolę z zalewą, ziemniaki, marchew i koncentrat.|3. Gotuj 25 min, dopraw majerankiem.",
            new[]{("Fasola biała (puszka)",200.0),("Ziemniaki",120),("Marchew",50),("Boczek wędzony",40),("Koncentrat pomidorowy",15),("Majeranek",2),("Bulion warzywny",6)}, "Zupy"),

        new("Zupa gulaszowa", 60, "mięso,wołowina",
            "1. Wołowinę obsmaż, dodaj cebulę i paprykę.|2. Wlej wodę z bulionem, dodaj koncentrat i paprykę mieloną.|3. Wrzuć ziemniaki, gotuj 30 min.",
            new[]{("Wołowina gulaszowa",120.0),("Papryka czerwona",70),("Ziemniaki",150),("Cebula",50),("Papryka słodka mielona",4),("Koncentrat pomidorowy",15),("Bulion warzywny",6)}, "Zupy"),

        new("Krem pomidorowy z bazylią", 30, "wege",
            "1. Cebulę i czosnek podsmaż.|2. Dodaj passatę i świeże pomidory, gotuj 15 min.|3. Zblenduj, wmieszaj śmietanę i bazylię.",
            new[]{("Passata pomidorowa",200.0),("Pomidory świeże",100),("Cebula",40),("Śmietana 30%",30),("Bazylia suszona",2),("Czosnek",5),("Bulion warzywny",5)}, "Zupy"),

        new("Zupa brokułowo-serowa", 35, "wege",
            "1. Brokuła, ziemniaki i cebulę gotuj w wodzie z bulionem 15 min.|2. Dodaj mleko i starty ser, mieszaj do rozpuszczenia.|3. Dopraw, podawaj z grzankami.",
            new[]{("Brokuł",180.0),("Ser żółty gouda",40),("Ziemniaki",120),("Cebula",30),("Mleko",100),("Bulion warzywny",5),("Masło",10)}, "Zupy"),

        new("Zupa z soczewicy z mlekiem kokosowym", 35, "wege",
            "1. Cebulę, imbir i kurkumę podsmaż.|2. Dodaj soczewicę, marchew i mleko kokosowe, wlej wodę.|3. Gotuj 20 min, zblenduj częściowo.",
            new[]{("Soczewica czerwona",70.0),("Mleko kokosowe",120),("Marchew",60),("Cebula",40),("Imbir",6),("Kurkuma",2),("Bulion warzywny",5)}, "Zupy"),

        new("Zupa cebulowa zapiekana", 45, "wege",
            "1. Cebulę zeszklij na maśle z tymiankiem.|2. Oprósz mąką, wlej wodę z bulionem, gotuj 20 min.|3. Rozlej do miseczek, połóż grzankę z chleba i ser, zapiecz.",
            new[]{("Cebula",250.0),("Ser żółty gouda",40),("Chleb pszenny",60),("Masło",15),("Mąka pszenna",15),("Tymianek",1),("Bulion warzywny",6)}, "Zupy"),

        new("Zupa ogórkowa z koperkiem", 40, "wege",
            "1. Ziemniaki i marchew ugotuj z bulionem.|2. Zetrzyj ogórki kiszone, dodaj do zupy z koperkiem.|3. Zaciągnij śmietaną 30%.",
            new[]{("Ogórek kiszony",120.0),("Ziemniaki",150),("Marchew",50),("Śmietana 30%",30),("Koperek",6),("Por",30),("Bulion warzywny",6)}, "Zupy"),

        new("Zupa z dyni i soczewicy", 40, "wege",
            "1. Dynię, cebulę i czosnek podduś.|2. Dodaj soczewicę i wlej wodę z bulionem.|3. Gotuj 20 min, dopraw kminem, zblenduj.",
            new[]{("Dynia",200.0),("Soczewica czerwona",60),("Cebula",40),("Czosnek",5),("Kmin rzymski",2),("Bulion warzywny",5),("Olej rzepakowy",8)}, "Zupy"),

        new("Barszcz biały z jajkiem", 45, "mięso,wieprzowina",
            "1. Kiełbasę białą i cebulę podsmaż, zalej wodą.|2. Zabiel śmietaną z mąką, dodaj chrzan i majeranek.|3. Wrzuć ziemniaki, gotuj 20 min, podawaj z jajkiem.",
            new[]{("Kiełbasa biała surowa",80.0),("Ziemniaki",150),("Jajka",50),("Śmietana 18%",30),("Chrzan tarty",10),("Mąka pszenna",10),("Majeranek",1),("Cebula",30)}, "Zupy"),

        // ---------------- DRÓB ----------------
        new("Kurczak w sosie słodko-kwaśnym", 35, "mięso,drob,lubiane-przez-dzieci",
            "1. Kurczaka pokrój, obsmaż i odłóż.|2. Podsmaż paprykę i cebulę, dodaj ananasa.|3. Wlej ketchup, ocet i wodę, zagęść, połącz z kurczakiem.|4. Podawaj z ryżem.",
            new[]{("Pierś z kurczaka",130.0),("Papryka czerwona",50),("Papryka zielona",50),("Ananas (puszka)",60),("Cebula",40),("Ketchup",25),("Ocet jabłkowy",8),("Ryż biały",70)}, "Drób"),

        new("Kurczak tikka masala", 40, "mięso,drob",
            "1. Kurczaka zamarynuj w jogurcie z curry.|2. Obsmaż, dodaj cebulę, czosnek i imbir.|3. Wlej passatę, duś 15 min.|4. Podawaj z ryżem basmati.",
            new[]{("Pierś z kurczaka",140.0),("Jogurt naturalny",40),("Passata pomidorowa",120),("Cebula",40),("Czosnek",5),("Imbir",5),("Przyprawa curry",4),("Ryż basmati",70)}, "Drób"),

        new("Udka pieczone w miodzie i musztardzie", 55, "mięso,drob,lubiane-przez-dzieci",
            "1. Wymieszaj miód, musztardę i czosnek.|2. Natrzyj udka i odstaw 15 min.|3. Piecz 45 min w 190°C razem z ziemniakami, posyp tymiankiem.",
            new[]{("Udka z kurczaka",220.0),("Miód",20),("Musztarda",20),("Czosnek",6),("Ziemniaki",250),("Olej rzepakowy",10),("Tymianek",1)}, "Drób"),

        new("Indyk pieczony z warzywami", 50, "mięso,drob",
            "1. Pierś z indyka natrzyj oliwą i tymiankiem.|2. Warzywa pokrój, wymieszaj z oliwą.|3. Piecz wszystko 40 min w 200°C.",
            new[]{("Pierś z indyka",160.0),("Marchew",60),("Cukinia",80),("Papryka czerwona",60),("Ziemniaki",200),("Oliwa z oliwek",15),("Tymianek",1)}, "Drób"),

        new("Nuggetsy z kurczaka", 30, "mięso,drob,lubiane-przez-dzieci",
            "1. Kurczaka pokrój w kawałki, oprósz mąką i papryką.|2. Maczaj w jajku i bułce tartej.|3. Smaż na złoto na oleju.",
            new[]{("Pierś z kurczaka",150.0),("Jajka",30),("Bułka tarta",50),("Mąka pszenna",20),("Olej rzepakowy",20),("Papryka słodka mielona",2)}, "Drób"),

        new("Kurczak w sosie śmietanowo-pieczarkowym", 35, "mięso,drob",
            "1. Kurczaka pokrój i obsmaż z cebulą.|2. Dodaj pieczarki i czosnek, smaż 5 min.|3. Wlej śmietanę, zagotuj.|4. Podawaj z makaronem.",
            new[]{("Pierś z kurczaka",130.0),("Pieczarki",120),("Śmietana 18%",40),("Cebula",40),("Makaron świderki",85),("Czosnek",4),("Masło",10)}, "Drób"),

        new("Wątróbka drobiowa z jabłkami", 25, "mięso,drob",
            "1. Cebulę i jabłka podsmaż na maśle, odłóż.|2. Wątróbkę oprósz i podsmaż z majerankiem.|3. Połącz z jabłkami, podawaj z pieczywem.",
            new[]{("Wątróbka drobiowa",150.0),("Jabłka",80),("Cebula",50),("Jajka",25),("Masło",15),("Majeranek",1)}, "Drób"),

        new("Curry z indyka z warzywami", 35, "mięso,drob",
            "1. Indyka pokrój i obsmaż z cebulą.|2. Dodaj paprykę, imbir i curry.|3. Wlej mleko kokosowe, duś 15 min.|4. Podawaj z ryżem basmati.",
            new[]{("Pierś z indyka",140.0),("Mleko kokosowe",120),("Papryka czerwona",60),("Cebula",40),("Przyprawa curry",4),("Imbir",6),("Ryż basmati",70)}, "Drób"),

        new("Kurczak z rożna z ziemniakami", 55, "mięso,drob,lubiane-przez-dzieci",
            "1. Udka natrzyj oliwą, czosnkiem i wędzoną papryką.|2. Ziemniaki pokrój, wymieszaj z tymiankiem.|3. Piecz razem 45 min w 200°C.",
            new[]{("Udka z kurczaka",230.0),("Ziemniaki",250),("Czosnek",6),("Papryka słodka mielona",3),("Wędzona papryka",3),("Oliwa z oliwek",15),("Tymianek",1)}, "Drób"),

        new("Gyros z kurczaka w tortilli", 30, "mięso,drob,lubiane-przez-dzieci",
            "1. Kurczaka pokrój, dopraw i usmaż.|2. Zawijaj w tortillę z sałatą, pomidorem i cebulą.|3. Polej sosem z jogurtu i czosnku.",
            new[]{("Pierś z kurczaka",130.0),("Tortilla pszenna",120),("Sałata lodowa",40),("Pomidory świeże",60),("Cebula czerwona",30),("Jogurt naturalny",50),("Czosnek",4)}, "Drób"),

        new("Fricassee z kurczaka", 40, "mięso,drob",
            "1. Kurczaka pokrój i podduś z marchwią.|2. Dodaj groszek, oprósz mąką.|3. Wlej bulion i śmietanę, gotuj 10 min.|4. Podawaj z ryżem.",
            new[]{("Pierś z kurczaka",120.0),("Marchew",50),("Groszek zielony (mrożony)",60),("Śmietana 18%",35),("Mąka pszenna",15),("Bulion drobiowy",4),("Ryż biały",70)}, "Drób"),

        new("Kurczak w sosie BBQ z batatami", 45, "mięso,drob",
            "1. Wymieszaj ketchup, miód, musztardę i wędzoną paprykę.|2. Natrzyj udka i piecz 40 min.|3. Bataty upiecz obok z czosnkiem.",
            new[]{("Udka z kurczaka",220.0),("Ketchup",30),("Miód",15),("Musztarda",10),("Wędzona papryka",3),("Czosnek",5),("Bataty",250)}, "Drób"),

        new("Kotlet de volaille", 50, "mięso,drob",
            "1. Pierś rozbij, w środek włóż masło z natką.|2. Zawiń, panieruj w mące, jajku i bułce.|3. Usmaż na złoto, podawaj z ziemniakami.",
            new[]{("Pierś z kurczaka",150.0),("Masło",20),("Jajka",30),("Bułka tarta",50),("Mąka pszenna",20),("Natka pietruszki",5),("Ziemniaki",250),("Olej rzepakowy",15)}, "Drób"),

        new("Sajgonki drobiowe (na patelni)", 35, "mięso,drob",
            "1. Kurczaka i warzywa pokrój w paski, podsmaż z imbirem.|2. Dopraw sosem sojowym.|3. Zawijaj w tortillę z kapustą pekińską.",
            new[]{("Pierś z kurczaka",120.0),("Kapusta pekińska",80),("Marchew",50),("Papryka zielona",50),("Imbir",5),("Sos sojowy",12),("Tortilla pszenna",120)}, "Drób"),

        new("Kurczak po tajsku z warzywami", 30, "mięso,drob",
            "1. Kurczaka obsmaż na dużym ogniu.|2. Dodaj brokuł, marchew i paprykę chili.|3. Wlej mleko kokosowe i sos sojowy, duś 8 min.|4. Podawaj z ryżem basmati.",
            new[]{("Pierś z kurczaka",130.0),("Brokuł",80),("Marchew",50),("Papryczka chili",5),("Mleko kokosowe",100),("Sos sojowy",12),("Ryż basmati",70)}, "Drób"),

        new("Pieczony kurczak z cytryną i czosnkiem", 55, "mięso,drob",
            "1. Kurczaka natrzyj oliwą, sokiem z cytryny i czosnkiem.|2. Piecz 45 min w 200°C.|3. Podawaj z kaszą bulgur i rukolą.",
            new[]{("Pierś z kurczaka",160.0),("Cytryna",30),("Czosnek",8),("Oliwa z oliwek",15),("Kasza bulgur",70),("Rukola",30),("Tymianek",1)}, "Drób"),

        new("Wrap z indykiem i warzywami", 20, "mięso,drob",
            "1. Indyka pokrój i podsmaż z papryką.|2. Tortillę posmaruj jogurtem.|3. Zawiń indyka z sałatą, ogórkiem i rukolą.",
            new[]{("Pierś z indyka",120.0),("Tortilla pszenna",120),("Sałata lodowa",30),("Ogórek świeży",50),("Rukola",20),("Jogurt naturalny",40),("Papryka czerwona",40)}, "Drób"),

        new("Kurczak zapiekany z brokułem i serem", 45, "mięso,drob",
            "1. Kurczaka i brokuł ułóż w naczyniu.|2. Zalej sosem ze śmietany i czosnku.|3. Posyp serem, zapiekaj 25 min w 200°C.",
            new[]{("Pierś z kurczaka",140.0),("Brokuł",120),("Ser żółty gouda",40),("Śmietana 30%",40),("Czosnek",5),("Bułka tarta",20),("Masło",10)}, "Drób"),

        // ---------------- WIEPRZOWINA ----------------
        new("Karkówka z grilla z ziemniakami", 40, "mięso,wieprzowina",
            "1. Karkówkę natrzyj musztardą, czosnkiem i papryką.|2. Grilluj lub usmaż z obu stron.|3. Podawaj z pieczonymi ziemniakami.",
            new[]{("Karkówka wieprzowa",160.0),("Cebula",40),("Czosnek",6),("Musztarda",15),("Olej rzepakowy",12),("Papryka słodka mielona",3),("Ziemniaki",250)}, "Wieprzowina"),

        new("Żeberka w sosie barbecue", 90, "mięso,wieprzowina",
            "1. Żeberka natrzyj wędzoną papryką, podpiecz.|2. Wymieszaj ketchup, miód, ocet i czosnek.|3. Polej żeberka, piecz pod folią 60 min.",
            new[]{("Żeberka wieprzowe",250.0),("Ketchup",40),("Miód",20),("Czosnek",6),("Wędzona papryka",4),("Ocet jabłkowy",8)}, "Wieprzowina"),

        new("Kotlet schabowy panierowany", 40, "mięso,wieprzowina,lubiane-przez-dzieci",
            "1. Schab rozbij, oprósz mąką.|2. Maczaj w jajku i bułce tartej.|3. Usmaż na złoto, podawaj z ziemniakami i kapustą.",
            new[]{("Schab wieprzowy",150.0),("Jajka",30),("Bułka tarta",50),("Mąka pszenna",20),("Olej rzepakowy",20),("Ziemniaki",250),("Kapusta biała",80)}, "Wieprzowina"),

        new("Boczek pieczony z kapustą kiszoną", 60, "mięso,wieprzowina",
            "1. Boczek pokrój i podsmaż z cebulą.|2. Dodaj kapustę kiszoną i kmin.|3. Duś 30 min, podawaj z ziemniakami.",
            new[]{("Boczek surowy",150.0),("Kapusta kiszona",180),("Cebula",50),("Kmin rzymski",2),("Ziemniaki",200),("Liść laurowy",1)}, "Wieprzowina"),

        new("Gulasz węgierski z karkówki", 60, "mięso,wieprzowina",
            "1. Karkówkę obsmaż, dodaj cebulę i paprykę.|2. Wsyp paprykę mieloną, wlej passatę i wodę.|3. Duś 40 min, podawaj z kaszą.",
            new[]{("Karkówka wieprzowa",140.0),("Papryka czerwona",70),("Cebula",50),("Papryka słodka mielona",5),("Passata pomidorowa",100),("Kasza gryczana",70),("Czosnek",5)}, "Wieprzowina"),

        new("Szynka zapiekana z ziemniakami", 50, "mięso,wieprzowina",
            "1. Ziemniaki pokrój w plastry.|2. Przełóż szynką i czosnkiem, zalej śmietaną z jajkiem.|3. Posyp serem, zapiekaj 30 min w 200°C.",
            new[]{("Szynka gotowana",120.0),("Ziemniaki",280),("Ser żółty gouda",40),("Śmietana 18%",50),("Czosnek",5),("Masło",10),("Jajka",30)}, "Wieprzowina"),

        new("Golonka duszona z warzywami", 100, "mięso,wieprzowina",
            "1. Golonkę obgotuj z liściem i zielem.|2. Dodaj marchew, cebulę i czosnek.|3. Duś 80 min, posmaruj musztardą i podpiecz.",
            new[]{("Golonka wieprzowa",250.0),("Cebula",50),("Czosnek",8),("Marchew",60),("Majeranek",2),("Liść laurowy",1),("Musztarda",15)}, "Wieprzowina"),

        new("Kiełbasa z cebulą i ziemniakami z patelni", 30, "mięso,wieprzowina",
            "1. Ziemniaki podgotuj i podsmaż.|2. Dodaj kiełbasę, cebulę i paprykę.|3. Dopraw wędzoną papryką, smaż do zrumienienia.",
            new[]{("Kiełbasa śląska",120.0),("Ziemniaki",250),("Cebula",50),("Papryka czerwona",50),("Olej rzepakowy",12),("Wędzona papryka",3)}, "Wieprzowina"),

        new("Parówki w cieście naleśnikowym", 25, "mięso,wieprzowina,lubiane-przez-dzieci",
            "1. Zrób ciasto z mąki, jajek i mleka.|2. Parówki maczaj w cieście.|3. Smaż na oleju, podawaj z ketchupem.",
            new[]{("Parówki",150.0),("Mąka pszenna",60),("Jajka",50),("Mleko",100),("Ketchup",30),("Olej rzepakowy",15)}, "Wieprzowina"),

        new("Sznycel z karkówki mielonej", 35, "mięso,wieprzowina",
            "1. Karkówkę drobno posiekaj, wymieszaj z jajkiem i musztardą.|2. Formuj sznycle, panieruj w bułce.|3. Usmaż, podawaj z kaszą jęczmienną.",
            new[]{("Karkówka wieprzowa",150.0),("Musztarda",10),("Jajka",25),("Bułka tarta",40),("Olej rzepakowy",18),("Kasza jęczmienna",70)}, "Wieprzowina"),

        new("Schab pieczony ze śliwką", 70, "mięso,wieprzowina",
            "1. W schabie zrób nacięcia, włóż śliwki suszone.|2. Natrzyj majerankiem i czosnkiem.|3. Piecz 60 min w 180°C, podawaj z ziemniakami.",
            new[]{("Schab wieprzowy",160.0),("Śliwki suszone",30),("Czosnek",6),("Majeranek",2),("Ziemniaki",250),("Olej rzepakowy",10)}, "Wieprzowina"),

        new("Boczek z soczewicą po wiejsku", 45, "mięso,wieprzowina",
            "1. Boczek i cebulę podsmaż.|2. Dodaj soczewicę, marchew i passatę.|3. Duś 25 min, dopraw tymiankiem.",
            new[]{("Boczek surowy",100.0),("Soczewica czerwona",70),("Marchew",50),("Cebula",40),("Passata pomidorowa",100),("Tymianek",1),("Bulion warzywny",4)}, "Wieprzowina"),

        new("Kiełbasa pieczona z kapustą i jabłkiem", 50, "mięso,wieprzowina",
            "1. Kapustę poszatkuj, wymieszaj z jabłkiem i kminem.|2. Ułóż z kiełbasą i cebulą w naczyniu.|3. Piecz 40 min w 190°C.",
            new[]{("Kiełbasa biała surowa",130.0),("Kapusta biała",180),("Jabłka",70),("Cebula",40),("Kmin rzymski",2),("Olej rzepakowy",10)}, "Wieprzowina"),

        new("Wieprzowina słodko-kwaśna", 40, "mięso,wieprzowina",
            "1. Schab pokrój w kostkę, obsmaż.|2. Dodaj paprykę, ananasa i cebulę.|3. Wlej sos z ketchupu, octu i sojowego.|4. Podawaj z ryżem.",
            new[]{("Schab wieprzowy",140.0),("Papryka czerwona",60),("Ananas (puszka)",60),("Cebula",40),("Ketchup",25),("Ocet jabłkowy",8),("Sos sojowy",10),("Ryż biały",70)}, "Wieprzowina"),

        // ---------------- WOŁOWINA ----------------
        new("Gulasz wołowy z kaszą", 90, "mięso,wołowina",
            "1. Wołowinę obsmaż, dodaj cebulę i marchew.|2. Dorzuć paprykę i koncentrat, wlej wodę.|3. Duś 60 min, podawaj z kaszą jęczmienną.",
            new[]{("Wołowina gulaszowa",150.0),("Cebula",50),("Marchew",50),("Papryka czerwona",60),("Koncentrat pomidorowy",15),("Papryka słodka mielona",4),("Kasza jęczmienna",70)}, "Wołowina"),

        new("Pieczeń wołowa w sosie własnym", 100, "mięso,wołowina",
            "1. Wołowinę obsmaż, dodaj cebulę, marchew i czosnek.|2. Posmaruj musztardą, wlej bulion.|3. Duś 80 min, podawaj z ziemniakami.",
            new[]{("Wołowina na stek",180.0),("Marchew",60),("Cebula",50),("Czosnek",6),("Musztarda",15),("Bulion drobiowy",5),("Ziemniaki",250)}, "Wołowina"),

        new("Stek wołowy z masłem czosnkowym", 25, "mięso,wołowina",
            "1. Stek dopraw, usmaż po 3 min z każdej strony.|2. Dodaj masło, czosnek i tymianek, polewaj.|3. Podawaj z pieczonymi ziemniakami i rukolą.",
            new[]{("Wołowina na stek",180.0),("Masło",20),("Czosnek",6),("Tymianek",1),("Ziemniaki",250),("Rukola",30)}, "Wołowina"),

        new("Klopsiki wołowe w sosie pomidorowym", 45, "mięso,wołowina,lubiane-przez-dzieci",
            "1. Wołowinę wymieszaj z jajkiem i bułką tartą, uformuj klopsiki.|2. Obsmaż, zalej passatą z czosnkiem i cebulą.|3. Duś 20 min, podawaj ze spaghetti.",
            new[]{("Wołowina mielona",130.0),("Jajka",20),("Bułka tarta",25),("Passata pomidorowa",150),("Cebula",40),("Czosnek",5),("Makaron spaghetti",85)}, "Wołowina"),

        new("Wołowina po burgundzku", 110, "mięso,wołowina",
            "1. Boczek i wołowinę obsmaż.|2. Dodaj cebulę, pieczarki i marchew.|3. Wlej bulion, duś 90 min, podawaj z ziemniakami.",
            new[]{("Wołowina gulaszowa",150.0),("Boczek wędzony",40),("Pieczarki",80),("Marchew",60),("Cebula",50),("Bulion drobiowy",5),("Ziemniaki",250)}, "Wołowina"),

        new("Burger wołowy z serem", 30, "mięso,wołowina,lubiane-przez-dzieci",
            "1. Uformuj kotlety z wołowiny, dopraw, usmaż.|2. Bułki przekrój, podsmaż.|3. Złóż z serem, sałatą, pomidorem, cebulą i ketchupem.",
            new[]{("Wołowina mielona",150.0),("Bułka pszenna",120),("Ser żółty gouda",30),("Sałata lodowa",30),("Pomidory świeże",50),("Cebula czerwona",30),("Ketchup",20)}, "Wołowina"),

        new("Chili z wołowiną i batatami", 50, "mięso,wołowina",
            "1. Wołowinę i cebulę podsmaż z kminem.|2. Dodaj bataty, fasolę czarną i passatę.|3. Duś 25 min z chili, podawaj z pieczywem.",
            new[]{("Wołowina mielona",130.0),("Fasola czarna (puszka)",120),("Bataty",150),("Passata pomidorowa",120),("Cebula",40),("Kmin rzymski",3),("Papryczka chili",5)}, "Wołowina"),

        new("Wołowina po strogonowsku", 50, "mięso,wołowina",
            "1. Wołowinę pokrój w paski, obsmaż.|2. Dodaj pieczarki, cebulę i ogórek kiszony.|3. Wlej śmietanę z koncentratem, duś 20 min.|4. Podawaj z kaszą.",
            new[]{("Wołowina gulaszowa",140.0),("Pieczarki",100),("Cebula",40),("Ogórek kiszony",40),("Śmietana 18%",40),("Koncentrat pomidorowy",10),("Kasza gryczana",70)}, "Wołowina"),

        new("Zrazy wołowe zawijane", 90, "mięso,wołowina",
            "1. Plastry wołowiny rozbij, posmaruj musztardą.|2. Zawiń z ogórkiem kiszonym i cebulą, obsmaż.|3. Duś w bulionie 70 min, podawaj z kaszą.",
            new[]{("Wołowina na stek",160.0),("Ogórek kiszony",50),("Cebula",40),("Musztarda",15),("Bulion drobiowy",5),("Mąka pszenna",10),("Kasza jęczmienna",70)}, "Wołowina"),

        new("Tacos z wołowiną", 30, "mięso,wołowina,lubiane-przez-dzieci",
            "1. Wołowinę podsmaż z cebulą, kminem i chili.|2. Nałóż na tortillę z kukurydzą i sałatą.|3. Dodaj pomidory i ser.",
            new[]{("Wołowina mielona",130.0),("Tortilla pszenna",120),("Kukurydza (puszka)",50),("Sałata lodowa",30),("Pomidory świeże",50),("Ser żółty gouda",30),("Kmin rzymski",2)}, "Wołowina"),

        // ---------------- RYBY ----------------
        new("Łosoś pieczony z cytryną", 30, "ryby",
            "1. Łososia skrop cytryną i oliwą, dopraw.|2. Piecz 18 min w 200°C z czosnkiem.|3. Podawaj z ziemniakami i brokułem.",
            new[]{("Łosoś (filet)",150.0),("Cytryna",30),("Oliwa z oliwek",12),("Czosnek",5),("Koperek",5),("Ziemniaki",250),("Brokuł",100)}, "Ryby"),

        new("Dorsz w cieście piwnym", 35, "ryby",
            "1. Rybę osusz i oprósz mąką.|2. Zrób ciasto z mąki, jajka i wody.|3. Maczaj i smaż na złoto, podawaj z cytryną.",
            new[]{("Filet z dorsza (mrożony)",160.0),("Mąka pszenna",50),("Jajka",30),("Bułka tarta",30),("Olej rzepakowy",25),("Cytryna",20)}, "Ryby"),

        new("Ryba po grecku", 50, "ryby",
            "1. Rybę usmaż, odłóż.|2. Zeszklij marchew, seler i cebulę, dodaj passatę i koncentrat.|3. Duś warzywa, przełóż na rybę.",
            new[]{("Filet z mintaja (mrożony)",150.0),("Marchew",80),("Seler korzeniowy",40),("Cebula",50),("Koncentrat pomidorowy",15),("Passata pomidorowa",80),("Olej rzepakowy",12)}, "Ryby"),

        new("Makrela pieczona z warzywami", 30, "ryby",
            "1. Makrelę rozłóż, skrop cytryną.|2. Ziemniaki i cebulę czerwoną upiecz z oliwą.|3. Podawaj z rukolą.",
            new[]{("Makrela wędzona",150.0),("Ziemniaki",250),("Cebula czerwona",40),("Cytryna",20),("Rukola",30),("Oliwa z oliwek",12)}, "Ryby"),

        new("Śledzie w śmietanie z jabłkiem", 20, "ryby",
            "1. Śledzie pokrój, wypłucz.|2. Wymieszaj śmietanę z cebulą, jabłkiem i ogórkiem.|3. Zalej śledzie, schłodź.|4. Podawaj z pieczywem.",
            new[]{("Śledź solony",120.0),("Śmietana 18%",50),("Cebula",40),("Jabłka",60),("Ogórek kiszony",40),("Koperek",4)}, "Ryby"),

        new("Krewetki w czosnku ze spaghetti", 25, "ryby",
            "1. Makaron ugotuj.|2. Krewetki podsmaż z czosnkiem, chili i oliwą.|3. Skrop cytryną, wymieszaj z makaronem i natką.",
            new[]{("Krewetki (mrożone)",120.0),("Makaron spaghetti",85),("Czosnek",8),("Oliwa z oliwek",15),("Papryczka chili",4),("Natka pietruszki",5),("Cytryna",20)}, "Ryby"),

        new("Paella z owocami morza", 45, "ryby",
            "1. Cebulę i paprykę podsmaż z kurkumą.|2. Wsyp ryż, wlej bulion, gotuj 15 min.|3. Dodaj krewetki i groszek, dorzuć passatę.",
            new[]{("Krewetki (mrożone)",120.0),("Ryż basmati",80),("Papryka czerwona",60),("Groszek zielony (mrożony)",60),("Cebula",40),("Passata pomidorowa",80),("Kurkuma",2)}, "Ryby"),

        new("Kotlety rybne", 40, "ryby",
            "1. Rybę rozdrobnij, wymieszaj z ugotowanym ziemniakiem, jajkiem i cebulą.|2. Formuj kotlety, panieruj w bułce.|3. Usmaż, posyp koperkiem.",
            new[]{("Filet z mintaja (mrożony)",150.0),("Ziemniaki",120),("Jajka",25),("Bułka tarta",40),("Cebula",30),("Koperek",4),("Olej rzepakowy",18)}, "Ryby"),

        new("Dorsz pod pierzynką warzywną", 45, "ryby",
            "1. Rybę ułóż w naczyniu, skrop cytryną.|2. Zetrzyj marchew i pokrój por, poddus, wymieszaj ze śmietaną.|3. Przykryj rybę, posyp serem, zapiekaj 25 min.",
            new[]{("Filet z dorsza (mrożony)",150.0),("Marchew",70),("Por",40),("Ser żółty gouda",40),("Śmietana 18%",40),("Cytryna",15),("Ziemniaki",200)}, "Ryby"),

        new("Łosoś w sosie koperkowym z kaszą", 30, "ryby",
            "1. Łososia usmaż, odłóż.|2. Zrób sos ze śmietany, koperku i cytryny.|3. Podawaj z kaszą bulgur i polej sosem.",
            new[]{("Łosoś (filet)",150.0),("Śmietana 30%",40),("Koperek",6),("Cytryna",15),("Kasza bulgur",70),("Masło",10),("Czosnek",4)}, "Ryby"),

        new("Tuńczyk zapiekany z makaronem", 40, "ryby",
            "1. Makaron ugotuj.|2. Wymieszaj z tuńczykiem, kukurydzą i passatą.|3. Przełóż do naczynia, posyp serem, zapiekaj 20 min.",
            new[]{("Tuńczyk (puszka)",80.0),("Makaron penne",90),("Kukurydza (puszka)",60),("Passata pomidorowa",120),("Ser żółty gouda",40),("Cebula",30)}, "Ryby"),

        new("Mintaj smażony z surówką z pekińskiej", 30, "ryby",
            "1. Rybę oprósz mąką i usmaż.|2. Kapustę pekińską poszatkuj z marchewką.|3. Dopraw jogurtem i cytryną, podawaj z ziemniakami.",
            new[]{("Filet z mintaja (mrożony)",150.0),("Kapusta pekińska",100),("Marchew",40),("Jogurt naturalny",40),("Cytryna",15),("Mąka pszenna",15),("Ziemniaki",200)}, "Ryby"),

        new("Ryba po marynarsku z ryżem", 35, "ryby",
            "1. Cebulę i paprykę podsmaż.|2. Dodaj dorsza, passatę i oliwki.|3. Duś 12 min, podawaj z ryżem.",
            new[]{("Filet z dorsza (mrożony)",150.0),("Papryka czerwona",50),("Cebula",40),("Passata pomidorowa",120),("Oliwki czarne",30),("Ryż biały",70),("Oliwa z oliwek",10)}, "Ryby"),

        new("Sałatka śledziowa z burakiem", 25, "ryby",
            "1. Buraki ugotuj i zetrzyj.|2. Wymieszaj ze śledziem, cebulą i ogórkiem.|3. Dopraw olejem, odstaw na godzinę.",
            new[]{("Śledź solony",100.0),("Buraki",120),("Cebula",40),("Ogórek kiszony",40),("Olej rzepakowy",12),("Natka pietruszki",4)}, "Ryby"),
    };
}
