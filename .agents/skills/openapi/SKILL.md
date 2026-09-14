---
name: openapi
description: >
  OpenAPI guidance for ASP.NET Core APIs using built-in OpenAPI support.
      Use when documenting controllers/actions, schemas, response contracts,
      operation metadata, API descriptions, or generated OpenAPI documents.
---

# OpenAPI

## Core Principles
- Treat OpenAPI as an API contract, not decoration.
- Keep generated documentation aligned with actual controller behavior.
- Document non-success responses and authentication requirements where relevant.
- Prefer built-in ASP.NET Core OpenAPI support unless another library is justified.

## Patterns
- Use explicit request/response DTOs.
- Add useful summaries/descriptions without duplicating obvious code.
- Represent ProblemDetails/error responses consistently.
- Keep schema names stable for public APIs.

## Anti-patterns
- Do not document a response code the endpoint cannot produce.
- Do not expose internal/domain implementation details as public schemas accidentally.
- Do not rely on UI tooling as the source of truth; the OpenAPI document is the contract.

## Decision Guide
| Need | Guidance |
|---|---|
| Machine-readable API contract | OpenAPI document |
| Interactive documentation UI | Use a compatible UI such as Scalar when desired |
