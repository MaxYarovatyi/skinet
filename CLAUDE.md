# CLAUDE.md

ASP.NET Core Web API (net6.0) + Angular client, Clean Architecture. See README.md.

## Commands
- `dotnet build` from the repo root; `dotnet run --project API`
- Client: `cd client && npm install && npx ng serve`
- Redis must be running on localhost:6379 (see README)

## Layout
- `Core`: entities and interfaces only — no references to Infrastructure or API
- `Infrastructure`: data access (EF Core SQLite, Redis repositories), seed JSON in `Infrastructure/Data/SeedData`
- `API`: controllers, DTOs, DI setup in `Program.cs` / extension classes
- `client`: Angular app

## Rules
- Keep dependencies pointing inward: API → Infrastructure → Core
- Controllers return DTOs, never entities
- Do not commit bin/, obj/, node_modules/ or local *.db files
- The JWT key in `appsettings.Development.json` is a development placeholder; real secrets go to user-secrets or environment variables
