---
name: caching
description: >
  Caching guidance for .NET backend applications. Use when adding or reviewing
      application caching, cache keys, invalidation, TTLs, stampede prevention,
      HybridCache, distributed caching, or cache consistency.
---

# Caching

## Core Principles
- Add caching only when there is a concrete performance or availability benefit.
- Prefer `HybridCache` when application caching is required and its capabilities fit the scenario.
- Design cache keys, expiration, and invalidation deliberately.
- Treat cached data as potentially stale.

## Patterns
- Cache read-heavy data with a clear freshness tolerance.
- Include tenant/user/scope identity in keys when data isolation requires it.
- Keep serialization formats stable for distributed caches.
- Measure hit rate and latency before assuming caching helps.

## Anti-patterns
- Do not cache sensitive/user-specific data under shared keys.
- Do not cache failures indefinitely.
- Do not use unbounded keys or TTLs.
- Do not add cache invalidation complexity without measurable value.

## Decision Guide
| Scenario | Guidance |
|---|---|
| Expensive read, tolerates short staleness | Good caching candidate |
| Strongly consistent write/read flow | Be cautious with caching |
| Multi-instance app | Prefer a design that supports distributed/shared behavior |
