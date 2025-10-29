# Software Requirements Specification

**For:** SkillSwap

**Version:** 0.1

**Prepared by:** Olesia Baidala & Khrystyna Bat


## 1. Product Overview

### 1.1 Category of Product

Веб-застосунок для бартерного обміну навичками та послугами між людьми.

### 1.2 Purpose

Метою є дати людям можливість обмінюватися своїми навичками без використання грошей, сприяти розвитку співпраці, взаємодопомоги та неформального навчання.

### 1.3 Product Description

SkillSwap — це застосунок, що об’єднує людей, які хочуть обмінятися своїми навичками або послугами.
Кожен користувач створює персональний профіль, у якому може описати власні вміння, інтереси та потреби.
Система аналізує наявні пропозиції в базі даних і автоматично підбирає можливі поєднання, допомагаючи швидко знайти партнера для взаємного навчання чи обміну.
У застосунку реалізовано внутрішній чат для комунікації, можливість домовлятися про деталі співпраці, отримувати сповіщення про нові збіги та залишати відгуки після завершених обмінів.

### 1.4 Target Audience

Студенти, фрілансери, молоді спеціалісти, невеликі спільноти.


## 2. Actors / Personas

### 2.1 Regular User (Learner/Sharer)

**Опис:** людина, яка хоче навчитися новому або поділитися своїми навичками.

**Цілі**
* Знайти людину, яка навчить потрібної навички.
* Поділитися власними знаннями чи допомогти іншому.
* Отримати відгуки для підвищення репутації.

**Сценарії використання**
* Створює профіль і вказує власні навички.
* Шукає партнерів для обміну.
* Використовує чат для домовленостей.
* Залишає відгуки після обміну.

### 2.2 Moderator

**Опис:** відповідає за підтримку порядку у спільноті та має розширені права доступу.

**Цілі**
* Контролювати якість контенту та спілкування.
* Блокувати некоректних користувачів.
* Стежити за безпекою та дотриманням правил.

**Сценарії використання**
* Переглядає скарги користувачів.
* Видаляє або редагує профілі/повідомлення.
* Налаштовує загальні параметри системи (категорії навичок).


## 3. Functional Requirements

| ID    | Назва                            | Опис                                                                                                     |
|-------|----------------------------------|----------------------------------------------------------------------------------------------------------|
| FR01 | Реєстрація користувача           | Система повинна дозволяти новому користувачу створювати акаунт за допомогою email та пароля.             |
| FR02 | Вхід користувача                 | Система повинна дозволяти зареєстрованому користувачу входити в систему з правильними обліковими даними. |
| FR03 | Відновлення пароля               | Система повинна дозволяти користувачу відновити пароль через email.                                      |
| FR04 | Соціальний логін                 | Система може дозволяти користувачам входити через Google або Facebook.                                   |
| FR05 | Створення та редагування профілю | Система повинна дозволяти створювати й змінювати профіль.                                                |
| FR06 | Додавання навичок                | Система повинна дозволяти вказувати власні навички.                                                      |
| FR07 | Додавання інтересів              | Система повинна дозволяти зазначати навички чи послуги, які користувач хоче отримати.                    |
| FR08 | Завантаження аватарки            | Система повинна дозволяти завантажити фото профілю.                                                      |
| FR09 | Відображення відгуків            | Система повинна показувати рейтинг та відгуки у профілі.                                                 |
| FR10 | Пошук за ключовими словами       | Система повинна дозволяти пошук користувачів за ключовими словами.                                       |
| FR11 | Фільтрація пошуку                | Система повинна підтримувати фільтри (категорія, рівень навичок, місто).                                 |
| FR12 | Автоматичний підбір партнерів    | Система повинна рекомендувати потенційних партнерів.                                                     |
| FR13 | Сповіщення про збіги             | Система повинна інформувати користувача про нові збіги.                                                  |
| FR14 | Чат між користувачами            | Система повинна надавати можливість обміну повідомленнями в реальному часі.                              |
| FR15 | Надсилання файлів                | Система повинна дозволяти прикріплювати файли та зображення у чаті.                                      |
| FR16 | Домовленість про час             | Система повинна надавати інструмент для узгодження часу зустрічі чи заняття.                             |
| FR17 | Підтвердження обміну             | Система повинна вимагати підтвердження угоди обома сторонами.                                            |
| FR18 | Залишення відгуку                | Система повинна дозволяти залишати відгуки після завершення обміну.                                      |
| FR19 | Історія обмінів                  | Система повинна зберігати список завершених обмінів.                                                     |
| FR20 | Email-сповіщення                 | Система повинна надсилати повідомлення на email при нових збігах.                                        |
| FR21 | Сповіщення про повідомлення      | Система повинна інформувати користувача про нові повідомлення у чаті.                                    |
| FR22 | Редагування облікових даних      | Система повинна дозволяти змінювати email та пароль.                                                     |
| FR23 | Видалення акаунта                | Система повинна дозволяти користувачу видаляти свій акаунт.                                              |
| FR24 | Налаштування приватності         | Система повинна дозволяти обирати, які дані видно іншим.                                                 |
| FR25 | Блокування користувачів          | Адміністратор може блокувати небажаних користувачів.                                                     |
| FR26 | Модерація контенту               | Адміністратор може видаляти або редагувати неприйнятний контент.                                         |
| FR27 | Аналітика активності             | Система повинна мати панель для перегляду статистики (користувачі, обміни, скарги).                      |
| FR28 | Управління категоріями навичок   | Адміністратор може створювати та редагувати категорії навичок.                                           |


