---
name: aspire
description: >
  .NET Aspire guidance for distributed application orchestration, AppHost,
  ServiceDefaults, service discovery, resource integrations, the Aspire dashboard,
  testing, deployment, and Aspire MCP integration. Use when creating, running,
  debugging, configuring, testing, deploying, or troubleshooting an Aspire-based
  application, or when deciding whether Aspire is appropriate for the solution.
---

# .NET Aspire

Aspire is an orchestration and developer-experience layer for distributed applications. It should be introduced when it solves a real orchestration, dependency, observability, or local-development problem—not simply because the application uses .NET.

Detailed guidance is split into references so it can be loaded only when relevant:

- [CLI and setup](references/cli-and-setup.md)
- [AppHost and resources](references/apphost-and-resources.md)
- [Observability and MCP](references/observability-and-mcp.md)
- [Testing and deployment](references/testing-and-deployment.md)

## Core Principles

1. **AppHost describes the distributed application.** It orchestrates projects, containers, executables, and external resources; it is not a business-logic layer.
2. **Use references instead of hardcoded local endpoints.** Let Aspire provide service-discovery and connection information where supported.
3. **Keep shared technical defaults out of business projects.** ServiceDefaults may centralize common health, telemetry, discovery, and resilience configuration when Aspire is adopted.
4. **Use integrations when they simplify resource wiring.** Do not add an Aspire integration without a resource or operational need.
5. **Treat the dashboard as an observability/development tool.** Production behavior must not depend on the dashboard.
6. **Keep deployment choices explicit.** Aspire can help model and publish distributed applications, but target-platform architecture still matters.

## Common Pattern

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var api = builder.AddProject<Projects.SupportTicketPlatform_Api>("api")
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();
```

## When to Use Aspire

Good candidates include:

- multiple services/resources that are difficult to run together locally;
- databases, caches, brokers, or emulators that need repeatable orchestration;
- service discovery between local components;
- a distributed application that benefits from a unified telemetry/dashboard experience;
- polyglot services that need one orchestration model.

Aspire is optional for a single simple API with no orchestration need.

## Anti-patterns

- Do not put domain or application behavior in AppHost.
- Do not hardcode resource endpoints that can be supplied through references.
- Do not introduce Aspire solely to obtain a dashboard.
- Do not require the Aspire dashboard for production operation.
- Do not assume local orchestration automatically defines a production deployment architecture.
- Do not add resources/integrations that the application does not use.

## Decision Guide

| Scenario | Guidance |
|---|---|
| Single API, no dependent resources | Aspire is optional |
| Several services/databases/caches need local orchestration | Aspire is a strong candidate |
| Shared health/telemetry/discovery defaults across services | Consider ServiceDefaults |
| Need to inspect running distributed resources with AI tooling | Consider Aspire MCP integration |
| Need production deployment | Load the deployment reference and validate target-platform requirements |
