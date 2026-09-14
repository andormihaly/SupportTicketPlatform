---
name: azure-architecture-autopilot
description: >
  Azure architecture analysis and design guidance. Use when reviewing an existing
  Azure architecture, proposing Azure resources for a new system, identifying
  architecture risks, or preparing infrastructure/deployment recommendations.
---

# Azure Architecture Autopilot

## Core Principles
- Start from workload requirements before selecting Azure services.
- Prefer managed Azure services when they reduce operational burden without violating requirements.
- Consider identity, networking, security, reliability, observability, scalability, and cost together.
- Prefer Managed Identity and least-privilege RBAC.
- Separate architecture recommendations from deployment execution.

## Workflow
1. Understand application components and dependencies.
2. Identify existing Azure resources when reviewing a deployed workload.
3. Map requirements to Azure capabilities.
4. Identify risks and trade-offs.
5. Propose the smallest architecture that satisfies the requirements.
6. Validate security, reliability, observability, and cost implications.
7. Produce infrastructure-as-code only when requested.

## Anti-patterns
- Do not add Azure services merely because they are available.
- Do not assume public networking is appropriate.
- Do not hardcode credentials.
- Do not treat a generated architecture diagram as deployment truth.
