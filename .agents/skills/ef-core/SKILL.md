---
name: ef-core
description: >
  Entity Framework Core guidance for .NET 10 Clean Architecture applications.
  Use when working with DbContext, entity configuration, relationships, migrations,
  LINQ-to-SQL translation, projections, tracking, pagination, concurrency,
  transactions, value converters, bulk operations, or persistence performance.
---

# Entity Framework Core

## Core Principles

1. **Use EF Core when it is the selected persistence technology.** Do not introduce another ORM or data-access library without a concrete requirement.
2. **Treat `DbContext` as a unit of work.** Do not add a generic repository layer that merely mirrors `DbSet<T>` operations.
3. **Keep EF Core implementation details in Infrastructure.**
4. **Project only the data you need.** Prefer server-side filtering and projection for read models.
5. **Use a scoped `DbContext` for normal ASP.NET Core request processing.**
6. **Treat migrations as reviewed source code.** Inspect generated operations and production impact.

## DbContext Design

Keep the context focused and configure it through dependency injection.

```csharp
internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

Use separate `IEntityTypeConfiguration<T>` classes when entity mappings are non-trivial.

Use `IDbContextFactory<TContext>` only when the lifetime/use case actually requires contexts outside the normal request scope, such as selected background, desktop, or parallel scenarios.

## Infrastructure Registration

Register EF Core through `InfrastructureServicesRegistration`:

```csharp
public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
```

The database provider is project-specific. Do not assume SQL Server when another provider is configured.

## Entity and Relationship Design

- Choose keys deliberately; surrogate keys are common, but domain requirements may justify other choices.
- Configure one-to-one, one-to-many, and many-to-many relationships explicitly when conventions are not sufficient.
- Keep navigation properties intentional; do not add large navigation graphs merely for convenience.
- Use fluent configuration for important constraints, indexes, precision, conversions, and relationship behavior.
- Consider owned/complex/value conversion mappings when they naturally represent domain value concepts.
- Configure delete behavior deliberately rather than relying on accidental cascade behavior.

## Querying

### Read-only Queries

Use `AsNoTracking()` when the returned entities will not be modified through the current context.

```csharp
var orders = await dbContext.Orders
    .AsNoTracking()
    .Where(x => x.CustomerId == customerId)
    .Select(x => new OrderSummary(x.Id, x.Total))
    .ToListAsync(cancellationToken);
```

### Projection

Prefer `Select(...)` to load only fields required by the use case.

### Pagination

Paginate potentially large result sets. Apply a deterministic ordering before `Skip`/`Take`.

For very large or frequently paged datasets, consider keyset/seek pagination when it better fits the access pattern.

### Related Data and N+1

Load related data deliberately.

- Use projection when the API needs a shaped read model.
- Use `Include` when materialized related entities are genuinely required.
- Watch for N+1 query patterns.
- Do not enable lazy loading by default.

### IQueryable

Keep database-translatable operations in the query before materialization. Understand when execution occurs.

Do not expose `IQueryable` across architectural boundaries as a generic persistence abstraction.

### Raw SQL

Prefer strongly typed LINQ when it expresses the query clearly and translates efficiently. Use parameterized raw SQL for justified cases where it provides a concrete benefit or required capability.

Never concatenate untrusted values into SQL.

## Change Tracking and Saving

- Track entities only when changes will be persisted.
- Avoid unnecessary repeated `SaveChangesAsync` calls when one unit of work can commit the required changes safely.
- Propagate `CancellationToken`.
- Use explicit transactions when multiple operations require a transaction boundary beyond EF Core's normal single-save behavior.
- Design for optimistic concurrency where concurrent updates can conflict.

## Bulk Operations

Consider `ExecuteUpdateAsync` and `ExecuteDeleteAsync` when many rows can be changed safely without materializing entities.

Remember that bulk operations bypass normal tracked-entity change processing.

## Value Converters

Use value converters when a domain value has a clear, stable scalar/database representation. Keep conversion rules deterministic.

## Interceptors

Consider interceptors for appropriate cross-cutting persistence concerns such as auditing. Prefer explicit application logic when it is simpler or more transparent.

## Migrations

Generate migrations from Infrastructure with Api as startup project:

```bash
dotnet ef migrations add AddOrderIndex --project src/SupportTicketPlatform.Infrastructure --startup-project src/SupportTicketPlatform.Api
```

Before applying a migration:

- review generated operations;
- assess data-loss and locking risks;
- verify non-nullable changes against existing rows;
- inspect indexes/constraints/defaults;
- check that renames were not accidentally generated as drop-and-create operations;
- generate/review SQL or other deployment artifacts when the environment requires it.

Keep migrations small and descriptively named where practical.

Do not edit an already-applied migration to rewrite production history; create a new migration.

Migration bundles may be considered when they fit the deployment model, but they are not mandatory.

## Anti-patterns

### Generic Repository Wrapper

Do not create a generic repository that only mirrors `DbSet<T>` operations.

When Application needs persistence access, define a contract appropriate to the application boundary. Keep the EF Core implementation in Infrastructure.

### Early Materialization

```csharp
var all = await dbContext.Orders.ToListAsync(cancellationToken);
var active = all.Where(x => x.IsActive);
```

Keep translatable filtering, ordering, and projection in the database query whenever practical.

### Lazy Loading by Default

Avoid making lazy loading the default. Hidden database calls make behavior and performance difficult to reason about.

### N+1 Queries

Do not execute one query for a parent set and then one additional database query per row when the same result can be retrieved efficiently in a bounded query shape.

### Long-lived DbContext

Do not keep a normal web `DbContext` alive beyond its intended scope or share it across concurrent operations.

### Missing Await/Cancellation

Await EF Core async APIs and propagate cancellation tokens.

## Decision Guide

| Scenario | Recommendation |
|---|---|
| Read-only query | `AsNoTracking()` plus projection when appropriate |
| Large result set | Server-side pagination with deterministic ordering |
| Related data for DTO/read model | Prefer projection; use `Include` when entity graph materialization is required |
| Bulk update/delete | Consider `ExecuteUpdateAsync` / `ExecuteDeleteAsync` |
| Concurrent edits | Consider optimistic concurrency tokens and conflict handling |
| Multiple operations requiring one atomic boundary | Explicit transaction when needed |
| Strongly typed value persisted as scalar | Value converter |
| Complex reporting | First consider optimized EF projection or parameterized raw SQL; add another data-access library only when justified |
| Audit/cross-cutting persistence concern | Consider interceptor |
| Production schema change | Generate, review, and deploy migration artifacts deliberately |
