# Mini-Banking

The core backend for a digital wallet. Users can deposit, withdraw, and transfer money between accounts while keeping operations safe and idempotent.

## Table of contents
- [Goals](#goals)
- [Project Planning (Jira)](#Project-Planning-(Jira))
- [Architecture](#architecture)
- [Features](#features)
- [Requirements](#requirements)
- [Run locally](#run-locally)
- [API usage](#api-usage)

## Goals
- Provide a simple, production-minded backend for a wallet: account creation, deposit, withdraw, and transfer.
- Demonstrate clear layering (API → Application → Domain → Infrastructure), testability, and operational considerations (idempotency, error handling, API versioning, OpenAPI).

## Project Planning (Jira)
This project was managed using Jira to simulate a real software team's workflow. Work was organized into Epics, User Stories, and Tasks to demonstrate backlog refinement, feature decomposition, and incremental delivery

<img width="1813" height="700" alt="image" src="https://github.com/user-attachments/assets/65050ffc-89e8-458e-a36b-a3c65ee143c4" />

linking commits to Jira issues
<img width="1717" height="814" alt="image" src="https://github.com/user-attachments/assets/6f6b1daf-57c7-4d2c-945d-743be3c0910d" />

## Architecture
The codebase is organized into separate projects under `src/backend/`:
- `API/` — ASP.NET Core HTTP API project. Contains middlewares, controllers, OpenAPI exposure.
- `ApplicationService/` — Application-level services (use-cases) and DTOs.
- `DomainLogic/` — Domain models, business rules and domain events.
- `infrastructure/` — Persistence, external integrations, and DI wiring.
- `tests/` — Automated tests (unit/integration as applicable).

Key runtime pieces:
- Each library project organizes its dependency injection registrations by using a registration class.
- `Program.cs` configures services, enables API versioning, applies middlewares (`GlobalException`, `IdempotencyMiddleware`) and maps controllers.
- Domain events are handled via MediatR.

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
