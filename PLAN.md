# TaniTydzień — planer posiłków z listą zakupów pod Biedronkę/Lidl

> Pomysł #9: wybierasz budżet i preferencje, dostajesz jadłospis na tydzień z listą zakupów opartą o realne ceny i promocje z gazetek Biedronki/Lidla. Monetyzacja: subskrypcja + afiliacja.

---

## 1. Problem i grupa docelowa

**Problem:**
- Ceny żywności są stałym tematem — ludzie aktywnie szukają sposobów na tańsze gotowanie (ogromne zasięgi „tanie gotowanie", „obiad za 10 zł" na TikToku/YT).
- Planowanie posiłków pod budżet jest czasochłonne: trzeba przejrzeć gazetki, ułożyć przepisy pod promocje, spisać listę zakupów. Nikt tego nie robi systematycznie.
- Istniejące planery posiłków (Fitatu, Jadłospisy dietetyczne) optymalizują pod kalorie/makro, **nikt nie optymalizuje pod cenę w konkretnej sieci sklepów**. To jest luka.

**Grupa docelowa:**
- Podstawowa: osoby 25–45 gotujące dla rodziny/pary z ograniczonym budżetem, robiące zakupy głównie w Biedronce/Lidlu (czyli ~70% Polski).
- Wtórna: studenci, single optymalizujący wydatki, osoby spłacające kredyty szukające oszczędności.
- Insight: to nie jest produkt „dla biednych" — to produkt „dla sprytnych". Komunikacja: oszczędność jako zaradność, nie bieda.

**Propozycja wartości:**
„Jadłospis na cały tydzień i gotowa lista zakupów do Biedronki — ułożone pod aktualne promocje. Rodzina 2+2 je obiady za mniej niż 150 zł tygodniowo."

---

## 2. Kluczowe wyzwanie: dane o cenach

To jest serce produktu i główna bariera wejścia (dobra wiadomość: dla konkurencji też).

