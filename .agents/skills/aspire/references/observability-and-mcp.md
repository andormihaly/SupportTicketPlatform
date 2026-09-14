# Aspire Observability and MCP

Aspire can provide a local dashboard for logs, traces, metrics, resource state, and endpoints.

## Observability

- Use the dashboard for development/troubleshooting, not as a production dependency.
- Keep telemetry compatible with the project's broader OpenTelemetry strategy.
- Avoid sending secrets or unnecessary PII into telemetry.

## MCP

Some Aspire versions expose an MCP server that supported AI assistants can use to inspect a running AppHost, resources, logs/traces, commands, and documentation.

Before configuring it:

1. verify the installed Aspire CLI supports MCP;
2. inspect the tools exposed by the current version;
3. configure it for the actual AI environment;
4. treat state-changing resource commands as operational actions, not read-only inspection.

Do not assume MCP tool names or capabilities remain identical across Aspire versions.
