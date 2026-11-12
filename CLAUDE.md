# TodoApp: shared rules

The rules below apply to both the backend and the frontend. Stack-specific rules live in `.claude/rules/` and are loaded automatically when you work with matching files: `backend.md` (`TodoApp.*`) and `frontend.md` (`todo-app-ui/`). Tasks are tracked in GitHub Issues (labels `backend`, `frontend`): `gh issue list`, `gh issue view <N>`. Read them when the next step comes up; do not load them ahead of time.

## Commands

* Backend: `dotnet build TodoApp.sln`, `dotnet run --project TodoApp.API`.
* Backend tests: `dotnet test TodoApp.sln`. A single test or class: `dotnet test TodoApp.Tests --filter "FullyQualifiedName~DataIsolationTests"` (same for `TodoApp.UnitTests`).
* Frontend (from `todo-app-ui/`): `npm start`, `npm run build`, `npm test` (Vitest via `ng test`).
* Docker: `docker compose up` starts the API + SQL Server; requires `.env` with `MSSQL_SA_PASSWORD` and `JWT_KEY`.

## Solution structure

* `TodoApp.Tests`: integration tests against a real LocalDB `TodoAppDb_Tests` (in CI, SQL Server from the `TODOAPP_TESTS_CONNECTION` variable). `TodoApp.UnitTests`: fast DB-free tests of mapping, settings validation, JWT and the exception handler.
* Startup migrations are applied only with `Database__MigrateOnStartup=true` (set in docker-compose); they are off locally and in tests, where the test factory migrates the database itself.

## 1. Communication

* Respond in Ukrainian, in natural language, like an experienced colleague. Keep technical terms, API names and code in English.
* Be brief and to the point. Explain the reasoning behind a decision only when the choice is non-obvious or there are significant alternatives.
* Be a critical counterpart, not just an executor. If my proposal violates best practices, adds unnecessary code or complicates maintenance, say so directly, explain why and suggest the reference approach. Agree only when you genuinely agree, not by default.
* Before answering, check the actual project code (Read, Grep) instead of assuming. If you cannot see the code in question, say so rather than guessing.
* If current information about the stack or libraries is missing, verify facts through web search against authoritative sources (official documentation) and cite the source. Check versions in `package.json` / `.csproj` instead of relying on memory.

## 2. Workflow

* **One step at a time:** one logical change = one commit (for example, service + controller + test, or the `.ts` + `.html` pair of a component). Do not mix several independent changes in one commit.
* **Branches and PRs:** each issue is done in its own branch from `main` (`feat/<N>-short-description`, `fix/<N>-...`) and merged through a PR with `Closes #N` in the description. Do not commit directly to `main`.
* **When to discuss:** for steps with an architectural choice (new layout, storage, routes, new dependency), start by discussing the approach and alternatives, and move to code only after agreement. Steps unambiguously described in an issue (refactoring, fixes) are done straight from code.
* **"Continue" ("Продовжуємо"):** first review the open issues (`gh issue list`; if it is unclear what is meant, ask), name the next step and implement it.
* **Implementation:** write and change project code directly in files (Edit/Write) instead of only showing it in the reply. After a change, run the build and relevant tests and briefly report the result. In the summary, state what changed, which files were affected and what is worth checking manually.
* **New dependencies and architectural changes:** do not add packages or change the structure without agreement (see "When to discuss").
* **Issues:** do not create, edit or close them without my direct request. Closing happens through `Closes #N` in a PR/commit. If you discover a new problem along the way, mention it in the reply and do not fix it in passing.
* **Rule files (`CLAUDE.md`, `.claude/rules/*.md`):** update only at my request.
* **Commits:** do not add a commit message to every step. When I ask you to write one, use Conventional Commits (`feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `style:`).
* **When asked to make a commit:** first review `git status` and `git diff`, check whether there is anything to commit and whether it deserves a separate commit by enterprise standards. A commit must carry a meaningful, self-contained change of behavior, contract or structure. Trivia (a removed unused `using`, formatting, renaming one variable, whitespace fixes) is not committed separately: attach it to an adjacent change or tell me a separate commit is not worthwhile. Do not add files unrelated to the step (other people's uncommitted changes); commit changes to `CLAUDE.md` and `.claude/rules/` separately from code. Do not push without a direct request.

## 3. Review

* When I ask you to review code, point out substantive problems: behavior, project rules, architecture, security, accessibility. Mention stylistic nitpicks only if they affect readability or violate project rules. Do not praise the obvious.
