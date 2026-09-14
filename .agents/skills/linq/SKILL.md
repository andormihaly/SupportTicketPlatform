---
name: linq
description: >
  C# LINQ guidance for querying and transforming collections and IQueryable
  sources. Use when implementing, reviewing, debugging, or optimizing LINQ
  queries involving IEnumerable, IQueryable, deferred execution, materialization,
  filtering, projection, grouping, joins, ordering, pagination, expression trees,
  or LINQ performance.
---

# C# LINQ

## Core Principles

- Prefer readable LINQ over clever or overly compressed query chains.
- Understand whether the current source is `IEnumerable<T>` or `IQueryable<T>` before composing a query.
- Keep filtering and projection server-side when working with `IQueryable<T>` whenever the provider can translate the query.
- Materialize only when a concrete snapshot or in-memory operation is required.
- Avoid multiple enumeration when the source is expensive, remote, stateful, or has side effects.
- Optimize LINQ allocations only when profiling or benchmarking shows a meaningful hot-path problem.

## IEnumerable vs IQueryable

`IEnumerable<T>` represents in-memory enumeration. `IQueryable<T>` builds an expression tree that a provider such as EF Core may translate to another query language such as SQL.

```csharp
IQueryable<Order> query = dbContext.Orders
    .Where(o => o.CreatedAt >= cutoff)
    .OrderBy(o => o.CreatedAt);

var orders = await query.ToListAsync(cancellationToken);
```

Be careful with operations that switch to client-side evaluation:

```csharp
var orders = dbContext.Orders
    .AsEnumerable()
    .Where(o => ExpensiveInMemoryCheck(o))
    .ToList();
```

After `AsEnumerable()`, subsequent LINQ operators execute in application memory.

Do not expose `IQueryable<T>` across architectural boundaries as a generic persistence abstraction.

## Deferred Execution

Most LINQ operators are deferred. A query is often not executed until it is enumerated or materialized.

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

var query = numbers.Where(n => n > 2);

numbers.Add(6);

var result = query.ToList(); // 3, 4, 5, 6
```

Use `ToList()`, `ToArray()`, or another materializer when a stable snapshot is required.

Be aware that repeated enumeration can re-run the underlying work.

```csharp
var query = GetExpensiveItems().Where(x => x.IsActive);

var count = query.Count();
var total = query.Sum(x => x.Amount);
```

If the source is expensive and both values are needed, materialize once:

```csharp
var items = GetExpensiveItems()
    .Where(x => x.IsActive)
    .ToList();

var count = items.Count;
var total = items.Sum(x => x.Amount);
```

## Materialization

Common terminal/materializing operations include:

- `ToList()`
- `ToArray()`
- `ToDictionary()`
- `Count()`
- `Any()`
- `First()` / `FirstOrDefault()`
- `Single()` / `SingleOrDefault()`
- enumeration with `foreach`

For `IQueryable<T>`, these operations usually trigger provider execution.

Avoid premature materialization:

```csharp
// Avoid
var products = await dbContext.Products.ToListAsync(cancellationToken);

return products
    .Where(p => p.Price > 100)
    .Select(p => new ProductDto(p.Id, p.Name))
    .ToList();
```

Prefer composing before execution:

```csharp
return await dbContext.Products
    .Where(p => p.Price > 100)
    .Select(p => new ProductDto(p.Id, p.Name))
    .ToListAsync(cancellationToken);
```

## Query Composition

### Filtering

```csharp
var active = products.Where(p => p.IsActive);
```

Prefer predicates that are easy to understand and, for `IQueryable<T>`, easy for the provider to translate.

### Projection

```csharp
var summaries = orders.Select(o => new OrderSummary(o.Id, o.Total));
```

Project only the data needed by the current use case, especially for database-backed queries.

### Flattening

```csharp
var items = orders.SelectMany(o => o.OrderLines);
```

Use `SelectMany` when flattening nested sequences.

### Grouping

```csharp
var totals = sales
    .GroupBy(s => s.Category)
    .Select(g => new
    {
        Category = g.Key,
        Count = g.Count(),
        Total = g.Sum(x => x.Amount)
    });