**Źródła danych, od najprostszego:**
1. **Gazetki promocyjne (start):** Biedronka i Lidl publikują gazetki co tydzień. Parsowanie: pobranie PDF/obrazów → ekstrakcja LLM-em z vision (produkt, cena, warunki promocji typu „przy zakupie 2"). Koszt: grosze za gazetkę, aktualizacja 2×/tydz.
2. **Ceny bazowe (produkty niepromocyjne):** ręcznie utrzymywany koszyk ~300 produktów bazowych (mąka, jajka, kurczak, ziemniaki…) aktualizowany raz w miesiącu + korekty od użytkowników („zgłoś inną cenę" — crowdsourcing jak Waze).
3. **Aplikacje sieci (później, ostrożnie):** Biedronka ma ceny w aplikacji — scraping ryzykowny prawnie i technicznie; traktować jako v3, nie fundament.

**Decyzja projektowa:** ceny są **szacunkowe** i tak komunikowane („szacunkowy koszt koszyka ±10%"). Nie obiecujemy cen co do grosza — obiecujemy, że jadłospis jest ułożony pod to, co jest tanie w tym tygodniu.

**Baza przepisów:**
- Start: 150–250 własnych przepisów (można wygenerować LLM-em i zredagować + zdjęcia własne/stock). Każdy przepis rozpisany na znormalizowane składniki z gramaturą → mapowanie na produkty sklepowe.
- Tagowanie: czas przygotowania, dieta (wege, bez laktozy…), „lubiane przez dzieci", sprzęt (piekarnik/patelnia).
- To jest własność intelektualna produktu — przepisy + mapowanie składnik→produkt→cena.

---

## 3. Zakres MVP (6–8 tygodni)

### Musi być (v1)
1. **Onboarding:** liczba osób, budżet tygodniowy, sklep (Biedronka/Lidl), wykluczenia (bez ryb, wege…), ile obiadów gotujesz w tygodniu.
2. **Generator jadłospisu:** 5–7 obiadów na tydzień dopasowanych do budżetu i aktualnych promocji. Algorytm: scoring przepisów po (koszt z uwzgl. promocji, różnorodność, preferencje, wykorzystanie wspólnych składników — kupujesz kapustę, to dwa przepisy z kapustą).
3. **Wymiana dania:** „nie chcę tego" → podmiana z przeliczeniem kosztu i listy.
4. **Lista zakupów:** zagregowana ze wszystkich przepisów, pogrupowana po działach sklepu, z cenami szacunkowymi i oznaczeniem promocji („w gazetce do niedzieli"). Odhaczanie w sklepie (PWA, działa offline).
5. **Widok przepisu:** składniki + kroki, tryb gotowania (duża czcionka, ekran nie gaśnie).
6. **Subskrypcja:** darmowy 1 jadłospis na próbę, potem paywall.

### Świadomie POZA MVP
- Śniadania/kolacje (v1.1 — obiady to 80% problemu i kosztu).
- Kalorie i makro z suwakami (v1.1 — opisane szczegółowo w sekcji „Filtrowanie po makro"; dane makro zbierać w bazie przepisów od pierwszego dnia, ale UI i filtry dopiero po starcie).
- Inne sieci (Aldi, Kaufland, Auchan — v2, po dowiezieniu dwóch pierwszych).
- Spiżarnia/„co mam w lodówce" (ładne, ale odkłada start o miesiąc).
- Aplikacje natywne — PWA wystarczy na rok.

---

## 3a. Filtrowanie po makro — suwaki (v1.1)

**Idea:** każdy posiłek ma rozpisane makroskładniki (białko, węglowodany, tłuszcze + kalorie na porcję), a użytkownik ustawia **suwaki** określające, jakich posiłków szuka. Generator i przeglądarka przepisów pokazują tylko dania mieszczące się w ustawionych zakresach.

### Skąd wziąć dane makro
- Makro liczone **automatycznie ze składników**, nie wpisywane ręcznie per przepis: tabela `ingredients` dostaje wartości odżywcze na 100 g (białko, węgle, tłuszcze, kcal) z otwartych baz (np. Otwarte Dane / USDA FoodData Central + ręczna weryfikacja polskich produktów), a makro przepisu = suma po `recipe_ingredients` podzielona przez liczbę porcji.
- Zaleta tego podejścia: przy wymianie/skalowaniu składników makro przelicza się samo, a dodanie nowego przepisu nie wymaga żadnej dodatkowej pracy.
- Wartości komunikowane jako przybliżone (±10%) — spójnie z filozofią cen szacunkowych.
- **Decyzja na teraz:** kolumny makro w bazie i wyliczanie od pierwszego dnia (koszt bliski zera), UI z suwakami dopiero w v1.1 — start nie może się przez to opóźnić.

### UX suwaków
- Ekran filtrów (dostępny z przeglądarki przepisów i z ustawień generatora) z czterema suwakami zakresowymi (dwa uchwyty: min–max):
  - **Białko** na porcję: 0–60 g
  - **Węglowodany** na porcję: 0–120 g
  - **Tłuszcze** na porcję: 0–50 g
  - **Kalorie** na porcję: 200–1200 kcal
- Pod suwakami na żywo licznik: „**43 przepisy** pasują do tych ustawień" — natychmiastowy feedback zapobiega frustracji ustawieniem zakresów, do których nic nie pasuje; przy < 5 wynikach podpowiedź, który suwak najbardziej ogranicza pulę („poluzuj białko o 5 g → +12 przepisów").
- Gotowe presety zamiast ręcznego ustawiania (większość osób nie zna swoich liczb): **„Więcej białka"** (≥30 g białka), **„Mniej węgli"** (≤40 g węgli), **„Lekkie"** (≤500 kcal), **„Bez filtra"**. Preset ustawia suwaki, które można potem doszlifować.
- Ustawienia zapisywane w profilu użytkownika — generator jadłospisu respektuje je przy każdym kolejnym tygodniu.

### Wpływ na generator jadłospisu
- Filtr makro działa jako **twarde ograniczenie puli** przepisów przed scoringiem cenowym (najpierw odsiew po makro, potem optymalizacja kosztu i różnorodności).
- Konflikt budżet × makro trzeba obsłużyć jawnie: wysokie białko + niski budżet może zostawić pustą lub drogą pulę. Wtedy komunikat z wyborem: „Przy tych makro tydzień wyjdzie ~172 zł (budżet: 150 zł). Zwiększyć budżet czy poluzować makro?" — nigdy cicho ignorować któregoś z ustawień.
- Na karcie dania i w podsumowaniu tygodnia pokazywać makro (na porcję i średnio dziennie) — to domyka wartość dla osób „fit" bez robienia z aplikacji pełnego kalkulatora diety.

### Wpływ na produkt i biznes
- Otwiera drugą grupę docelową: osoby trenujące / na redukcji, które dziś używają Fitatu, ale nikt nie łączy im makro z ceną („redukcja za 150 zł tygodniowo" to gotowy content na TikToka).
- Naturalny kandydat na feature premium: suwaki i presety tylko w planie płatnym (free ma jadłospis bez kontroli makro) — wzmacnia paywall bez zabierania podstawowej wartości.

---

## 4. Architektura i stack

- **Frontend:** Angular (standalone components) + Tailwind, PWA (`@angular/pwa` — manifest + service worker; lista zakupów musi działać w sklepie przy słabym zasięgu).
- **Backend:** ASP.NET Core Web API (.NET 8, C#) + Entity Framework Core + PostgreSQL. Osobny worker (BackgroundService / hostowany serwis lub oddzielny proces console) do przetwarzania gazetek.
- **Pipeline gazetek:** zadanie w tle (środa/czwartek — nowe gazetki) → pobranie → LLM vision (Claude) ekstrahuje pozycje do JSON → walidacja (ceny odstające do ręcznego review) → zapis do `promotions`.
- **Generator jadłospisu:** deterministyczny algorytm w C# (scoring + dobór zachłanny z ograniczeniem budżetu), NIE generowanie LLM-em w locie — powtarzalność, koszt zero, łatwe testy jednostkowe. LLM tylko offline do produkcji przepisów.
- **Płatności:** Stripe (Stripe.net); BLIK przez Przelewy24 od v1.1 (dla tej grupy docelowej BLIK istotnie podnosi konwersję).

**Struktura repo (monorepo):**
```
/api        — ASP.NET Core Web API (.NET 8)  → domyślnie http://localhost:5080
/web        — Angular + Tailwind (PWA)        → domyślnie http://localhost:4200
/worker     — konsola .NET (pipeline gazetek)
start.bat   — jeden klik: sprzątanie portów + install + uruchomienie (patrz 4a)
```

**Model danych (rdzeń):**
```
recipes (id, nazwa, kroki, czas_min, tagi[], porcje_bazowe,
         -- makro wyliczane ze składników, cache per porcja:
         bialko_g, wegle_g, tluszcze_g, kcal)
ingredients (id, nazwa_znormalizowana, jednostka,
             bialko_100g, wegle_100g, tluszcze_100g, kcal_100g)
recipe_ingredients (recipe_id, ingredient_id, ilosc_na_porcje)
products (id, sklep, nazwa, ingredient_id, cena_bazowa, gramatura)
promotions (id, product_id, cena_promo, warunek, od, do, zrodlo_gazetka)
users (id, osoby, budzet, sklep, wykluczenia[], plan,
       makro_filtry jsonb)  -- zakresy suwaków: {bialko:[min,max], wegle:[...], ...}
menus (id, user_id, tydzien, przepisy[], koszt_szacowany)
price_reports (user_id, product_id, cena_zgloszona, data)  -- crowdsourcing
```

---

## 4a. Uruchamianie lokalne — `start.bat`

Jeden skrypt w katalogu głównym uruchamiany dwuklikiem (Windows). Kolejność działań:

1. **Zabija procesy trzymające porty** aplikacji (`5080` — API, `4200` — Angular). Znajduje PID po porcie (`netstat -ano` / `findstr`) i wykonuje `taskkill /F` — bez tego restart pada na „port already in use".
2. **Instaluje zależności w razie potrzeby** — `dotnet restore` dla API/workera oraz `npm install` w `/web` tylko gdy brakuje `node_modules` (szybki restart, gdy już zainstalowane).
3. **Odpala aplikacje** — API (`dotnet run`) i frontend (`ng serve` / `npm start`) w osobnych oknach, żeby logi obu były widoczne.

Gotowy plik `start.bat` leży w katalogu głównym repo (obok tego planu).

---

## 5. Model biznesowy

| Plan | Cena | Zawartość |
|------|------|-----------|
| Free | 0 zł | 1 pełny jadłospis (jednorazowo) + podgląd bieżącego tygodnia bez listy zakupów |
| Premium | **14,99 zł/mies.** lub **99 zł/rok** | pełne jadłospisy co tydzień, lista zakupów, wymiany dań, historia |
| Rodzinny (v2) | 19,99 zł/mies. | 2 konta, śniadania+kolacje, eksport PDF |

- **Pozycjonowanie cenowe:** „aplikacja oszczędza ci 100–200 zł miesięcznie i kosztuje 15 zł" — komunikować oszczędność w zł na dashboardzie („ten jadłospis: 148 zł, analogiczny koszyk bez promocji: 189 zł").
- **Afiliacja (drugi filar):** programy partnerskie sprzętu AGD, dostawcy pudełek jako alternatywa („nie masz czasu gotować w tym tygodniu?"), Allegro (garnki, pojemniki). Realnie: dodatek, nie fundament — planować <20% przychodu.
- **Ekonomia:** koszty LLM na użytkownika bliskie zera (generator deterministyczny), infrastruktura płaska. Marża brutto >90%. Break-even hobby→biznes: ~400 subskrybentów = ~5 000 zł MRR.

---

## 6. Go-to-market

**To jest produkt konsumencki → wygrywa dystrybucją treści, nie reklamą.**

**Faza 0 — walidacja przed kodem (2 tygodnie):**
- Zrób ręcznie to, co ma robić apka: ułóż jadłospis tygodnia pod gazetkę Biedronki, opublikuj jako post/rolkę („tydzień obiadów dla rodziny 2+2 za 140 zł — wszystko z promocji Biedronki"). Powtórz 2–3 tygodnie.
- Jeśli to nie łapie zasięgów organicznie — produkt też nie złapie. Jeśli łapie: masz walidację i gotowy kanał marketingowy w dniu startu.

**Faza 1 — start (miesiąc 1–3):**
- TikTok/Instagram/YT Shorts: cotygodniowa rolka „jadłospis tygodnia z promocji" (generowana przez własną apkę — produkt sam produkuje content marketingowy, to jest kluczowa pętla wzrostu).
- Grupy FB: „Tanie gotowanie", „Gotowanie dla rodziny", grupy oszczędnościowe (setki tysięcy członków).
- Współprace z mikro-twórcami „budżetowe gotowanie" (barter/afiliacja 30% z pierwszego roku subskrypcji).

**Faza 2 — skala:**
- SEO: strony „jadłospis na tydzień za X zł", „co ugotować z promocji Biedronki [data]" — świeży content co tydzień automatycznie z bazy = silne SEO.
- Newsletter czwartkowy (nowe gazetki) — darmowy skrót + pełny jadłospis dla premium; newsletter sam w sobie jest produktem retencyjnym.

**Metryki:** aktywacja = wygenerowany jadłospis + otwarta lista zakupów; retencja tygodniowa (produkt ma naturalny tygodniowy rytm — to rzadka zaleta); konwersja free→paid ≥ 5%; churn < 8%/mies. (konsumencki będzie wyższy niż B2B — kompensować planem rocznym).

---

## 7. Harmonogram (solo developer)

| Tydzień | Zakres |
|---------|--------|
| 1–2 | Baza przepisów (redakcja 150 szt.), model danych, normalizacja składników + wartości odżywcze na 100 g (pod przyszłe suwaki makro) |
| 3 | Pipeline gazetek (LLM vision → promotions) + koszyk cen bazowych |
| 4 | Algorytm generatora jadłospisów + testy na realnych gazetkach |
| 5 | UI: onboarding, jadłospis, wymiana dań, widok przepisu |
| 6 | Lista zakupów (PWA offline, odhaczanie), agregacja składników |
| 7 | Stripe + paywall, landing, dashboard oszczędności |
| 8 | Beta z 20–30 osobami z grup FB, poprawki mapowania cen |

**Definicja "gotowe":** użytkownik przechodzi od onboardingu do listy zakupów w < 3 min, koszt koszyka trafia w ±15% rzeczywistego paragonu (sprawdzone na 5 realnych zakupach).

---

## 8. Ryzyka i mitigacje

| Ryzyko | Ocena | Mitigacja |
|--------|-------|-----------|
| Jakość ekstrakcji cen z gazetek | średnie | Walidacja + ręczny review odstających; ceny komunikowane jako szacunkowe; crowdsourcing korekt |
| Prawne: użycie danych z gazetek | niskie/średnie | Ceny jako fakty nie podlegają ochronie autorskiej; nie kopiować grafik gazetek; monitorować, w razie czego przejść na dane crowdsourcingowe |
| Churn konsumencki po nowości | wysokie | Tygodniowy rytm produktu + newsletter + dashboard „zaoszczędziłeś łącznie X zł" (sunk value); push przed weekendowymi zakupami |
| Biedronka/Lidl zrobią to same | niskie | Sieci nie mają motywacji optymalizować budżetu klienta między kategoriami; my jesteśmy po stronie klienta i multi-sklepowi |
| Sezonowość treści (przepisy się nudzą) | średnie | Stały dopływ przepisów (produkcja LLM + redakcja = tania), sezonowe kolekcje |

---

## 9. Roadmapa po MVP

1. Śniadania i kolacje (plan Rodzinny — podniesienie ARPU).
2. Kolejne sieci: Aldi, Kaufland, Dino → porównanie „w którym sklepie ten jadłospis wyjdzie taniej" (mocny feature PR-owy).
3. „Spiżarnia" — odejmij od listy to, co już masz.
4. Suwaki makro (sekcja 3a) — pierwszy duży feature po starcie (v1.1): filtry min–max białko/węgle/tłuszcze/kcal z presetami, wpięte w generator; otwiera grupę „fit" i wzmacnia paywall.
5. B2B2C: white-label dla banków/kas oszczędnościowych (programy oszczędnościowe dla klientów) — spekulatywne, ale wysokomarżowe.

---

## 10. Status realizacji (dziennik)

> Aktualizowane na bieżąco. Ostatnia aktualizacja: **2026-07-11**.

### ✅ Zrobione

**Backend — `/api` (ASP.NET Core .NET 8 + EF Core + SQLite)**
- Model danych (Entities) + `SeedData` z **51 przepisami**, ~59 składnikami (makro/100 g, działy sklepu) i promocjami tygodnia.
- Generator jadłospisu — deterministyczny scoring (`Services/MenuGenerator.cs`): koszt + premia za promocje + współdzielenie drogich składników + różnorodność białka. Bez LLM w locie.
- Pricing + lista zakupów (`PricingService`, `ShoppingListService`) — agregacja po działach, ceny szacunkowe z promocjami, oszczędności.
- Endpointy: `POST /api/menu/generate`, `/swap`, `/shopping-list`, `/compare`, `/pool-count`, `GET /api/recipes`, `/recipes/{id}`.
- **3 sieci: Biedronka / Lidl / Auchan** — każdy składnik ma cenę w każdej sieci + własne promocje.
- **Porównywarka sklepów** (`/api/menu/compare`) — wycenia ten sam jadłospis w każdej sieci, ranking od najtańszego + `maxSaving`.
- **Walidacja żądań** — `[Range]`/`[Required]` na DTO z polskimi komunikatami; błędy zawsze w formacie `{ message }`.
- **Filtry makro (v1.1, sekcja 3a)** — rekord `MacroFilters` (min–max białko/węgle/tłuszcze/kcal), twarde ograniczenie puli przed scoringiem, jawny komunikat konfliktu budżet × makro, licznik `/pool-count`.
- **Testy jednostkowe** — `api.Tests` (xUnit): 14 testów generatora (scoring, promocje, wykluczenia, filtry makro, determinizm).

**Frontend — `/web` (Angular 19 standalone + signals, własny CSS, mobile-first)**
- Onboarding (sklep, osoby, obiady, budżet, wykluczenia) → generowanie jadłospisu.
- **Filtry makro w onboardingu** — presety („Więcej białka", „Mniej węgli", „Lekkie") + 4 zakresy suwaków + licznik na żywo „X przepisów pasuje".
- Ekran jadłospisu: karty dań, banner oszczędności, ostrzeżenie o budżecie, makro dziennie, wymiana dania.
- **Ekran porównywarki `/compare`**: ranking 3 sklepów, najtańszy podświetlony, wybór koszyka → lista zakupów pod wybrany sklep.
- Lista zakupów (grupy po działach, odhaczanie, suma + oszczędności na promocjach) — **z fallbackiem offline** (ostatnia pobrana lista z localStorage przy braku sieci).
- Widok przepisu (składniki, kroki, tryb gotowania z dużą czcionką) — **Wake Lock**: ekran nie gaśnie w trybie gotowania, odnawiany po powrocie do karty.
- **PWA** — `@angular/pwa`: manifest (nazwa, kolory), service worker, ikony.
- **Persystencja stanu** — plan w localStorage (refresh nie gubi jadłospisu); wspólne helpery `core/storage.ts`.
- **Historia jadłospisów** — `/history` (na urządzeniu, maks. 20 wpisów): przywracanie, usuwanie.
- **Obsługa błędów** — timeout HTTP 15 s (interceptor), wspólny `apiErrorMessage` (w tym komunikat dla timeoutu).

**Jakość**
- Przegląd kodu (multi-agent, 2026-07-11) — naprawione m.in.: wyścig Wake Lock, crash przy zablokowanym localStorage, maskowanie błędów serwera przez cache offline, przesunięcie indeksów przy swap z nieaktualnymi id, niespójna semantyka filtra kcal.

### ⏭️ Następne kroki (kolejność realizacji)

1. **Stripe + paywall** (`Stripe.net`): free = 1 jadłospis na próbę, potem premium. Landing + dashboard „zaoszczędziłeś X zł". **Bloker:** konto Stripe (klucze API) + decyzja o modelu kont użytkowników (auth). ← następne
2. **Worker gazetek** (`/worker`, konsola .NET): pobranie gazetki → LLM vision (Claude) → ekstrakcja pozycji → walidacja → zapis do `promotions`. **Bloker:** klucz API Anthropic + wybór źródła gazetek.
3. Dalsza rozbudowa bazy przepisów (51 → 150–250, wg sekcji 2).
4. Kolejne sieci (Aldi, Kaufland, Dino), spiżarnia, plan Rodzinny (śniadania/kolacje) — wg roadmapy w sekcji 9.

### ⚠️ Uwagi techniczne dla kontynuacji
- **Reseed bazy:** po zmianie danych w `SeedData` usuń `api/tanitydzien.db*` — `EnsureCreated`+`Seed` nie reseedują istniejącej bazy (`Seed` bailuje, gdy `Recipes.Any()`).
- **Angular control flow:** alias `as` nie działa na `@else if` — zagnieżdżaj `@if (x; as y)` wewnątrz `@else`.
- **Walidacja na rekordach pozycyjnych C#:** atrybuty (`[Range]` itd.) muszą być na parametrach konstruktora, NIE `[property: ...]` — inaczej MVC rzuca `InvalidOperationException` w runtime.
- **Testy ręczne API w PowerShell:** body z polskimi znakami wysyłaj jako bajty UTF-8 (`[Text.Encoding]::UTF8.GetBytes(...)` + `charset=utf-8`), inaczej wykluczenia typu „mięso" cicho nie zadziałają.
- Uruchomienie całości: `start.bat` (porty: API `5080`, web `4200`).
- Testy: `dotnet test` w `api.Tests/`.
