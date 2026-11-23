# Skinet

Early learning project (2023): an e-commerce store built while studying full-stack development with ASP.NET Core and Angular.

## Features
- Product catalogue with filtering by brand and type, sorting, search and paging
- Shopping basket stored in Redis
- Registration, login and JWT authentication (ASP.NET Core Identity)
- Checkout with delivery methods and order history
- Centralised API error handling

## Architecture
| Project | Role |
|---|---|
| `Core` | Entities (products, basket, order aggregate), interfaces, Specification pattern |
| `Infrastructure` | EF Core (SQLite) with a generic repository, Redis basket repository, Identity store, seed data |
| `API` | ASP.NET Core Web API, AutoMapper DTOs, error middleware |
| `client` | Angular 12 front end (Bootstrap / Bootswatch) |

## Running
Requires the .NET 6 SDK, Docker and Node.js.
```
docker compose up -d                 # Redis on :6379, Redis Commander on :8081
dotnet run --project API             # https://localhost:5001
npm install                          # root packages used by the client
cd client && npm install && npx ng serve
```
The client runs over HTTPS with the self-signed development certificate in `client/ssl/`.

## Tech stack
C#, .NET 6, ASP.NET Core Web API, EF Core (SQLite), Redis, AutoMapper, Angular 12, TypeScript, Docker Compose.
