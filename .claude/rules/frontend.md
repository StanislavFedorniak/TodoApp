---
paths:
  - "todo-app-ui/**"
---

# UI Project Rules & Guidelines (Angular)

Rules for the TodoApp frontend. Loaded when Claude works with `todo-app-ui/` files; shared communication and workflow rules live in the root `CLAUDE.md`. Tasks: GitHub Issues with the `frontend` label.

**Definition of Done (for every step):** the code follows these rules; every interactive element has an accessible name and is keyboard-operable; `ng build` passes with no errors or warnings; no temporary comments remain in templates.

## 1. Context and Stack

* **Project:** TodoApp is an SPA for managing tasks modeled on Microsoft To-Do UI/UX: a category sidebar, inline task creation, a details panel on the right, a star for important tasks. Works with a REST API on .NET 8.
* **Stack:** Angular 22, strictly typed TypeScript (`unknown` instead of `any`), Bootstrap 5.3.
* **UI language:** texts inside the application are in English.
* **Structure:** `core/` (models, services, stores, tokens, interceptors), `features/<name>/` (screens), `shared/` (reusable components).

## 2. Architecture and Components

* Standalone components only, without an explicit `standalone: true`. The template lives in a separate `.html` file.
* `input()`, `output()`, `model()` instead of decorators.
* `inject()` instead of constructors.
* New singleton services use `@Service()`. Do not rewrite existing `@Injectable({ providedIn: 'root' })` ones without a dedicated issue.
* Data needed by more than one component (for example, the category list) lives in a signal store in `core/` instead of being loaded by each component separately.
* State that must survive a page reload and the Back button (selected filter, open task) is kept in the URL, not in a signal.
* Do not use `@HostBinding` and `@HostListener`. Use only the `host` object in the decorator.

## 3. RxJS and HTTP

* Loading/blocking states (`loading`, `isSubmitting`, `trackingTaskId`) are reset only in `.pipe(finalize(...))` and are not duplicated in `next` and `error`.
* Combine independent requests of one screen with `forkJoin` instead of several separate `subscribe` calls.
* The `error` parameter is typed as `unknown`. The user always sees an error message; `console.error` alone is not enough.
* Optional DTO fields are sent as `null`, never `undefined`, because the backend response model uses `null`. Empty text is normalized with `value?.trim() || null`.
* After a successful response, state is updated immutably via `signal.update()`. Do not reload the whole list because of one change.

## 4. State and Templates

* Local state uses Signals only. `computed()` for derived data, `linkedSignal()` for state that depends on another source but can be changed by the user.
* Native control flow: `@if`, `@for` with `track`, `@switch`. Keep template expressions to simple comparisons; move anything more complex into `computed()` or a method.
* No `ngClass` or `ngStyle`. Use direct `[class.x]` / `[style.x]` bindings and Bootstrap classes.
* Do not leave change-marker comments ("ADDED", "Changed"). A comment explains "why", not "what".

## 5. Forms

* Do not use template-driven forms.
* New forms use Signal Forms (stable since v22). Do not rewrite existing Reactive Forms without a dedicated issue.
* The submit button is disabled while the form is invalid or a request is in progress.

## 6. Accessibility

* The interface must pass AXE checks and meet WCAG AA.
* Every field has a `label` (`visually-hidden` is acceptable). Every button without unambiguous text has an `aria-label` naming the action's target ("Delete task Buy milk").
* Dynamic messages use `aria-live`. Decorative elements (spinners) use `aria-hidden="true"`.
* After adding, deleting, or opening/closing a panel, focus is moved deliberately instead of being lost.
