# Current Tasks & Progress

## Completed Tasks
- [x] Створено N-Tier структуру проєкту (TodoApp.API, TodoApp.Core, TodoApp.Data, TodoApp.Services).
- [x] Реалізовано сутність `Category` у доменному шарі (Core).
- [x] Реалізовано DTO для `Category` з використанням record (`CategoryDto`, `CategoryCreateDto`, `CategoryUpdateDto`).
- [x] Написано чисті Extension Methods для мапінгу `CategoryMappingExtensions` без залежності від БД.
- [x] Створено кастомний виняток `NotFoundException` та перенесено його в директорію `Exceptions`.
- [x] Створено інтерфейс `ICategoryService` та реалізовано повністю `CategoryService`. Додано підтримку `CancellationToken`.
- [x] Оптимізовано EF Core запити: проекції для GET-методів та `FindAsync` для команд.
- [x] Підключено проєкти `TodoApp.Core` та `TodoApp.Services` як References до `TodoApp.API`.
- [x] У `Program.cs` зареєстровано `CategoryService` в Dependency Injection.
- [x] У `CategoryController.cs` реалізовано логіку для HTTP GET, POST, PUT, DELETE. Синаксис та обробка винятків виправлені.
- [x] Оновлено `using` для `NotFoundException`.
- [x] Запустити проєкт і перевірити роботу CRUD операцій.
- [x] (Скасовано) Створити глобальний GlobalExceptionHandler (залишено `try-catch`).

## In Progress (Current Focus)
- [-] Розпочати імплементацію сутності `TodoTask` за аналогічним циклом (DTO -> Mapping -> Service -> Controller).

## Next Steps
- [ ] Створити DTO записи (`TodoTaskDto`, `TodoTaskCreateDto`, `TodoTaskUpdateDto`).
- [ ] Написати `TodoTaskMappingExtensions`.
- [ ] Реалізувати `ITodoTaskService` та `TodoTaskService` (з обробкою `CancellationToken`).
- [ ] Створити `TodoTaskController`, додати CRUD-операції.

## Known Issues / Blockers
- Активні блокери відсутні.