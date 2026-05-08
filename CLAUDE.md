# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Full-stack TODO List application built for an internship at Embrace-It. The app manages tasks with categories, tags, subtasks, and 5 statuses (Non Started, In Progress, Paused, Late, Finished). Users authenticate via JWT.

All code, comments, and documentation must be written in **English**.

## Tech Stack

**Frontend:** Angular (latest LTS), standalone components, NGXS, RxJS, PrimeNG, Reactive Forms  
**Backend:** .NET 10, Minimal API + Controller-Based, Layered Architecture, Entity Framework Core  
**Database:** PostgreSQL (dev) — originally specified as SQL Server  
**Auth:** ASP.NET Identity + JWT

## Project Structure

```
TaskApp/
├── TaskApp.API/                  # Entry point — HTTP endpoints, middleware, DI wiring
├── TaskApp.Application/          # Business logic — services, DTOs, interfaces
├── TaskApp.Domain/               # Core domain — entities, enums (no external dependencies)
│   ├── Entities/                 # AppUser, AppTask, Category, Tag, SubTask
│   ├── Enums/                    # TaskStatus enum
│   └── Interfaces/               # Repository interfaces (to be implemented in Infrastructure)
├── TaskApp.Infrastructure/       # External concerns — EF Core, repositories, JWT
│   └── AppDbContext.cs           # IdentityDbContext<AppUser> with all DbSets
└── frontend/                     # Angular app (not yet created)
    └── src/app/
        ├── core/                 # Interceptors, Guards, singleton services
        ├── shared/               # Reusable standalone components, pipes, directives
        ├── features/             # Feature folders: auth, tasks, categories, tags
        └── store/                # NGXS state definitions
```

## Current State (as of 2026-04-27)

- Domain layer complete: all entities and TaskStatus enum created
- Infrastructure layer: AppDbContext created, migrations not yet run
- Application and API layers: empty
- Frontend: not started
- PostgreSQL database: `taskapp` db, user `rubiales`, host `localhost`

## Backend Commands

```bash
# Run from TaskApp/ root
dotnet build
dotnet run --project TaskApp.API
dotnet ef migrations add <Name> --project TaskApp.Infrastructure --startup-project TaskApp.API
dotnet ef database update --project TaskApp.Infrastructure --startup-project TaskApp.API
```

## Frontend Commands

```bash
cd frontend
npm install
ng serve             # http://localhost:4200
ng build
```

## Key Architecture Decisions

### Backend — Layered Architecture
```
TaskApp.API → TaskApp.Application → TaskApp.Domain ← TaskApp.Infrastructure
```
- **Domain** has zero external dependencies — only plain C# classes
- **Infrastructure** implements what Domain defines (repositories, DbContext)
- **Application** contains business logic services; knows Domain, not Infrastructure directly
- **API** wires everything together in `Program.cs` via Dependency Injection

### Entity relationships
- `AppUser` (IdentityUser) → many `AppTask`, many `Category`, many `Tag`
- `AppTask` → optional `Category`, many-to-many `Tag`, many `SubTask`
- `SubTask` belongs to one `AppTask`
- `AppTask` has `TaskStatus` enum: NonStarted, InProgress, Paused, Late, Finished

### AppDbContext
- Inherits from `IdentityDbContext<AppUser>` (not plain DbContext) to include Identity tables
- DbSets: `AppTasks`, `Categories`, `Tags`, `SubTasks`
- Entity configurations will use separate `IEntityTypeConfiguration<T>` classes (not inline in OnModelCreating)

### Frontend
- All components are **standalone** (no NgModules)
- State via **NGXS** — components dispatch Actions, never call services directly for stored data
- **Interceptor** attaches JWT token to every HTTP request
- **Guards** protect routes by checking NGXS auth state
- **Reactive Forms** exclusively (no template-driven forms)
- UI components from **PrimeNG**

## Student Context

Jorge is an internship student. Background: C# console apps and basic HTML. Angular and TypeScript are new. Bridge explanations from C# concepts to Angular/TypeScript equivalents. Do not write code for the student unless asked — guide what properties/methods to create and review after.