```

### Joining

Query syntax can be clearer for multi-source joins:

```csharp
var result =
    from customer in customers
    join order in orders on customer.Id equals order.CustomerId
    select new
    {
        customer.Name,
        order.Id
    };
```

Method syntax is equally valid when it is clearer for the team and query shape.

## Core Operators

### Existence

Prefer `Any()` when checking whether at least one element exists.

```csharp
var hasActive = products.Any(p => p.IsActive);
```

Avoid `Count() > 0` when only existence matters.

### First vs Single

Use `First` / `FirstOrDefault` when any first match is acceptable.

Use `Single` / `SingleOrDefault` only when uniqueness is part of the contract and multiple matches should be considered an error.

### Set Operations

Use built-in operations such as:

- `Distinct` / `DistinctBy`
- `Union`
- `Intersect`
- `Except`
- `Concat`

Prefer built-in operators over recreating custom versions without a concrete need.

### Ordering

Use `OrderBy` / `OrderByDescending` and `ThenBy` / `ThenByDescending` deliberately.

For pagination, always use deterministic ordering.

## Pagination

Offset pagination is simple and often sufficient:

```csharp
var page = await dbContext.Orders
    .OrderBy(o => o.Id)
    .Skip(pageNumber * pageSize)
    .Take(pageSize)
    .ToListAsync(cancellationToken);
```

For deep or high-frequency pagination, consider keyset/seek pagination when it fits the access pattern:

```csharp
var page = await dbContext.Orders
    .Where(o => o.Id > lastSeenId)
    .OrderBy(o => o.Id)
    .Take(pageSize)
    .ToListAsync(cancellationToken);
```

## Expression Trees

A lambda may be compiled either as a delegate or represented as an expression tree:

```csharp
Func<int, bool> predicate = x => x > 10;

Expression<Func<int, bool>> expression = x => x > 10;
```

`IQueryable<T>` providers inspect expression trees to translate queries.

When building dynamic expressions, keep them simple and provider-translatable. Avoid constructing expression trees when ordinary query composition is sufficient.

## Performance

### Multiple Enumeration

Repeated enumeration can repeat CPU work, database queries, or other side effects. Materialize once when the result will be consumed multiple times and the source is expensive.

### Server vs Client Evaluation

For `IQueryable<T>` sources:

- filter before materialization;
- project before materialization;
- avoid `AsEnumerable()` before database-friendly operations;
- avoid calling arbitrary C# methods inside provider-translated predicates unless translation support is known.

### LINQ-to-Objects Allocations

LINQ operators can allocate iterators, delegates, and intermediate collections.

For most business logic, this overhead is negligible.

Only consider replacing LINQ with manual loops when:

- the code is a measured hot path;
- allocation/GC pressure is demonstrated;
- the readability trade-off is justified.

Do not micro-optimize LINQ by default.

## Anti-patterns

- Do not call `ToList()` or `ToArray()` earlier than needed.
- Do not enumerate an expensive deferred query multiple times unintentionally.
- Do not switch from `IQueryable<T>` to `IEnumerable<T>` before filtering/projection unless client-side evaluation is intentional.
- Do not expose `IQueryable<T>` across architectural boundaries.
- Do not use `Count() > 0` when `Any()` expresses the intent.
- Do not use `Single()` when multiple matches are valid.
- Do not create custom LINQ operators that duplicate built-in .NET functionality.
- Do not replace readable LINQ with manual loops without a measured reason.
- Do not assume every C# method used in an `IQueryable<T>` predicate can be translated by the query provider.
- Do not add compiled-query logic as a default LINQ optimization.

## Decision Guide

| Scenario | Recommendation |
|---|---|
| Querying an in-memory collection | `IEnumerable<T>` LINQ |
| Building an EF/database query | Keep it as `IQueryable<T>` until execution |
| Need only existence | `Any()` |
| Need exactly one result by contract | `Single` / `SingleOrDefault` |
| Need a stable snapshot | Materialize with `ToList()` / `ToArray()` |
| Same expensive result consumed repeatedly | Materialize once |
| Need a DTO/read model from EF | Project with `Select` before materialization |
| Deep database pagination | Consider keyset/seek pagination |
| Dynamic provider query | Compose expressions carefully and verify translation |
| Suspected LINQ allocation problem | Measure first; optimize only if meaningful |
