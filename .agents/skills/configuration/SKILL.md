---
name: configuration
description: >
  Configuration guidance for .NET applications. Use when working with
      appsettings, environment variables, Options, validation, connection strings,
      Azure Key Vault, secrets, or environment-specific configuration.
---

# Configuration

## Core Principles
- Use strongly typed Options for cohesive configuration.
- Validate required configuration at startup when practical.
- Keep secrets out of committed configuration files.
- Prefer Azure Key Vault for secrets and `DefaultAzureCredential` for Azure access where appropriate.

## Patterns
- Bind configuration sections to dedicated options types.
- Keep options close to the layer that owns the configuration concern.
- Use environment variables or secure providers to override deployment values.
- Prefer Managed Identity in deployed Azure environments.

## Anti-patterns
- Do not read arbitrary configuration keys throughout business code.
- Do not commit real secrets in `appsettings.*.json`.
- Do not silently continue when mandatory configuration is missing.

## Decision Guide
| Data | Source |
|---|---|
| Non-secret application setting | appsettings / environment override |
| Secret | Key Vault or another secure secret provider |
| Azure local development identity | `az login` + `DefaultAzureCredential` |
