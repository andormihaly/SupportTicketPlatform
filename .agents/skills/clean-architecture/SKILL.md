---
name: clean-architecture
description: >
  Clean Architecture guidance for .NET backend applications using Domain,
  Application, Infrastructure, and Api projects. Use when implementing,
  reviewing, or refactoring code where layer responsibilities, dependency
  direction, use cases, persistence boundaries, or architectural placement
  are relevant.
---

# Clean Architecture

## Core Principles

1. **Dependencies point inward.** `Domain` has no project references. `Application` references only `Domain`. `Infrastructure` references only `Application`. `Api` references `Application` and `Infrastructure`.
2. **Keep framework concerns out of Domain.** Domain code should model business concepts and rules without ASP.NET Core, EF Core, Azure SDK, or infrastructure dependencies.
3. **Organize application behavior around cohesive use cases.** Do not introduce CQRS, mediator libraries, command/query abstractions, or additional patterns unless the project needs them.
4. **Infrastructure is a plugin.** Persistence, external APIs, messaging, file storage, and similar concerns live in Infrastructure and implement contracts exposed by Application.
5. **Api is the composition root.** Controllers translate HTTP concerns into application calls and map results back to HTTP responses. Keep business logic out of controllers.

## Project Layout

```text
src/
├── SupportTicketPlatform.Api/
│   └── Controllers/
├── SupportTicketPlatform.Application/
│   ├── Configuration/
│   ├── Core/
│   ├── Features/
│   └── Interfaces/
├── SupportTicketPlatform.Domain/
│   └── Common/
└── SupportTicketPlatform.Infrastructure/
    ├── Persistence/
    └── Services/
```

## Patterns

### Application Contract, Infrastructure Implementation

Define a contract in Application when application behavior needs an external capability. Implement that contract in Infrastructure.

```csharp
public interface IClock
{
    DateTimeOffset GetUtcNow();
}
```

Infrastructure supplies the implementation and registers it through `InfrastructureServicesRegistration`.

### Thin Controllers

```csharp
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        // Delegate application behavior to the Application layer.
        return Ok();
    }
}
```

Controllers should handle routing, HTTP-specific validation/binding, authorization intent, and response mapping—not business rules.

### Service Registration

Use the existing registration classes:

```csharp
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
```

## Anti-patterns

- Do not reference Infrastructure or Api from Domain or Application.
- Do not place EF Core, HTTP clients, Azure SDK clients, or file-system code in Domain.
- Do not place business logic in controllers.
- Do not introduce a generic repository abstraction that simply mirrors `DbSet<T>` operations.
- Do not introduce architecture patterns only for future possibilities.

## Decision Guide

| Question | Guidance |
|---|---|
| Where does a business rule belong? | Domain when it is intrinsic domain behavior; otherwise Application when it coordinates a use case. |
| Where does an external service implementation belong? | Infrastructure. |
| Where does the interface for an external capability belong? | Application when Application depends on that capability. |
| Where does HTTP-specific behavior belong? | Api. |
| Should a new abstraction be introduced? | Only when it solves a concrete current requirement. |
