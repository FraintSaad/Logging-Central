# 📘 Technical Documentation: LogsCentral (Log Analysis & Alerting System)

## 1. Overview

**LogsCentral** is an ASP.NET Core MVC application designed for:

* Collecting and analyzing logs from various systems.
* Displaying logs with filtering, sorting, and pagination.
* Defining **alert rules** (thresholds based on log levels within a lookback period).
* Sending email alerts when rules are triggered.
* Running background jobs to evaluate alert rules against logs.

It is built with:

* **.NET 7/8 (ASP.NET Core MVC)**
* **Entity Framework Core** (SQL Server backend)
* **Serilog** (logging)
* **Bootstrap** (UI styling)
* **Basic Authentication middleware** (production mode)

---

## 2. Architecture

The app follows **MVC architecture**:

* **Model layer**: EF Core DbContext, ViewModels, DTOs.
* **View layer**: Razor views with Bootstrap.
* **Controller layer**: REST-like endpoints (`/logs`, `/alert-rules`).
* **Background jobs**: Hosted services for alert processing.
* **Services**: Email service, alert evaluation, template rendering.

### Key namespaces:

* `LogsCentral.Models` → Data models
* `LogsCentral.ViewModels` → Page view models
* `LogsCentral.Services` → Business services (e.g., email, alert generation)
* `LogsCentral.Jobs` → Hosted background jobs
* `Data.Context` → EF Core DbContext

---

## 3. Components

### Program.cs

* Configures services and middleware.
* Registers:

  * MVC controllers/views
  * DbContext (`LogsDbContext`)
  * Hosted background job (`AlertsBackgroundJob`)
  * Custom services (`AlertService`, `AzureEmailService`, `AlertEmailGenerator`).
* Configures logging via Serilog.
* Sets up **Basic Authentication middleware** (in non-development).

### Controllers

1. **LogsController**

   * Route: `/logs`
   * Features:

     * Filter by log level, time range, timezone.
     * Pagination and sorting.
     * Persists timezone preference in cookies.

2. **AlertRulesController**

   * Route: `/alert-rules`
   * Features:

     * List alert rules.
     * Create, edit, delete rules.
     * Validate input (email, thresholds, periods).
     * CSRF-protected via AntiForgery tokens.

### ViewModels

* **LogsViewModel**: Encapsulates log filtering, pagination, sorting.
* **LogsPageModel**: Represents log data for UI.
* **AlertRulesPageViewModel**: Handles alert rule CRUD.
* **AlertRuleViewModel**: Single alert rule with properties like level, threshold, recipients.
* **StatusPageViewModel** / **LivePageViewModel**: For system health and real-time views.

### Services

* **AlertService**: Evaluates rules against logs.
* **AlertEmailGenerator**: Generates alert email content.
* **AzureEmailService** (implements `IEmailService`): Sends alert emails via Azure.

### Background Jobs

* **AlertsBackgroundJob**

  * Hosted service running periodically.
  * Checks log data against defined rules.
  * Fires alerts (saves to DB + triggers emails).

---

## 4. Data Flow

1. **Logs ingestion**

   * Logs are written into SQL Server (via Serilog sink).
   * `LogsDbContext` exposes `SerilogEvents`.

2. **Log analysis**

   * User visits `/logs`.
   * Controller applies filters, loads logs, passes `LogsPageModel` to view.
   * Razor view renders logs with Bootstrap UI.

3. **Alerting**

   * User defines rules at `/alert-rules`.
   * Rules stored in DB.
   * Background job queries logs in the lookback window.
   * If log count exceeds threshold → alert email sent.

---

## 5. Authentication & Security

* **Development**: No authentication required.
* **Production**:

  * Basic Auth middleware is injected.
  * Configurable via `AuthUsername` and `AuthPassword` (from environment variables).
  * Unauthorized requests return `401 Unauthorized` with `WWW-Authenticate: Basic realm="LogsCentral"`.
* **CSRF Protection**: Razor forms use `@Html.AntiForgeryToken()`.

---

## 6. Views / UI

### Logs View (`Views/Logs/Index.cshtml`)

* Filtering:

  * Checkboxes for log levels (Info, Warning, Error).
  * Time range pickers (`datetime-local` inputs).
  * Timezone selector.
* Display:

  * Paginated table of logs with timestamp, level badge, message.
  * Sorting by Time, Level, Message.
* Pagination:

  * Compact page navigation with ellipsis.

### Alert Rules View (`Views/AlertRules/Index.cshtml`)

* Rules table with actions (edit, delete).
* Modals:

  * **Add Rule** modal (select log level, threshold, lookback, recipients).
  * **Edit Rule** modal (pre-filled via `data-*` attributes).
* UI powered by Bootstrap 5 (badges, forms, buttons, modal dialogs).

---

## 7. Database

### Tables (via EF Migrations)

* **SerilogEvents** → Logs table (ingested by Serilog sink).
* **AlertRules** → Stores user-defined alert rules.
* **FiredAlerts** → Stores triggered alerts for audit/history.

### Example schema (simplified)

```sql
CREATE TABLE SerilogEvents (
    Id INT PRIMARY KEY IDENTITY,
    Timestamp DATETIMEOFFSET NOT NULL,
    Level NVARCHAR(20),
    Message NVARCHAR(MAX)
);

CREATE TABLE AlertRules (
    Id INT PRIMARY KEY IDENTITY,
    LogLevel NVARCHAR(20),
    LookbackPeriod INT,
    Threshold INT,
    Recipients NVARCHAR(MAX),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE FiredAlerts (
    Id INT PRIMARY KEY IDENTITY,
    RuleId INT FOREIGN KEY REFERENCES AlertRules(Id),
    FiredAt DATETIME NOT NULL,
    EmailSent BIT NOT NULL
);
```

The project contains EF Migrations, so intially the DB structure is created automatically

---

## 8. Deployment Considerations

* **Environment Variables**:

  * `DatabaseConnectionString`
  * `AuthUsername` / `AuthPassword`
* **Logging**:

  * Console logs in dev.
  * Daily rolling log file in production (`logs.txt`).
* **Database Migration**:

  * Runs `dbContext.Database.Migrate()` on startup.
* **HTTPS & HSTS**:

  * Enabled in production.
* **Scaling**:

  * Background job ensures periodic alert checks (cron-like).
  * Can scale web layer independently from background jobs if needed.