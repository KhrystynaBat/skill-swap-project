# Software Requirements Specification

**For:** SkillSwap

**Version:** 0.1

**Prepared by:** Olesia Baidala & Khrystyna Bat

## 1. Actors / Personas

### 1.1 Regular User (Learner/Sharer)

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

### 1.2 Moderator

**Опис:** відповідає за підтримку порядку у спільноті та має розширені права доступу.

**Цілі**
* Контролювати якість контенту та спілкування.
* Блокувати некоректних користувачів.
* Стежити за безпекою та дотриманням правил.

**Сценарії використання**
* Переглядає скарги користувачів.
* Видаляє або редагує профілі/повідомлення.
* Налаштовує загальні параметри системи (категорії навичок).


## 2. Functional Requirements

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


## 3. Non-Functional Requirements

| ID     | Назва             | Опис                                                                                           |
| ------ | ----------------- | ---------------------------------------------------------------------------------------------- |
| NFR01 | Простий інтерфейс | Інтерфейс має бути простим і зрозумілим (мінімум «зайвих кліків»).                             |
| NFR02 | Локалізація       | Система повинна підтримувати українську та англійську мови.                                    |
| NFR03 | Адаптивність      | Інтерфейс має бути придатним для мобільного браузера.                                          |
| NFR04 | Безпека паролів   | Паролі повинні зберігатися у вигляді хешів.                                                    |
| NFR05 | Шифрування        | Вхід у систему має відбуватися через HTTPS.                                                    |
| NFR06 | Доступність       | Система має бути доступною принаймні 90% часу (допускаються невеликі збої).                    |
| NFR07 | Логування         | У випадку помилок система повинна записувати повідомлення з датою та дією, яка викликала збій. |

## 4. Use Case Diagram

