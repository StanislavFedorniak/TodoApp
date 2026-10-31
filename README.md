# TodoApp

[![CI](https://github.com/StanislavFedorniak/TodoApp/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/StanislavFedorniak/TodoApp/actions/workflows/ci.yml)

A full-stack task manager inspired by the UX of Microsoft To Do. It pairs a layered **ASP.NET Core Web API** with an **Angular** single-page client, and is built as a deliberate exercise in clean architecture, modern Angular (signals, standalone components) and accessibility.

> **Status:** active development. Core CRUD for tasks and categories works end to end, and the API is protected by JWT authentication with per-user data isolation. The Angular client does not have login yet, so it cannot call the protected endpoints; navigation, filtering and a details panel are on the roadmap below.

## Features

- Create, edit, complete and delete **tasks** (title, description, category)
- Create and delete **categories** and assign tasks to them
- Inline task editing with validation (whitespace-only titles are rejected)
- Delete confirmation and visible error states for failed requests
- Accessible UI: labelled form fields, descriptive `aria-label`s, `aria-live` error messages
- Swagger UI for exploring the API
- JWT authentication (register, login, account deletion) with per-user data isolation
- Rate limiting on login and account deletion against password brute-forcing

## Tech stack

| Layer | Technologies |
|---|---|
| Frontend | Angular 22, TypeScript (strict), RxJS, Signals, Reactive Forms, Bootstrap 5.3 |
| Backend | ASP.NET Core Web API (.NET 10), C# |
| Data | Entity Framework Core, SQL Server (LocalDB for development), EF migrations |
| API docs | Swagger / OpenAPI |

## Architecture

### Backend: N-tier with a dependency-free core

```
TodoApp.API        Thin controllers: HTTP in, DTOs out, status codes only
TodoApp.Services   Business logic (CategoryService, TodoTaskService, AuthService, AccountService)
TodoApp.Core       Entities, DTO records, service interfaces, exceptions, mapping extensions
TodoApp.Data       AppDbContext, entity configuration, migrations
```

- Controllers never touch `AppDbContext` and never return entities, only DTOs.
- DTOs are immutable `record`s; mapping lives in extension methods that have no database dependency.
- Reads use projections / `AsNoTracking`; updates and deletes use change tracking and return no body (`204 No Content`).
- Missing entities raise `NotFoundException` instead of returning `null` or `bool`.
- All I/O is `async` and passes a `CancellationToken`.

### Frontend: feature-oriented standalone Angular

```
todo-app-ui/src/app
├── core/        models, services, injection tokens
├── features/
│   ├── categories/   category list and creation form
│   └── tasks/        task list, task creation form, shared title validators
└── shared/      reusable components
```

- Standalone components with `input()` / `output()` and `inject()`; no NgModules.
- Local state in signals, derived data in `computed()`.
- Independent requests are combined with `forkJoin`; loading flags reset in `finalize`.
- Immutable state updates after each response instead of reloading whole lists.
- Native control flow (`@if`, `@for` with `track`).

## REST API

Base URL: `http://localhost:5293/api`

All endpoints except `/Auth/*` require a JWT in the `Authorization: Bearer <token>` header; without it they return `401`. See [Authentication](#authentication).

| Method | Endpoint | Description |
|---|---|---|
| POST | `/Auth/register` | Create an account, returns a token |
| POST | `/Auth/login` | Sign in, returns a token (rate limited) |
| POST | `/Account/delete` | Delete the account and all its data; requires the password (rate limited) |
| GET | `/Category` | List categories |
| GET | `/Category/{id}` | Get a category |
| POST | `/Category` | Create a category |
| PUT | `/Category/{id}` | Update a category |
| DELETE | `/Category/{id}` | Delete a category |
| GET | `/TodoTask` | List tasks; optional filters `categoryId`, `isCompleted`, `search` (title substring, up to 200 characters) |
| GET | `/TodoTask/{id}` | Get a task |
| POST | `/TodoTask` | Create a task |
| PUT | `/TodoTask/{id}` | Update a task |
| DELETE | `/TodoTask/{id}` | Delete a task |

Interactive documentation is available at `/swagger` when running in Development.

## Authentication

The API issues short-lived JWT access tokens (no refresh tokens yet). Passwords are hashed with ASP.NET Core's `PasswordHasher`. Every category and task belongs to a user: another user's record is never visible and responds with `404`, not `403`, so its existence is not revealed.

### Signing key

The signing key is a secret and must never be committed. `appsettings.json` only holds `Issuer`, `Audience` and `ExpirationMinutes`; set the key (at least 32 characters) locally with user-secrets:

```bash
dotnet user-secrets set "Jwt:Key" "<at least 32 random characters>" --project TodoApp.API
```

On a server use the environment variable `Jwt__Key`. The app refuses to start if the key is missing or too short.

### Example requests

```bash
# register (or POST /api/auth/login with the same body)
curl -X POST http://localhost:5293/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"me@example.com","password":"Passw0rd!123"}'
# -> {"token":"<jwt>","expiresAt":"..."}

# call a protected endpoint with the token
curl http://localhost:5293/api/category -H "Authorization: Bearer <jwt>"

# delete the account (wrong password -> 403)
curl -X POST http://localhost:5293/api/account/delete \
  -H "Authorization: Bearer <jwt>" -H "Content-Type: application/json" \
  -d '{"password":"Passw0rd!123"}'
```

In Swagger UI use **Authorize** and paste the token without the `Bearer` prefix.

### Rate limiting

Login (per client IP) and account deletion (per user) use fixed-window limits, `5` and `3` requests per `60` seconds by default. Exceeding them returns `429 Too Many Requests` with a `Retry-After` header. Limits are configured in the `RateLimiting` section of `appsettings.json` (`LoginPermitLimit`, `LoginWindowSeconds`, `AccountDeletePermitLimit`, `AccountDeleteWindowSeconds`).

> **Behind a reverse proxy:** the login limit is keyed by the connection's remote IP. Behind nginx, IIS or a load balancer every request appears to come from the proxy, so all users would share one limit. Before deploying behind a proxy, enable `UseForwardedHeaders` and trust `X-Forwarded-For` only from your own proxies (`KnownProxies` / `KnownNetworks`). Trusting it from everyone lets a client spoof the header and bypass the limit.

## Getting started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (current LTS) and npm
- SQL Server LocalDB (installed with Visual Studio) or another SQL Server instance
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Run the API

```bash
# apply migrations (creates the TodoAppDb database)
dotnet ef database update --project TodoApp.Data --startup-project TodoApp.API

# start the API on http://localhost:5293
dotnet run --project TodoApp.API --launch-profile http
```

The connection string lives in `TodoApp.API/appsettings.json` (`DefaultConnection`). CORS allows the Angular dev server at `http://localhost:4200`. Set the JWT signing key before the first run, see [Signing key](#signing-key).

### 2. Run the client

```bash
cd todo-app-ui
npm install
npm start
```

Open `http://localhost:4200`. The API base URL is configured in `todo-app-ui/src/app/app.config.ts`.

### Build and test

```bash
dotnet build TodoApp.sln
dotnet test TodoApp.sln
cd todo-app-ui && npm run build
```

The backend integration tests run against a real LocalDB database `TodoAppDb_Tests` with the same migrations as production, so they need SQL Server LocalDB.

## Roadmap

- [ ] App shell with a category sidebar and routing
- [ ] Shared category store (single source of truth for the sidebar and task list)
- [ ] Filtering by category via the URL, active / completed sections
- [ ] Toast notifications and an HTTP error interceptor
- [ ] Task details panel with managed focus
- [ ] Unit tests and automated accessibility (AXE) checks
- [ ] Authentication in the client (login and register pages, token interceptor, route guard); the API side is done
- [ ] Importance flag, due dates and smart lists (requires backend changes)

## Project structure

```
.
├── TodoApp.API/        ASP.NET Core Web API
├── TodoApp.Services/   Business logic
├── TodoApp.Core/       Domain model and contracts
├── TodoApp.Data/       EF Core context and migrations
├── TodoApp.Tests/      Integration tests (xUnit, WebApplicationFactory)
├── TodoApp.sln
└── todo-app-ui/        Angular client
```
