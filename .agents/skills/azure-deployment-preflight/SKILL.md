---
name: azure-deployment-preflight
description: >
  Pre-deployment validation guidance for Azure workloads. Use before deploying
  applications or infrastructure to Azure to identify configuration, identity,
  networking, infrastructure-as-code, and operational risks.
---

# Azure Deployment Preflight

## Checks
- Confirm subscription, tenant, resource group, region, and environment.
- Validate infrastructure-as-code before deployment.
- Use what-if/preview capabilities where supported.
- Verify required resource providers and quotas.
- Verify Managed Identity/RBAC assignments and least privilege.
- Verify networking, DNS, firewall, private endpoint, and ingress requirements.
- Confirm secrets come from secure configuration.
- Verify application configuration and required environment variables.
- Confirm health checks, logging, metrics, and diagnostics.
- Review destructive or replacement operations before applying changes.

## Anti-patterns
- Do not deploy immediately after generated infrastructure changes without validation.
- Do not assume successful template validation guarantees application correctness.
- Do not broaden RBAC permissions merely to make deployment succeed.
