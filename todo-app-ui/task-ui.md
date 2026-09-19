# UI Tasks & Implementation Checklist

## Project Scope
Розробка клієнтської частини TodoApp на Angular (v20+) з використанням Bootstrap для взаємодії з .NET 8 Web API.

## Core Features & Tasks

### 1. Environment & Setup
- [x] Налаштувати HTTP-клієнт (provideHttpClient) та базовий URL до API (через API_URL InjectionToken)
- [x] Створити базову структуру директорій: core/, Features/, shared/`n- [x] Визначити TypeScript-моделі (інтерфейси для TodoTask, Category, DTO запитів/відповідей)

### 2. Authentication & Authorization (Log in / Log out)
- [-] *Skipped* (Відкладено до реалізації контролерів автентифікації на бекенді)

### 3. Categories Management
- [x] Створити CategoryService для виконання API-запитів з методами getAll, getById, create, update, delete`n- [x] Згенерувати компонент CategoryList (eatures/categories/category-list)
- [x] Реалізувати логіку CategoryListComponent (використання Angular Signals для стану завантаження, помилок, списку категорій)
- [x] Розробити HTML-шаблон для CategoryList (таблиця або список карток для категорій)
- [ ] Додати Reactive Form для створення нових категорій (вбудовану в цей самий компонент)
- [ ] Підключити методи додавання та видалення категорій з миттєвим оновленням UI-стану
- [ ] Вивести обробку помилок та стан завантаження в інтерфейс

### 4. Tasks Management (CRUD)
- [ ] Створити TodoTaskService для виконання CRUD-операцій з HTTP клієнтом
- [ ] Згенерувати компонент TaskList (Features/tasks/task-list)
- [ ] Реалізувати отримання списку завдань із відображенням статусів виконання
- [ ] Додати Reactive Form для створення нового завдання (з можливістю вибору створених категорій зі списку)
- [ ] Реалізувати можливість відзначати завдання як виконане/невиконане (зміна статусу)
- [ ] Реалізувати редагування та видалення завдань із підтвердженням

### 5. Filtering & Advanced UI/UX
- [ ] Реалізувати фільтрацію завдань за обраною категорією на рівні UI
- [ ] Додати візуальні сповіщення для помилок або успішних дій (наприклад, Bootstrap Toasts або просто локальні Alerts)
- [ ] Застосувати фінальну стилізацію за допомогою Bootstrap (доступність компонентів, правильні margin/padding)
- [ ] Перевірити UI на відповідність WCAG AA (контрастність, керування фокусом під час додавання нових елементів)
