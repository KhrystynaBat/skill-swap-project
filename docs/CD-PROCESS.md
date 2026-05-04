# CD Process Documentation

**For:** SkillSwap

**Version:** 0.1

---

## Огляд

Цей документ описує процес безперервної доставки та розгортання (CD) для проєкту SkillSwap.

---

## Середовища розгортання

| Середовище | Гілка     | Frontend URL                                    | Backend URL                                     |
| ---------- | --------- | ----------------------------------------------- | ----------------------------------------------- |
| Staging    | `develop` | https://skillswap-frontend-staging.onrender.com | https://skill-swap-project-staging.onrender.com |
| Production | `main`    | https://skillswap-frontend-rzb6.onrender.com    | https://skill-swap-project-lipk.onrender.com    |

---

## CD Flow

1. Розробник пише код
2. Push в гілку `develop`
3. CI Pipeline — тести, лінтинг, білд
4. Автоматичний деплой на Staging
5. Health check на Staging
6. Merge `develop` → `main`
7. Автоматичний деплой на Production
8. Health check з автоматичним rollback
9. Production live

---

## Стратегія деплою

### Blue-Green Deployment

Проєкт реалізує blue-green стратегію розгортання.

При кожному Pull Request до гілки `main` Render автоматично створює
тимчасове ізольоване середовище (Preview Environment) з унікальним URL.
Це дозволяє перевірити зміни в живому середовищі без впливу на
Production — аналогічно до класичного blue-green підходу.

Після успішної перевірки зміни merge-яться в `main` і деплояться
на Production. Preview середовище автоматично видаляється.

---

## Автоматичний деплой

Деплой запускається автоматично при push в репозиторій:

| Гілка             | Дія                  |
| ----------------- | -------------------- |
| `develop`         | Деплой на Staging    |
| `main`            | Деплой на Production |
| Pull Request      | Preview середовище   |

---

## Моніторинг після деплою

### Health Checks

- Backend endpoint: `/health`
- Перевірка після кожного деплою: 3 спроби з інтервалом 30 секунд
- Render автоматично перевіряє `/health` endpoint periodically

### Сповіщення

- Render Notifications налаштовано на email при падінні сервісу
- GitHub Actions надсилає статус кожного pipeline

---

## Rollback процес

### Автоматичний rollback

Якщо health check провалюється після деплою на Production — pipeline автоматично:

1. Отримує список останніх деплоїв через Render API
2. Повертає попередній успішний деплой
3. Позначає pipeline як failed та сповіщає команду

### Ручний rollback

У разі потреби ручного відкату виконати наступні кроки в Render Dashboard:

1. Обрати сервіс (backend або frontend)
2. Перейти у вкладку **Events**
3. Знайти попередній успішний деплой
4. Натиснути кнопку **Rollback**

Ручний rollback рекомендується використовувати коли:

- автоматичний rollback не спрацював
- потрібно відкотитись на конкретну версію (не обов'язково попередню)
- виникли проблеми які не визначаються health check

---

## GitHub Secrets

Для роботи pipeline необхідні наступні secrets в репозиторії:

| Secret                                | Опис                                |
| ------------------------------------- | ----------------------------------- |
| `RENDER_BACKEND_DEPLOY_HOOK`          | Deploy hook для production backend  |
| `RENDER_FRONTEND_DEPLOY_HOOK`         | Deploy hook для production frontend |
| `RENDER_BACKEND_DEPLOY_HOOK_STAGING`  | Deploy hook для staging backend     |
| `RENDER_FRONTEND_DEPLOY_HOOK_STAGING` | Deploy hook для staging frontend    |
| `RENDER_API_KEY`                      | Render API ключ для rollback        |
| `RENDER_BACKEND_SERVICE_ID`           | ID production backend сервісу       |
| `RENDER_FRONTEND_SERVICE_ID`          | ID production frontend сервісу      |

---

## Інфраструктура

| Сервіс     | Тип                  | Платформа |
| ---------- | -------------------- | --------- |
| Backend    | Docker (Web Service) | Render    |
| Frontend   | Static Site          | Render    |
| База даних | PostgreSQL 18        | Render    |
| CI/CD      | GitHub Actions       | GitHub    |

---

## Контакти та підтримка

При проблемах з деплоєм:

1. Перевірити GitHub Actions → вкладка Actions
2. Перевірити логи в Render Dashboard → Events
3. За потреби виконати ручний rollback
