---
name: cloud-design-patterns
description: >
  Cloud and distributed-system design pattern guidance. Use when designing or
  reviewing reliability, scalability, messaging, data, performance, security,
  or distributed workflow behavior.
---

# Cloud Design Patterns

## Core Principles
- Start with the problem, not the pattern name.
- Introduce a pattern only when its trade-offs solve a concrete requirement.
- Prefer the simplest architecture that satisfies reliability and scale needs.
- Evaluate operational complexity introduced by every distributed pattern.

## Common Pattern Areas
- Reliability: retry, circuit breaker, bulkhead, health endpoint monitoring.
- Messaging: competing consumers, queue-based load leveling, publisher/subscriber.
- Data: cache-aside, CQRS where justified, materialized views.
- Integration: anti-corruption layer, gateway aggregation/routing.
- Resilience: timeout, throttling, backpressure, idempotency.

## Anti-patterns
- Do not introduce CQRS, event sourcing, sagas, or distributed messaging without a concrete need.
- Do not stack retries at multiple layers unknowingly.
- Do not use patterns as architectural decoration.
