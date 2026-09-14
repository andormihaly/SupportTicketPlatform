---
name: opentelemetry
description: >
  OpenTelemetry guidance for .NET applications. Use when adding traces,
      metrics, instrumentation, exporters, activities, resource attributes,
      distributed context, or observability around external dependencies.
---

# OpenTelemetry

## Core Principles
- Instrument meaningful service boundaries and operations.
- Prefer standard automatic instrumentation before custom spans.
- Keep telemetry vendor-neutral where practical.
- Avoid high-cardinality labels and sensitive data.

## Patterns
- Configure service/resource identity centrally.
- Add ASP.NET Core and HttpClient instrumentation for backend APIs.
- Add database instrumentation when useful and safe.
- Create custom `ActivitySource` spans only for meaningful operations not already covered.

## Anti-patterns
- Do not put user IDs, emails, raw URLs with unbounded IDs, or secrets into metric labels.
- Do not create a span for every trivial method.
- Do not assume telemetry export failures should break business requests.

## Decision Guide
| Signal | Use |
|---|---|
| Request/dependency flow | Traces |
| Rates, latency, saturation | Metrics |
| Detailed event/context | Logs |
