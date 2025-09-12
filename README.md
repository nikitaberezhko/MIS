MIS (.NET Aspire, ASP.NET Core, PostgreSQL)

Стек
- .NET 9
- .NET Aspire (AppHost, ServiceDefaults, ресурс PostgreSQL)
- ASP.NET Core Minimal API
- EF Core 9 + Npgsql
- PostgreSQL

Запуск
1. Установите .NET SDK 9.
2. Запустите AppHost:
   ```bash
   dotnet run --project MIS.AppHost
   ```
   Aspire поднимет ресурс PostgreSQL и API `MIS.Api`.

Архитектура
- `MIS.Api` — Web API с минимальными эндпоинтами.
- `MIS.AppHost` — конфигурация распределенного приложения Aspire, ресурс `mis-db` (PostgreSQL).
- `MIS.ServiceDefaults` — телеметрия, health, сервис-дискавери по умолчанию.

Сущности домена
- Пациент (`Patient`): ФИО, ДР, пол, контакты, адрес, страховой полис, лечащий врач, МКБ, аудиты.
- Доктор (`Doctor`): ФИО, специальность, номер лицензии и срок, контакты, активность, аудиты.
- Болезнь (`Disease`): код (например, ICD-10), название, описание, признаки хроническая/инфекционная, базовая тяжесть, аудиты.
- Связь (`PatientDisease`): статус диагноза, тяжесть, даты постановки/разрешения, заметки.

Эндпоинты
- GET/POST/PUT/DELETE /api/patients
- GET/POST/PUT/DELETE /api/doctors
- GET/POST/PUT/DELETE /api/diseases
- GET/POST/DELETE /api/patients/{patientId}/diseases

Миграции
- При старте API выполняется Database.Migrate() автоматически.

Health
- GET /healthz

