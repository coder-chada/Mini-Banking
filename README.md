# Mini-Banking

The core backend for a digital wallet. Users can deposit, withdraw, and transfer money between accounts while keeping operations safe and idempotent.

## Table of contents
- [Goals](#goals)
- [Architecture](#architecture)
- [Features](#features)
- [Requirements](#requirements)
- [Run locally](#run-locally)
- [API usage](#api-usage)
- [Testing](#testing)
- [Notes for reviewers / maintainers](#notes-for-reviewers--maintainers)
- [License](#license)
- [Contact](#contact)

## Goals
- Provide a simple, production-minded backend for a wallet: account creation, deposit, withdraw, and transfer.
- Demonstrate clear layering (API → Application → Domain → Infrastructure), testability, and operational considerations (idempotency, error handling, API versioning, OpenAPI).

## Architecture
The codebase is organized into separate projects under `src/backend/`:
- `API/` — ASP.NET Core HTTP API project. Contains `Program.cs`, controllers, OpenAPI exposure, and request samples (`API.http`).
- `ApplicationService/` — Application-level services (use-cases) and DTOs. Example: `accounts/Services/AccountService.cs`.
- `DomainLogic/` — Domain models and business rules.
- `infrastructure/` — Persistence, external integrations, and DI wiring.
- `tests/` — Automated tests (unit/integration as applicable).

Key runtime pieces:
- `Program.cs` configures services, registers MediatR handlers, enables API versioning and OpenAPI, applies middlewares (`GlobalException`, `IdempotencyMiddleware`) and maps controllers.
- Commands/queries are handled via MediatR; controllers delegate to application services or MediatR requests.

## Features
- Account lifecycle: create accounts and query balances
- Money operations: deposit, withdraw, transfer between accounts
- Idempotency middleware to guard duplicate/at-least-once requests
- Global exception handling middleware
- API versioning (URL segment and custom header reader)
- OpenAPI (swagger) exposed in development
- Sample HTTP requests in `src/backend/API/API.http`

## Requirements
- .NET SDK 6.0 or later (the project uses the minimal ASP.NET Core hosting model)
- Git

## Run locally
1. Clone the repo:
   ```bash
   git clone https://github.com/coder-chada/Mini-Banking.git
   cd Mini-Banking
