# Документація автоматизованих тестів

Цей документ описує стратегію автоматизованого тестування проєкту SkillSwap.
У проєкті реалізовано backend unit tests, backend API tests, backend integration tests, frontend Angular tests, Playwright UI tests, а також CI/CD pipeline через GitHub Actions.

## Загальна таблиця

| Група | Інструменти | Розташування | Що перевіряється | Кількість |
| --- | --- | --- | --- | ---: |
| Backend unit tests | xUnit + EF Core InMemory | [ChatControllerTests.cs](../backend/SkillSwap.Tests/ChatControllerTests.cs)<br>[MatchControllerTests.cs](../backend/SkillSwap.Tests/MatchControllerTests.cs)<br>[ProfileControllerTests.cs](../backend/SkillSwap.Tests/ProfileControllerTests.cs)<br>[ReviewControllerTests.cs](../backend/SkillSwap.Tests/ReviewControllerTests.cs) | Контролери, валідація, авторизація, логіка match/review/chat/profile | 32 |
| Backend API tests | xUnit + реальні HTTP-запити | [ApiTests.cs](../backend/SkillSwap.Tests/ApiTests.cs) | HTTP-статуси, захищені endpoint-и, некоректні запити, відповідь login з token | 8 |
| Backend integration tests | xUnit + ASP.NET Core test app + EF Core InMemory | [IntegrationTests.cs](../backend/SkillSwap.Tests/IntegrationTests.cs)<br>[ApiIntegrationTestApp.cs](../backend/SkillSwap.Tests/ApiIntegrationTestApp.cs) | Повні API-сценарії: register/login, search, match, accept match, review | 5 |
| Frontend service tests | Angular TestBed + HttpTestingController | [auth.service.spec.ts](../frontend/src/app/core/services/auth.service.spec.ts)<br>[users.service.spec.ts](../frontend/src/app/core/services/users.service.spec.ts)<br>[profile.service.spec.ts](../frontend/src/app/core/services/profile.service.spec.ts)<br>[skills.service.spec.ts](../frontend/src/app/core/services/skills.service.spec.ts) | Auth, users, profile, skills HTTP-запити та робота з token | 24 |
| Frontend component tests | Angular TestBed + Jasmine | [app.spec.ts](../frontend/src/app/app.spec.ts)<br>[login.spec.ts](../frontend/src/app/features/auth/pages/login/login.spec.ts)<br>[register.spec.ts](../frontend/src/app/features/auth/pages/register/register.spec.ts)<br>[matches.component.spec.ts](../frontend/src/app/features/matches/pages/matches/matches.component.spec.ts)<br>[search-users.component.spec.ts](../frontend/src/app/features/users/pages/search-users/search-users.component.spec.ts) | Login/register форми, пошук користувачів, matches, app shell | 25 |
| UI tests | Playwright | [skill-swap.ui.spec.ts](../frontend/e2e/skill-swap.ui.spec.ts)<br>[playwright.config.ts](../frontend/playwright.config.ts) | Реальні browser-сценарії: login, register, search + match, finish match + review | 4 |

Поточна кількість автоматизованих тест-кейсів: **98**.


## Backend Tests

### Unit Tests

Backend unit tests напряму викликають методи контролерів і перевіряють логіку контролерів без запуску повного web server.

Файли:

| Файл | Що покриває |
| --- | --- |
| `ChatControllerTests.cs` | Завантаження діалогів, створення повідомлень, належність повідомлень користувачам, логіка чату |
| `MatchControllerTests.cs` | Створення match, перевірка duplicate match, некоректний status, accept/reject/complete |
| `ProfileControllerTests.cs` | Завантаження профілю, оновлення профілю, додавання skills та interests |
| `ReviewControllerTests.cs` | Створення review, перевірка rating, захист від duplicate review, завершення active match |

### API Tests

API tests виконують реальні HTTP-запити до тестового застосунку.
Вони перевіряють поведінку API на рівні HTTP: `401 Unauthorized`, `400 Bad Request`, `404 Not Found`, а також успішні відповіді.

Приклади покритих сценаріїв:

| Сценарій | Очікуваний результат |
| --- | --- |
| Search users без авторизації | `401 Unauthorized` |
| Register з некоректним паролем | `400 Bad Request` |
| Login з неправильним паролем | `401 Unauthorized` |
| Отримання неіснуючого користувача | `404 Not Found` |
| Login з правильними даними | JSON-відповідь містить `token` |
| Create match без авторизації | `401 Unauthorized` |
| Create review без active match | `400 Bad Request` |

### Integration Tests

Integration tests запускають легкий ASP.NET Core test application.

Основні integration-сценарії:

| Сценарій | Що доводить |
| --- | --- |
| Register then login | Auth endpoints працюють разом і повертають JWT token |
| Search users by category | Authorized search endpoint повертає відповідних користувачів |
| Create, accept and list match | Життєвий цикл match працює через реальні HTTP-запити |
| Invalid match request | API блокує match, якщо skills/interests не збігаються |
| Finish match with review | Review створюється, а active match переходить у completed |

## Frontend Angular Tests

Angular tests використовують Jasmine, Angular TestBed та HttpTestingController.

### Service Tests

| Файл | Що покриває |
| --- | --- |
| `auth.service.spec.ts` | Register/login/me запити, збереження token, отримання user id з token |
| `users.service.spec.ts` | Пошук користувачів, match, reviews, text responses |
| `profile.service.spec.ts` | API-запити профілю, skills та interests |
| `skills.service.spec.ts` | Завантаження skills з API |

### Component Tests

| Файл | Що покриває |
| --- | --- |
| `app.spec.ts` | Створення root app та рендер app shell |
| `login.spec.ts` | Некоректна форма, успішний login, помилка login |
| `register.spec.ts` | Некоректна форма, register + auto-login, помилка register |
| `matches.component.spec.ts` | Завантаження matches, accept/reject, finish match, review |
| `search-users.component.spec.ts` | Фільтрація skills, search submit, empty/error states, створення match |

## Playwright UI Tests

Playwright UI tests запускають Angular application у Chromium і взаємодіють зі сторінкою так, як це робить користувач.

UI-сценарії:

| Тест | User flow |
| --- | --- |
| Login page submits credentials and opens profile | Користувач заповнює login form і переходить на profile |
| Register page creates account, logs in and opens profile | Користувач реєструється, auto-login успішний, відкривається profile |
| Search page finds user and creates match from the UI | Користувач шукає skill і натискає Match у картці результату |
| Matches page finishes active match with review form | Користувач відкриває finish form, залишає review і завершує match |


## CI/CD Integration

CI/CD workflow знаходиться тут:

```text
.github/workflows/automated-tests.yml
```
