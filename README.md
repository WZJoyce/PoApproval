# PoApproval

A simplified Purchase Order approval system — a project for modern .NET stack, clean architecture, and TDD.

## Status
🚧 In progress — Solution skeleton up and running.

## Tech Stack
- **Backend**: ASP.NET Core 9, EF Core 9, SQL Server
- **Frontend** *(planned)*: React 18, TypeScript, Vite
- **Testing**: xUnit, FluentAssertions, Moq, WebApplicationFactory
- **Cloud** *(planned)*: Azure App Service, Azure SQL, Application Insights
- **CI/CD** *(planned)*: GitHub Actions

## Architecture
```
src/
├── PoApproval.Api/             # Web API — controllers, middleware
├── PoApproval.Domain/          # Pure business logic — entities, services, no external deps
└── PoApproval.Infrastructure/  # EF Core, persistence

tests/
├── PoApproval.Domain.Tests/    # Fast unit tests
└── PoApproval.Api.Tests/       # Integration tests via WebApplicationFactory
```

## Run locally
```bash
dotnet build
cd src/PoApproval.Api
dotnet run
```
