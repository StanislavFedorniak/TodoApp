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
- [x] Створити DTO записи (`TodoTaskDto`, `TodoTaskCreateDto`, `TodoTaskUpdateDto`).
- [x] Написати `TodoTaskMappingExtensions`.
- [x] Реалізувати `ITodoTaskService` та `TodoTaskService` (з обробкою `CancellationToken`).
- [x] Завершити імплементацію сутності `TodoTask` за аналогічним циклом.
- [x] Створити `TodoTaskController`, додати CRUD-операції.
- [x] Зареєструвати `ITodoTaskService` та `TodoTaskService` у `Program.cs` (Dependency Injection).
- [x] Перевірити коректність роботи всіх ендпоінтів (`/api/todotask` та `/api/category`).
- [x] Провести рефакторинг та стандартизацію назв змінних і обробки винятків у сервісах й контролерах.

## In Progress (Current Focus)
Розробка API-шару повністю завершена. Перехід до клієнтської частини.

## Next Steps
- [ ] Отримати рішення та розпочати розробку клієнтської частини (`todo-app-ui`).

## Known Issues / Blockers
- Активні блокери відсутні.