# FinTrackAPI

A production-grade personal finance REST API built with **ASP.NET Core 10**, **Entity Framework Core**, **SQL Server**, and **Microsoft Azure**. Designed to demonstrate real-world backend engineering patterns used in fintech and banking systems.

**Live API:** http://20.68.108.61

---

## Features

- **JWT Authentication** — secure user registration and login with BCrypt password hashing and JWT token generation
- **Account Management** — create and manage Current and Savings accounts with real-time balance tracking
- **Transaction Processing** — credit, debit, and transfer operations with transactional integrity and insufficient funds protection
- **Filtered Transaction History** — query transactions by type, date range, and amount
- **Background Job Processing** — Hangfire-powered monthly report generation running on a cron schedule
- **Hangfire Dashboard** — live job monitoring at `/hangfire`
- **Swagger UI** — full API documentation with JWT Bearer auth support
- **CI/CD Pipeline** — GitHub Actions workflow deploying on every push to `main`
- **Dockerised** — containerised with Docker, deployed on Azure VM via Azure Container Registry

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (Azure SQL) |
| Authentication | JWT Bearer + BCrypt |
| Background Jobs | Hangfire + Hangfire.SqlServer |
| API Docs | Swashbuckle / Swagger UI |
| Containerisation | Docker |
| Registry | Azure Container Registry |
| Hosting | Azure VM (Ubuntu 24.04) |
| CI/CD | GitHub Actions |

---

## Architecture

```
Client
  │
  ▼
Swagger UI / API Consumer
  │
  ▼
ASP.NET Core 10 API (Docker Container)
  │
  ├── Auth Controller        → Register, Login, JWT generation
  ├── Accounts Controller    → CRUD accounts, balance tracking
  ├── Transactions Controller → Debit, Credit, Transfer, Filtering
  │
  ├── Hangfire Server        → Background job processing
  │     └── ReportService    → Monthly financial summary generator
  │
  └── AppDbContext (EF Core)
        │
        ▼
      Azure SQL Database
        ├── Users
        ├── Accounts
        ├── Transactions
        └── MonthlyReports
```

---

## API Endpoints

### Auth
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### Accounts
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/accounts` | Create a new account |
| GET | `/api/accounts` | List all accounts for authenticated user |
| GET | `/api/accounts/{id}` | Get account details and balance |

### Transactions
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/transactions` | Record a new transaction |
| GET | `/api/transactions` | List transactions with optional filters |
| GET | `/api/transactions/{id}` | Get single transaction |

**Transaction filters:** `type`, `from`, `to`, `minAmount`, `maxAmount`

---

## Key Engineering Decisions

**Transactional integrity** — balance updates and transaction records are written atomically. A failed transaction never leaves accounts in an inconsistent state.

**JWT with claims** — tokens embed user ID, email, and name as claims. All protected endpoints extract the user ID from the token rather than accepting it as a parameter, preventing users from accessing other users' data.

**Idiomatic EF Core** — relationships defined via Fluent API with explicit cascade delete rules and decimal precision for all monetary fields.

**Background processing** — Hangfire uses SQL Server as its backing store, ensuring jobs survive container restarts and are not lost on failure.

---

## Author

**Kaushal Bakrania** — Backend Software Engineer  
[LinkedIn](https://linkedin.com/in/kaushal-bakrania) · [GitHub](https://github.com/kaushal-bakrania)
