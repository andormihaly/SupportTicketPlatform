---
name: resilience
description: >
  Resilience guidance for .NET outbound dependencies. Use when designing
      retries, timeouts, circuit breakers, rate limiting, hedging, fallback, or
      resilience pipelines for HTTP and other transient-failure-prone operations.
---

# Resilience

## Core Principles
- Add resilience for known failure modes, not by default everywhere.
- Always bound external calls with appropriate timeouts.
- Retry only transient failures and only when the operation is safe to retry.
- Combine resilience mechanisms carefully to avoid retry storms.

## Patterns
- Use standard .NET resilience integrations for HttpClient when applicable.
- Add jitter to retries when many clients can retry simultaneously.
- Propagate cancellation independently from timeout handling.
- Observe resilience events so repeated failures are diagnosable.

## Anti-patterns
- Do not retry authentication/validation/permanent failures.
- Do not retry non-idempotent operations blindly.
- Do not stack multiple retry layers unknowingly.
- Do not use fallback to silently return incorrect data.

## Decision Guide
| Failure mode | Strategy |
|---|---|
| Slow dependency | Timeout |
| Short transient failure | Limited retry with backoff/jitter |
| Repeated dependency outage | Circuit breaker |
| Excess load | Rate limiting / concurrency control |
