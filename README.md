# GameStore

Веб-приложение для управления каталогом компьютерных игр: каталог с фильтрацией и пагинацией, корзина покупок, административная панель для CRUD-операций и аутентификация через Keycloak (OpenID Connect).

Построено по принципам **Clean Architecture** (.NET 9)

## Стек технологий

**Бэкенд**
- ASP.NET Core 9 — Web API
- Entity Framework Core 9 + Npgsql — доступ к данным, PostgreSQL как СУБД
- MediatR — CQRS (Commands/Queries/Handlers)
- FluentValidation — валидация команд/запросов
- HybridCache + StackExchange.Redis — двухуровневое кэширование (in-memory L1 + Redis L2)
- JWT Bearer / OpenID Connect — аутентификация через Keycloak
- Scalar (OpenAPI) — интерактивная документация API

**Фронтенд**
- ASP.NET Core MVC — каталог игр и сессионная корзина
- Razor Pages (`Areas/Admin`) — административная панель

**Инфраструктура**
- PostgreSQL — основная база данных
- Redis — кэш
- Keycloak — Identity Provider (OIDC)
- Docker Compose — оркестрация всего стека для локального запуска

## Архитектура и разделение функционала

```
GameStore.Domain          доменные сущности (Game, Genre), без зависимостей
        ↑
GameStore.Application     CQRS через MediatR, DTO, валидация, абстракции
        ↑                 (IUserContext, ICacheService, IFileService)
GameStore.Infrastructure  EF Core, миграции, FileService, HybridCacheService,
        ↑                 KeycloakRoleParser, DbInitializer
        │
GameStore.API             REST API, JWT Bearer, тонкие контроллеры (MediatR.Send),
                           Scalar/OpenAPI

GameStore.Domain ← GameStore.Application
                          ↑
                   GameStore.UI          MVC (каталог + корзина) и Razor Pages
                                          (админка); ходит в API только по HTTP
```

### Роли и доступ (Keycloak)

Realm `GameStore`, клиент `game-store-ui`. Роли:

| Роль | Назначение |
|---|---|
| `user-game-store` | Стандартная роль (выдаётся автоматически при регистрации) — доступ к корзине |
| `admin-game-store` | Назначается вручную в Keycloak — доступ к админ-панели и записи в API |

## Запуск в Docker

Понадобится Docker и Docker Compose.

1. Скопировать пример переменных окружения:
   ```bash
   cp .env.example .env
   ```
   При необходимости отредактировать значения (см. раздел ниже).

2. Поднять весь стек:
   ```bash
   docker compose up --build
   ```

3. После старта будут доступны:

   | Сервис | URL |
   |---|---|
   | UI (каталог, корзина, админка) | http://localhost:5001 |
   | API + Scalar-документация | http://localhost:5002/scalar/v1 |
   | Keycloak Admin Console | http://localhost:8080 |
   | PostgreSQL | localhost:5432 |
   | Redis | localhost:6379 |

4. Зарегистрировать аккаунт через UI (Sign up). Чтобы получить доступ к админ-панели:
   - зайти в Keycloak Admin Console (логин/пароль — `KC_ADMIN`/`KC_ADMIN_PASSWORD` из `.env`);
   - в realm `GameStore` найти созданного пользователя и назначить ему роль `admin-game-store` (Role mapping → Assign role).

Полный чистый рестарт (со сбросом данных БД):
```bash
docker compose down -v
docker compose up -d --build
```

## Пример .env-файла

Реальный файл `.env` не коммитится — используйте `.env.example` как шаблон:

```env
# ---- PostgreSQL ----
POSTGRES_DB=gamestore
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# ---- Keycloak bootstrap admin (Admin Console: http://localhost:8080) ----
KC_ADMIN=admin
KC_ADMIN_PASSWORD=admin123

# ---- Keycloak client secret for game-store-ui ----
# Must match the "secret" field of the game-store-ui client in
# docker/keycloak/GameStore-realm.json.
KEYCLOAK_UI_CLIENT_SECRET=dev-game-store-ui-secret
```

## Локальный запуск без Docker (опционально)

Для разработки одного из сервисов (например, `GameStore.API`) без пересборки всего стека:

```bash
dotnet build GameStore.sln
dotnet run --project GameStore.API
dotnet run --project GameStore.UI
```

В этом режиме PostgreSQL/Redis/Keycloak всё равно нужно поднять (например, через `docker compose up -d postgres redis keycloak`), а строки подключения для `GameStore.API`/`GameStore.UI` задаются через User Secrets, а не `appsettings.json`.

Добавление новой EF Core миграции:
```bash
dotnet ef migrations add <Name> --project GameStore.Infrastructure --startup-project GameStore.API
```