## 4. Non-Functional Requirements

| ID     | Назва             | Опис                                                                                           |
| ------ | ----------------- | ---------------------------------------------------------------------------------------------- |
| NFR01 | Простий інтерфейс | Інтерфейс має бути простим і зрозумілим (мінімум «зайвих кліків»).                             |
| NFR02 | Локалізація       | Система повинна підтримувати українську та англійську мови.                                    |
| NFR03 | Адаптивність      | Інтерфейс має бути придатним для мобільного браузера.                                          |
| NFR04 | Продуктивність    | Час відповіді на основні дії (вхід, пошук, повідомлення) — не більше 3 секунд.                 |
| NFR05 | Безпека паролів   | Паролі повинні зберігатися у вигляді хешів.                                                    |
| NFR06 | Шифрування        | Вхід у систему має відбуватися через HTTPS.                                                    |
| NFR07 | Доступність       | Система має бути доступною принаймні 90% часу (допускаються невеликі збої).                    |
| NFR08 | Логування         | У випадку помилок система повинна записувати повідомлення з датою та дією, яка викликала збій. |

## 5. Use Case Diagram

![Use Case Diagram](https://uml.planttext.com/plantuml/svg/fLPRJXj14Fn7yXr6V8KFNo72Pq0SO6KSOc4UCXlb8uhaHmH4Gd4K8IEXyB1yWMLjOB7nSeMU6wMwPyPUxsnCf012pBgwkgkwUvRDup1eXf_s6wMrHkr3gC93rQplVWpLJhrPguRrmuFIMcajg8Q7JRLELvJg5YMwhJkwIp55-dHT1STgUm7vIMDwqNsQwRx0MW1rE4o05MPZuk1Wjr43V63Ow85UA59RU_L6O-jpSBIklZ1IgQEWkXViClAE7lJ5dF84z3s5m4XHHB4mpF0xqT-GVE8GmgFmjQtAvVVg7LtYazQ2oQf07DDmM9TfDfQ3XaZGfsS3YvLkBuhMtHps5v1Zcb4AuiH4l4hEbARwV0MnFaLPA8sUE46gJ-28UV1eatpDMtBBj28Or0hbUzsXo7DWW6onHLhTzvGgoJJ6OPJrv3MR8LVy1IID7N36NG3ZAQf8LW1bCZf66FkuuRM7amiceiT5YQOYzd7YMNvhVHgXo1v2kxPHqWcMuqho3OIuCmQWBXRB0VUOcrR1t8obKsqedRVL1HSv3tIS7mrAiaZdY7LN1ZjBw-cyCH3UbBajl5lk0Sp7rfbEJkGnTaASvrsuV8CDqbvvQW7zBe4pB9WP04LFR0iC_ekFDnWXMNgxaJ428dq1xfHzAzZ3WRWzPPLksk09DhhBYu2pxkVglOPSvceZoZc7Hyyu9Yl2XYXb90EOk3MBT5lKSej4e1YK7oeU1baL7YuN3IAhj61mKOW-Sy3CzFtJk9hhagS3V_t899eFakVwm6-OvtE-oxNzeDue5_NwVyR3VWB1F7NJ9KglxA7n7CvEMPyXobo-pE7fk2kwdTYZgLptabu9OkQralTkRFlpnar_BR082zZ9NuE9FReNstcnEk_zPtYiJhCUiXOo1VdNL2KpJj06t2HRH-4Dlx4S-jkHvmBDlPlHN5bzVKPofyDUFd8PufiSOdw_IH7z7DULdpYIj2CxsJs8NtfTf7O_O2H-Dci7E_BF27__0G00)

## 6. Sequence Diagram

```mermaid
sequenceDiagram
autonumber
actor U as Користувач
actor A as Адміністратор
participant FE as Frontend (Angular SPA)
participant BE as Backend API (.NET Core)
participant DB as PostgreSQL
participant RS as Redis (Cache/Sessions)
participant SG as SignalR Hub
participant AF as Azure Functions
participant EM as Email Service

U->>FE: Відкрити сайт
FE->>BE: GET /
BE-->>FE: Завантаження SPA

U->>FE: Ввести email і пароль для реєстрації
FE->>BE: POST /api/auth/register
BE->>DB: INSERT new user
DB-->>BE: OK
BE->>RS: Зберегти сесію
BE->>EM: Надіслати підтвердження
EM-->>U: Email підтвердження
BE-->>FE: 201 Created

U->>FE: Увійти в систему
FE->>BE: POST /api/auth/login
BE->>DB: SELECT user
DB-->>BE: OK
BE->>RS: Зберегти токен у Redis
BE-->>FE: JWT Token
FE-->>U: Авторизація успішна

U->>FE: Оновити профіль (навички, інтереси, фото)
FE->>BE: PUT /api/profile
BE->>DB: UPDATE profile
DB-->>BE: OK
BE->>RS: Оновити кеш
BE-->>FE: OK

U->>FE: Пошук користувачів
FE->>BE: GET /api/search?q=design
BE->>RS: Перевірити кеш
alt Результат у кеші
    RS-->>BE: Cached data
else Немає у кеші
    BE->>DB: SELECT користувачі за параметрами
    DB-->>BE: Дані
    BE->>RS: Зберегти в кеш
end
BE-->>FE: Результати пошуку
FE-->>U: Відображення результатів

BE->>DB: Автоматичний підбір партнерів
DB-->>BE: Список збігів
BE->>SG: Надіслати повідомлення в реальному часі
SG-->>FE: Push сповіщення
FE-->>U: Новий збіг

U->>FE: Надіслати повідомлення
FE->>SG: SignalR sendMessage()
SG->>BE: Зберегти повідомлення
BE->>DB: INSERT message
DB-->>BE: OK
BE-->>SG: OK
SG-->>FE: Доставка повідомлення іншому користувачу

U->>FE: Узгодити час співпраці
FE->>BE: POST /api/exchange/schedule
BE->>DB: UPDATE exchange
DB-->>BE: OK
BE-->>FE: OK

U->>FE: Підтвердити обмін
FE->>BE: POST /api/exchange/confirm
BE->>DB: UPDATE status
DB-->>BE: OK
BE-->>FE: OK

U->>FE: Залишити відгук
FE->>BE: POST /api/reviews
BE->>DB: INSERT review
DB-->>BE: OK
BE->>RS: Оновити кеш профілю
BE-->>FE: OK


BE->>AF: Виклик Azure Function для розсилки
AF->>EM: Надіслати email
EM-->>U: Email-сповіщення
SG-->>FE: SignalR сповіщення у браузері
FE-->>U: Нове повідомлення / збіг

A->>FE: Увійти в адмін-панель
FE->>BE: POST /api/admin/login
BE->>DB: SELECT admin
DB-->>BE: OK
BE-->>FE: JWT Token
FE-->>A: Авторизація успішна

A->>FE: Заблокувати користувача
FE->>BE: POST /api/admin/block/{userId}
BE->>DB: UPDATE users SET status=blocked
DB-->>BE: OK
BE->>RS: Інвалідність кешу
BE-->>FE: OK

A->>FE: Переглянути аналітику
FE->>BE: GET /api/admin/stats
BE->>DB: SELECT stats
DB-->>BE: Дані
BE-->>FE: JSON дані
FE-->>A: Графіки статистики
```

## 7. High-Level Architecture

### 7.1 Operating Environment

Операційне середовище для системи **SkillSwap**:

* **Архітектурний підхід:** клієнт–серверна система з розділенням на frontend (SPA) та backend (REST API).
* **Frontend:** Angular SPA, що виконується у браузері користувача.
* **Backend:** .NET Core (C#), реалізує REST API та SignalR для чату в реальному часі.
* **Database:** PostgreSQL – зберігання акаунтів, профілів, навичок, історії обмінів та відгуків.
* **Cache:** Redis – кеш пошукових запитів, зберігання сесій та токенів.
* **Message/Notification Layer:** SignalR (websocket) для push-повідомлень і миттєвого чату.
* **Клієнтське середовище:** сучасні браузери (Chrome, Firefox, Opera, Safari), адаптивна верстка для мобільних пристроїв.

### 7.2 Assumptions and Dependencies

Система розроблятиметься з використанням платформи **.NET Core** та мови програмування **C#**.

**Додаткові залежності**
* **Angular** — для створення односторінкового вебзастосунку (SPA).  
* **SignalR** — для реалізації чату та сповіщень у реальному часі.  
* **PostgreSQL** — для зберігання профілів користувачів, навичок і історії обмінів.  
* **Redis** — для кешування та зберігання сесій.  
* **Docker** — для контейнеризації та розгортання застосунку.  
* **Azure Functions** — для обробки сповіщень і виконання фонових завдань.

Фреймворк **Angular** забезпечує роботу застосунку без перезавантаження сторінки. Система буде доступною у браузерах **Chrome**, **Opera**, **Mozilla Firefox**, **Safari** та матиме адаптивний дизайн для смартфонів і планшетів.

```mermaid
graph TD
    subgraph Client["Client Side"]
        Browser["Web Browser<br/>(Chrome / Firefox / Safari / Opera)"]
    end

    subgraph Frontend["Frontend Layer"]
        Angular["Angular SPA"]
    end

    subgraph Backend["Backend Layer (.NET Core API)"]
        API["REST API"]
        SignalR["SignalR Hub (Real-time Chat/Notifications)"]
    end

    subgraph Data["Data Layer"]
        PostgreSQL["PostgreSQL Database"]
        Redis["Redis (Cache & Sessions)"]
    end

    subgraph Services["Background & External Services"]
        AzureF["Azure Functions<br/>(Background Jobs, Notifications)"]
        Email["Email Service"]
    end

    subgraph Infra["Infrastructure"]
        Docker["Docker Containers"]
    end

    %% Connections
    Browser --> Angular
    Angular --> API
    Angular --> SignalR

    API --> PostgreSQL
    API --> Redis
    API --> AzureF
    AzureF --> Email

    SignalR --> Redis

    API --> Docker
    SignalR --> Docker
    PostgreSQL --> Docker
    Redis --> Docker
```

## 8. Concurrency and Distributed Patterns

### 8.1 Обробка кількох пошукових запитів одночасно

Користувачі одночасно виконують пошук партнерів за навичками. Сервер обробляє їхні запити через пул потоків (**Thread Pool**), що дозволяє уникнути перевантаження системи та забезпечує швидке отримання результатів пошуку.

```mermaid
sequenceDiagram
    title Обробка кількох пошукових запитів одночасно
    participant User1
    participant User2
    participant User3
    participant SearchService
    participant Database

    User1->>SearchService: Запит на пошук партнерів
    User2->>SearchService: Запит на пошук партнерів
    User3->>SearchService: Запит на пошук партнерів

    note over SearchService: Сервіс отримує кілька запитів одночасно
    
    SearchService->>SearchService: Запуск потоків у Thread Pool

    par Потік 1
        SearchService->>Database: Пошук партнерів для User1
        Database-->>SearchService: Результати для User1
    end
    par Потік 2
        SearchService->>Database: Пошук партнерів для User2
        Database-->>SearchService: Результати для User2
    end
    par Потік 3
        SearchService->>Database: Пошук партнерів для User3
        Database-->>SearchService: Результати для User3
    end
    
    SearchService-->>User1: Надіслати результати пошуку
    SearchService-->>User2: Надіслати результати пошуку
    SearchService-->>User3: Надіслати результати пошуку
    
    note over User1,User3: Усі користувачі отримують відповіді одночасно
```

### 8.2 Обопільне підтвердження обміну навичками 

Користувачі одночасно виконують дії, пов’язані з підтвердженням обміну навичками. Якщо один користувач змінює або видаляє свій профіль під час очікування відповіді, система перевіряє актуальність даних перед підтвердженням угоди. У разі виявлення змін запит скасовується, а обидва користувачі отримують повідомлення про недійсність обміну. Такий підхід запобігає конфліктам даних і забезпечує узгодженість системи.

```mermaid
sequenceDiagram
    title Обопільне підтвердження обміну навичками
    participant UserA
    participant SkillSwapService
    participant UserB
    participant Database
    participant NotificationService

    UserA->>SkillSwapService: Надіслати запит на обмін
    SkillSwapService->>Database: Зберегти запит (version=1)
    SkillSwapService->>UserB: Сповіщення про новий запит

    par
        UserB->>SkillSwapService: Підтвердити обмін
        UserB->>SkillSwapService: Змінити або видалити профіль
    end

    SkillSwapService->>Database: Перевірити версію запису
    Database-->>SkillSwapService: Конфлікт версій (version mismatch)

    SkillSwapService->>NotificationService: Надіслати повідомлення про скасування
    NotificationService-->>UserA: Обмін недійсний (дані змінено)
    NotificationService-->>UserB: Обмін скасовано

    note over UserA,UserB: Optimistic Concurrency — система запобігає конфліктам,якщо дані змінились під час підтвердження
```

### 8.3 Обмеження частоти запитів на обмін 

Користувачі одночасно надсилають запити на обмін навичками. Система застосовує обмежувач частоти (**Rate Limiter**), який перевіряє кількість вихідних запитів від користувача та кількість вхідних запитів до адресата. Якщо ліміт перевищено — запит відхиляється або відкладається; якщо ні — створюється новий запис у базі даних і надсилається сповіщення адресату. Це запобігає спаму, перевантаженню сервера та забезпечує стабільну роботу системи під час великої кількості одночасних дій.

```mermaid
sequenceDiagram
    title Обмеження частоти запитів на обмін
    participant UserA as "UserA (Requester)"
    participant ExchangeService
    participant RateLimiter
    participant Database
    participant NotificationService
    participant UserB as "UserB (Target)"

    UserA->>ExchangeService: Надіслати запит на обмін (targetId)
    ExchangeService->>RateLimiter: Перевірити ліміт (by Requester)
    RateLimiter-->>ExchangeService: OK / Перевищено

    alt Ліміт відправника перевищено
        ExchangeService-->>UserA: Запит відхилено (занадто часто)<br/>Спробуйте пізніше
    else Ліміт відправника OK
        ExchangeService->>RateLimiter: Перевірити вхідний ліміт для Target
        RateLimiter-->>ExchangeService: OK / Інбокс переповнений
        alt Інбокс переповнений (Target)
            ExchangeService-->>UserA: Запит відхилено / поставлено в очікування
        else Обидва ліміти OK
            ExchangeService->>Database: Створити pending-запит
            Database-->>ExchangeService: Створено
            ExchangeService->>NotificationService: Сповістити Target про запит
            NotificationService-->>UserB: Новий запит на обмін
            ExchangeService-->>UserA: Запит надіслано успішно
        end
    end

    note over RateLimiter,ExchangeService: Token Bucket/Sliding Window для двох правил
```

### 8.4 Асинхронне оновлення рекомендацій 

Коли користувач змінює свої навички або інтереси у профілі, система має оновити список рекомендованих партнерів. Щоб не затримувати користувача під час збереження профілю, ці обчислення виконуються **асинхронно**. Після збереження профілю створюється подія **ProfileUpdated**, яку перехоплює фоновий сервіс і у кількох потоках оновлює рекомендації, не блокуючи інтерфейс користувача. (Event-Driven / Async)

```mermaid
sequenceDiagram
    title Асинхронне оновлення рекомендацій
    participant User
    participant ProfileService
    participant EventBus
    participant RecommendationService
    participant Database
    participant NotificationService

    User->>ProfileService: Оновити профіль (навички або інтереси)
    ProfileService->>Database: Зберегти зміни профілю
    ProfileService->>EventBus: Відправити подію "ProfileUpdated"
    ProfileService-->>User: Повідомити про успішне збереження профілю

    note over EventBus: Подія обробляється у фоновому режимі

    EventBus->>RecommendationService: Отримати подію "ProfileUpdated"
    RecommendationService->>Database: Перерахувати рекомендації
    Database-->>RecommendationService: Нові результати підбору
    RecommendationService->>NotificationService: Повідомити користувача про оновлення
    NotificationService-->>User: Оновлений список рекомендованих партнерів

    note over User,RecommendationService: Користувач одразу отримує підтвердження, а рекомендації оновлюються у фоні без затримки інтерфейсу
```

### 8.5 Надсилання повідомлень у чаті в реальному часі 

У системі **SkillSwap** користувачі спілкуються через чат у реальному часі. Коли багато користувачів одночасно надсилають повідомлення у різних чатах, сервер може бути перевантажений. Щоб уникнути затримок та втрати повідомлень під час пікових навантажень, застосовано патерн **Producer–Consumer**. Кожне повідомлення потрапляє до черги, де споживачі (серверні потоки) паралельно обробляють і доставляють його адресату. Це забезпечує стабільну роботу чату навіть при великій кількості активних діалогів.

```mermaid
sequenceDiagram
    title Надсилання повідомлень у чаті в реальному часі
    participant UserA
    participant ChatService
    participant MessageQueue
    participant Database
    participant UserB

    UserA->>ChatService: Надіслати повідомлення
    ChatService->>MessageQueue: Додати повідомлення у чергу
    note over MessageQueue: Черга вирівнює навантаження — повідомлення обробляються у фонових потоках
    MessageQueue->>ChatService: Передати повідомлення споживачу
    ChatService->>Database: Зберегти повідомлення
    Database-->>ChatService: Підтвердження збереження
    ChatService-->>UserA: Повідомлення доставлено успішно
    ChatService-->>UserB: Отримано нове повідомлення

    note over UserA,UserB: Повідомлення передаються через чергу — кілька чатів обробляються паралельно без блокувань
```

### 8.6 Захист сповіщень від недоступності зовнішнього API 

Система надсилає сповіщення через зовнішній сервіс. Коли цей сервіс недоступний, **Circuit Breaker** тимчасово блокує виклики, а сповіщення ставляться у **чергу з експоненційною затримкою (Retry Queue)**. Після відновлення з’єднання коло знову відкривається, і накопичені повідомлення автоматично відправляються повторно. Такий підхід запобігає перевантаженню системи та гарантує доставку повідомлень після збою.

```mermaid
sequenceDiagram
    title Захист сповіщень від недоступності зовнішнього API
    participant NotificationService
    participant CircuitBreaker
    participant RetryQueue
    participant ExternalAPI as "External Notification API"
    participant User

    NotificationService->>CircuitBreaker: Надіслати сповіщення (userId, payload)
    alt API доступний
        CircuitBreaker->>ExternalAPI: Відправити сповіщення
        ExternalAPI-->>CircuitBreaker: Успіх
        CircuitBreaker-->>NotificationService: OK
        NotificationService-->>User: Сповіщення доставлено
    else API недоступний / помилки
        CircuitBreaker->>CircuitBreaker: Відкрити коло (open) (блокувати виклики на N сек)
        CircuitBreaker->>RetryQueue: Додати завдання у чергу (backoff)
        CircuitBreaker-->>NotificationService: Прийнято до повторної відправки

        note over RetryQueue: Очікування згідно backoff (10s, 30s, 60s…)

        RetryQueue->>CircuitBreaker: Час повторної спроби
        alt Спроба у напіввідкритому стані (half-open)
            CircuitBreaker->>ExternalAPI: Тестова відправка
            ExternalAPI-->>CircuitBreaker: Успіх
            CircuitBreaker->>CircuitBreaker: Закрити коло (close)
            CircuitBreaker->>ExternalAPI: Відправити сповіщення з черги
            ExternalAPI-->>CircuitBreaker: Успіх
            CircuitBreaker-->>RetryQueue: Видалити завдання
            RetryQueue-->>NotificationService: Повторна відправка успішна
            NotificationService-->>User: Сповіщення доставлено
        else Знову помилка
            CircuitBreaker->>CircuitBreaker: Залишити коло відкритим
            CircuitBreaker->>RetryQueue: Перепланувати наступну спробу (більший backoff)
        end
    end

    note over NotificationService,ExternalAPI: Circuit Breaker захищає систему від перевантаження, а черга гарантує повторну доставку після відновлення сервісу
```