### 4.1 Автентифікація
![Use Case Diagram](https://uml.planttext.com/plantuml/svg/XP5FIiD05CRtWTpXaPLPz0gbEq_W0K8xra3Cm6HSYP0sqcu4HII2MozGWiLHctY5xzt8Dub2susRjtc_xy_CcnGsCVRgCWk3J9rRiffCEh-mD4kDcjfKvs4G1idKQaCHNj1n2SSBNdA51XjUHvGKT5OeupalIV9vfeHzGCDBRD7AT7pBDJw5Nt7Dzn5TUzghQ3GQqmcUPLLYoq-zTVbhBaB2fjTLtYS-qBnAG8sFWMg85qCnYTnnTOW-2lY65ftOka7mtITs-EAx-AZAJkZsNsu_djVINVhZdKwrblSVnSFzWRKW-qtXOlyLDtvCL3wJI_m0)

### 4.2 Профіль користувача
![Use Case Diagram](https://uml.planttext.com/plantuml/svg/fP9FIiDG4CRtWTnXoCekUWNf9Jn0Q6CDn0IIvqg4_X7gGX1SABYfHtWchQR6n2jySoFdlIeQfLB89Walt--RR-RD85D-givE8zU9WcD5Ag4q79ye6eLfC5HX4hkEw_X3bQJauG4DJr3ob6SyHmxD2u_yZ8wo83KwW_ZnE0f4knHbmvTyXugl2Qz_KMYFpWrcSEht3-a0Iwn5jaOXr0rgdlCC9Oc-4cAA6YKsXFUVthsMmQF0AsWKdrKi_ez-Hy4MGVAg-GgDg9eThUpbMakmNCY5U9GTZ6oUsVQNJAMB5N8nMzbnTSnaZZRcKzeizZAwj-FGjXUDPn7RH1fFPimlU3DzVfzkhrFpfEroAnNDZLTVob_BeYMAbWXIfMUsxS8y1a4yagt_0000)

### 4.3 Навички
![Use Case Diagram](https://uml.planttext.com/plantuml/svg/dP51IiDG48RtWTnXoCekUWNf9Jn0Q6CDn0IIvqg4ja7gGX14kX14Zn0Q0u-6f5VuvqR-Jmis229k7hoP_vlvPsPIc30tLvU9xoNHkH6JIHxFBupCuZoQcZXBVS_tmgd9SWdmZ5uNiBhKbPROej9r864XPqMKEvr3mdIUHDI-eAB2wXexs42kNTxfP3m-bHCyeSSx2wnW1TstLilHGFUAXYqRr6Zr0PsMNyGMBU4b0vi13Zl4tuYo3Ug3NmVhGZVyD8JlVuKVOJ_Xbkd_-QJ3lTiNXzq9Q_M7rUrPokhjSKKg_ZJvmAzmaaxl2DVeXP4Vzq4rSfk_SSyaIcSyzGS0)

### 4.4 Інтереси та пошук
![Use Case Diagram](https://uml.planttext.com/plantuml/svg/ZLD1Ji9G5DmtwHrUMEc2AnWkuG4Qg4Y2a9IwCYP04s51D366XHjTk71PYmYXKAumxqRERyNm1GEBDktxC_FcJLyhdT0Bmkkhfkiq_OjGmhO4ZVfbABL6u5V3HhlbEgxZLSDs82Ky8zCkvjhJlaP84EkW95v7pZj-O726uhNgJP_OEwoqZodnK-rXBdo91Mig3CbTbEJ6C0nJokKpES4JCdnIiaymFX3ZYzTA8y6A3mdR3h3GwDGYlHGTC46g3mPTq1EaLEgnC1OTsqviWLUIsHMJdu4CtMRO-4VCStnAFuUm-ytzzLCKXfn-WaomsCqOyL53CrkioM6HnsRqxUJomIsPT_CTDe7lQKjVYM1cFjksJgRtjfqt0bCTaTZT2l60ubB7veIMbqUf6H8h5A2PAPQxi_qRq6Qfyj8i3sXajl4tjM8ZyzLodLjpg_Yj6d-ARm00)

### 4.5 Збіги та обміни
![Use Case Diagram](https://uml.planttext.com/plantuml/svg/fLDDIiDG4Du7SeUXArtq2j9BU83GnbgeBQJFbGWcHUf2B8X444IyGHfRZ6sJNc7cHlqcYP26OXKNBtcyV3yptxoqHiRnpEL5txRwxfaXCoIlrpqtrEbvRjlqXWFRiYsdROOUDVY5CxdXH7m9PCmnHp9fa3EYqv7hAKufpg3RTu5zvfc4_C49IS0HSSOpNkCavQH1LufM5ZMR9tJ4hnpnN4BnUGNH02J-nD46NW61KkKVxp3VU86I5Z1QoPHJ6HVi31MYKbv39PMm8iT9JI37VG4r_PFn8uxcSWlkgf3zPydeNRa-M4kLojF3YdV9xxn4GduPVscROtzb4khiT8hOFTHjTes0Yyjv5v5-TvDtXs-9JBGOP5oBz0dODLPSK65vJxo1KaQWeFgAc8phuz1OrLhhdrUZpO_a3asbEgsVHFPVYeYNke5GVYKgD_9WnWT6cUBj5mdlRT2shlNHSWST_6jR)

### 4.6 Відгуки та андміністрування
![Use Case Diagram](https://uml.planttext.com/plantuml/svg/ZP91JeDG48Rt9DpXXPLTz0gc7i83a8ALX482pvKnQMcYxcfSj8aBzGW8D5AmU8N_tSYPfwP1KBk0X3V___qpynZ5qexanTGt3TyzaqA68l8cvr8uNkIEfHS6fc4QzbY6aR3mW4RDKAgvIjG26L9rOmax5gUn6-rLToZmfbROgPLMpaYNi5DhJvoffw7CjuE9xvBbdkG5Se9MA0MfKu7q9-Op4ZiwMLhYaXaSB8R3Ot64DPLgwksMdeGWoFRCGKlvgAuP-wshkaCXRJkUi2791ZbgjQJuHIS1vI4EDVywBdWgR8N-s7AD-iaF2wKfKm8HuCyEddLCgekPle6IT_g2XXcbhjNzkp8DVKd_ZVBEIqE3gdU7HCm6RT9QvzTiwJP5_WQlVVNVBiWqhlWrSWE7Vk0F)
