# TaniTydzień 🛒

Planer tanich posiłków: wybierasz budżet i preferencje, dostajesz jadłospis obiadów na tydzień z gotową listą zakupów — ułożony pod aktualne promocje w Biedronce, Lidlu lub Auchan. Aplikacja porównuje też, w którym sklepie ten sam jadłospis wyjdzie najtaniej.

Ceny są **szacunkowe** (±10%) — nie obiecujemy cen co do grosza, obiecujemy jadłospis ułożony pod to, co jest tanie w tym tygodniu.

## Co już działa

- **Onboarding** — sklep, liczba osób, liczba obiadów, budżet tygodniowy, wykluczenia (np. wege, bez ryb).
- **Generator jadłospisu** — deterministyczny scoring w C# (koszt + premia za promocje + współdzielenie drogich składników + różnorodność białka). Bez LLM w locie — powtarzalny, tani, testowalny.
- **Wymiana dania** — „nie chcę tego" → podmiana z przeliczeniem kosztu.
- **Porównywarka sklepów** — ten sam jadłospis wyceniony w 3 sieciach, ranking od najtańszego.
- **Lista zakupów** — zagregowana ze wszystkich przepisów, pogrupowana po działach sklepu, z odhaczaniem i oznaczeniem promocji.
- **Widok przepisu** — składniki + kroki, tryb gotowania z dużą czcionką.

## Stack

| Warstwa | Technologia |
|---------|-------------|
| Backend (`/api`) | ASP.NET Core (.NET 8) + EF Core + SQLite |
| Frontend (`/web`) | Angular 19 (standalone components + signals), mobile-first |

## Uruchomienie (Windows)

Wymagania: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), [Node.js](https://nodejs.org/) (LTS).

Najprościej — dwuklik na **`start.bat`**: zwalnia porty, instaluje zależności (gdy brakuje) i odpala API + frontend w osobnych oknach.

Ręcznie:

```bash
# API → http://localhost:5080
cd api
dotnet run --urls http://localhost:5080

# Web → http://localhost:4200
cd web
npm install
npm start
```

Baza SQLite tworzy się i seeduje automatycznie przy pierwszym starcie API (19 przepisów, ~31 składników, ceny w 3 sieciach + promocje). Po zmianie danych w `SeedData` usuń pliki `api/tanitydzien.db*`, żeby wymusić reseed.

## API

| Endpoint | Opis |
|----------|------|
| `POST /api/menu/generate` | generuje jadłospis pod budżet i preferencje |
| `POST /api/menu/swap` | podmienia danie z przeliczeniem kosztu |
| `POST /api/menu/shopping-list` | lista zakupów zagregowana po działach |
| `POST /api/menu/compare` | wycena jadłospisu w każdej sieci + ranking |
| `GET /api/recipes`, `GET /api/recipes/{id}` | przeglądanie przepisów |

## Plany

Pełna wizja produktu, model biznesowy i roadmapa: [PLAN.md](PLAN.md). Najbliższe kroki: PWA (lista zakupów offline), Stripe + paywall, filtry makro (suwaki), worker parsujący gazetki promocyjne (LLM vision).
