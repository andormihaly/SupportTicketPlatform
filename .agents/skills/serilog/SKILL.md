---
name: serilog
description: >
  Serilog guidance for .NET applications. Use when Serilog is selected for
      structured logging, sinks, enrichers, request logging, configuration, or
      integration with ASP.NET Core and observability platforms.
---

# Serilog

## Core Principles
- Use Serilog only when its structured logging/sink ecosystem provides needed value.
- Keep logging structured and avoid sensitive data.
- Configure sinks and minimum levels by environment.
- Prefer `ILogger<T>` in application code so logging remains framework-friendly.

## Patterns
- Configure Serilog at the application host boundary.
- Use request logging when it provides useful request-level diagnostics.
- Enrich logs with stable correlation/service properties.
- Send logs to durable/centralized sinks in deployed environments when required.

## Anti-patterns
- Do not call static `Log.*` throughout business/application code when DI `ILogger<T>` is available.
- Do not log entire request/response bodies by default.
- Do not let sink failures block core application behavior without an explicit requirement.

## Decision Guide
| Need | Guidance |
|---|---|
| Built-in logging is sufficient | Do not add Serilog |
| Rich sinks/enrichment/structured pipeline needed | Consider Serilog |
