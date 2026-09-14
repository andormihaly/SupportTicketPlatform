---
name: scalar
description: >
  Scalar API reference guidance for ASP.NET Core OpenAPI projects. Use when
      adding or configuring Scalar as an interactive OpenAPI documentation UI,
      including document endpoints, environment exposure, and UI customization.
---

# Scalar API Reference

## Core Principles
- Use Scalar as a UI over an OpenAPI contract; do not treat it as the contract itself.
- Expose interactive API documentation deliberately, especially outside development.
- Keep authentication/document access aligned with the application's security requirements.

## Patterns
- Map built-in OpenAPI first, then map Scalar to that document.
- Keep UI customization minimal unless the project has a real documentation requirement.
- Prefer development-only exposure when public production documentation is not required.

## Anti-patterns
- Do not expose sensitive internal API documentation publicly by accident.
- Do not duplicate API descriptions solely in UI-specific configuration.
- Do not couple application behavior to the documentation UI.

## Decision Guide
| Need | Guidance |
|---|---|
| Interactive local API docs | Scalar is a good option |
| Machine-readable contract | OpenAPI document, independent of Scalar |
