---
name: appinsights-instrumentation
description: >
  Application Insights instrumentation guidance for .NET applications on Azure.
  Use when configuring Azure Monitor Application Insights, telemetry collection,
  correlation, sampling, custom telemetry, or troubleshooting observability.
---

# Application Insights Instrumentation

## Core Principles
- Prefer OpenTelemetry-based Azure Monitor instrumentation for new .NET applications.
- Keep telemetry configuration at the application/infrastructure boundary.
- Preserve distributed trace correlation across inbound and outbound calls.
- Never intentionally record secrets or unnecessary PII in telemetry.
- Configure sampling deliberately for production workloads.

## Patterns
- Configure Application Insights/Azure Monitor through dependency injection.
- Use standard ASP.NET Core, HttpClient, and dependency instrumentation before adding custom telemetry.
- Add custom spans/metrics only for meaningful business or operational events.
- Use environment-specific configuration for connection strings and exporters.
- Prefer Managed Identity where an Azure integration supports it.

## Anti-patterns
- Do not scatter vendor-specific telemetry calls through Domain code.
- Do not create spans for trivial methods.
- Do not use high-cardinality dimensions without understanding cost and query impact.
- Do not log tokens, credentials, request secrets, or sensitive payloads.
