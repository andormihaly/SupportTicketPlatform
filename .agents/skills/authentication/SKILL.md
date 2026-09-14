---
name: authentication
description: >
  Authentication and authorization guidance for ASP.NET Core APIs. Use when
      configuring identity providers, JWT bearer authentication, policies,
      claims, roles, `[Authorize]`, `[AllowAnonymous]`, or access-control behavior.
---

# Authentication and Authorization

## Core Principles
- Treat authentication and authorization as separate concerns.
- Make authorization intent explicit on every controller or endpoint.
- Prefer policy-based authorization for reusable business/access rules.
- Validate token issuer, audience, lifetime, and signature according to the identity provider.
- Keep secrets and credentials out of source code.

## Patterns
- Configure authentication once at the Api boundary.
- Use `[Authorize]`, `[AllowAnonymous]`, and named policies deliberately.
- Keep authorization decisions close to the protected operation while reusable rules live in policies/handlers.
- Prefer managed/workload identity over client secrets in Azure when supported.

## Anti-patterns
- Do not rely on authentication alone when authorization is required.
- Do not parse or trust claims from unvalidated tokens.
- Do not hardcode credentials or signing keys.
- Do not use broad admin-style policies when narrower permissions suffice.

## Decision Guide
| Need | Guidance |
|---|---|
| User/API identity | Configure an appropriate authentication scheme |
| Reusable access rule | Policy-based authorization |
| Public operation | Mark explicitly anonymous |
