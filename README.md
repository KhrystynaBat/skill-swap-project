# SkillSwap

[![Frontend](https://img.shields.io/badge/frontend-Angular-DD0031?logo=angular&logoColor=white)](https://angular.dev/)
[![Backend](https://img.shields.io/badge/backend-.NET-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/database-PostgreSQL-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Tests](https://img.shields.io/badge/tests-Angular%20%7C%20xUnit%20%7C%20Playwright-brightgreen)](./tests/automated_tests.md)
![CI](https://github.com/KhrystynaBat/skill-swap-project/actions/workflows/ci.yml/badge.svg)
![Version](https://img.shields.io/github/v/release/KhrystynaBat/skill-swap-project?sort=semver)

**SkillSwap** - це веб-застосунок для обміну навичками між користувачами.
Користувач може створити профіль, додати навички, які він може викладати, додати навички, які хоче вивчити, знайти людей зі спільними інтересами, створити match, спілкуватися в чаті та залишати відгуки після завершення обміну.

Демо: https://skillswap-frontend-rzb6.onrender.com/

## Основні можливості

- Реєстрація та авторизація користувачів через JWT.
- Профіль користувача з містом, біографією, аватаром, навичками та інтересами.
- Пошук користувачів за навичкою, категорією та містом.
- Match-система для взаємного обміну навичками.
- Чат між користувачами.
- Відгуки після завершення match.
- Автоматичне наповнення таблиці `Skills` базовим переліком навичок через EF migration.
- Автоматизовані тести та CI/CD pipeline.

## Технології

| Частина | Технології |
| --- | --- |
| Frontend | Angular, TypeScript, RxJS, Jasmine/Karma, Playwright |
| Backend | ASP.NET Core, Entity Framework Core, SignalR, xUnit |
| Database | PostgreSQL |
| CI/CD | GitHub Actions, semantic-release, Render |

## Структура проєкту

```text
skill-swap-project/
├── backend/
│   ├── SkillSwap.Api/              # ASP.NET Core API, controllers, SignalR hub
│   ├── SkillSwap.Application/      # DTOs
│   ├── SkillSwap.Domain/           # Domain entities
│   ├── SkillSwap.Infrastructure/   # EF Core DbContext, migrations
│   └── SkillSwap.Tests/            # Backend unit/API/integration tests
├── frontend/
│   ├── src/app/                    # Angular application
│   └── e2e/                        # Playwright UI tests
├── tests/                          # Testing documentation
└── .github/workflows/ci.yml        # CI/CD pipeline
```

## ⚙️ Getting Started

### 1. Clone repository

```bash
git clone https://github.com/your-username/skill-swap.git
cd skill-swap
```

### 2. Run with Docker

```bash
docker-compose up --build
```

### 3. Run manually

#### Backend

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

#### Frontend

```bash
cd frontend
npm install
ng serve
```

---

## Тестування

Детальний опис тестів знаходиться тут:

[Ручне тестування](./tests/SkillSwapManualTestCases.md)

[Автоматизоване тестування](./tests/automated_tests.md)


## CI/CD

Pipeline знаходиться у файлі:

[ci.yml](./.github/workflows/ci.yml)

Workflow запускається при:

- `push` у `main`, `develop`;
- відкритті або оновленні Pull Request.

Pipeline виконує:

- генерацію версії build;
- встановлення залежностей frontend;
- lint frontend;
- production build frontend;
- Angular тести з coverage;
- Playwright UI tests;
- restore/build backend;
- backend tests з coverage;
- генерацію coverage report;
- quality gate;
- deploy на staging з гілки `develop`;
- deploy на production з `main` або `master`;
- health checks після deploy;
- rollback для production у разі невдалого health check.

 ## Versioning

Проєкт використовує **semantic-release** для автоматичного versioning та генерації changelog.

### Conventional commits:

- `feat:` нова функціональність  
- `fix:` виправлення багів   
- `BREAKING CHANGE:` несумісні зміни (major version)

## Документація

- [Software Requirements Specification](./docs/SkillSwapSRS.md)

## Автори

- Байдала Олеся
- Бать Христина
