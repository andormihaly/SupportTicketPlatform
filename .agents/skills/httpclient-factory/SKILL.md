---
name: httpclient-factory
description: >
  HttpClientFactory guidance for outbound HTTP integrations. Use when adding
      typed/named HttpClient clients, handlers, timeouts, resilience, headers,
      serialization, or external HTTP API integrations.
---

# HttpClientFactory

## Core Principles
- Use `IHttpClientFactory`; do not repeatedly construct/dispose `HttpClient`.
- Prefer typed clients for cohesive external integrations.
- Set timeouts and resilience according to the downstream dependency.
- Propagate `CancellationToken`.

## Patterns
- Keep DTOs/contracts for external APIs isolated from domain models.
- Use `System.Net.Http.Json` where it fits.
- Configure base address, default headers, handlers, and resilience at registration.
- Log enough context to diagnose failures without logging secrets.

## Anti-patterns
- Do not create a new `HttpClient` per request.
- Do not retry non-idempotent requests blindly.
- Do not use infinite/very large timeouts without justification.
- Do not hardcode credentials in client configuration.

## Decision Guide
| Scenario | Guidance |
|---|---|
| One cohesive external API | Typed client |
| Several configurations of same client | Named/keyed approach may fit |
| Transient downstream failures | Apply measured resilience policy |
