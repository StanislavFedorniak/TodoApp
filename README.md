# TodoApp

A full-stack task manager inspired by the UX of Microsoft To Do. It pairs a layered **ASP.NET Core Web API** with an **Angular** single-page client, and is built as a deliberate exercise in clean architecture, modern Angular (signals, standalone components) and accessibility.

> **Status:** active development. Core CRUD for tasks and categories works end to end; navigation, filtering and a details panel are on the roadmap below.

## Features

- Create, edit, complete and delete **tasks** (title, description, category)
- Create and delete **categories** and assign tasks to them
- Inline task editing with validation (whitespace-only titles are rejected)
- Delete confirmation and visible error states for failed requests
- Accessible UI: labelled form fields, descriptive `aria-label`s, `aria-live` error messages
- Swagger UI for exploring the API

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
TodoApp.Services   Business logic (CategoryService, TodoTaskService)
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

| Method | Endpoint | Description |
|---|---|---|
| GET | `/Category` | List categories |
| GET | `/Category/{id}` | Get a category |
| POST | `/Category` | Create a category |
| PUT | `/Category/{id}` | Update a category |
| DELETE | `/Category/{id}` | Delete a category |
| GET | `/TodoTask` | List tasks |
| GET | `/TodoTask/{id}` | Get a task |
| POST | `/TodoTask` | Create a task |
| PUT | `/TodoTask/{id}` | Update a task |
| DELETE | `/TodoTask/{id}` | Delete a task |

Interactive documentation is available at `/swagger` when running in Development.

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

The connection string lives in `TodoApp.API/appsettings.json` (`DefaultConnection`). CORS allows the Angular dev server at `http://localhost:4200`.

### 2. Run the client

```bash
cd todo-app-ui
npm install
npm start
```

Open `http://localhost:4200`. The API base URL is configured in `todo-app-ui/src/app/app.config.ts`.

### Build

```bash
dotnet build TodoApp.sln
cd todo-app-ui && npm run build
```

## Roadmap

- [ ] App shell with a category sidebar and routing
- [ ] Shared category store (single source of truth for the sidebar and task list)
- [ ] Filtering by category via the URL, active / completed sections
- [ ] Toast notifications and an HTTP error interceptor
- [ ] Task details panel with managed focus
- [ ] Unit tests and automated accessibility (AXE) checks
- [ ] Authentication
- [ ] Importance flag, due dates and smart lists (requires backend changes)

## Project structure

```
.
├── TodoApp.API/        ASP.NET Core Web API
├── TodoApp.Services/   Business logic
├── TodoApp.Core/       Domain model and contracts
├── TodoApp.Data/       EF Core context and migrations
├── TodoApp.sln
└── todo-app-ui/        Angular client
```
