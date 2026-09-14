---
name: project-structure
description: >
  Project and solution structure guidance for this .NET 10 Clean Architecture
  template. Use when adding projects, folders, namespaces, files, or tests, or
  when reviewing whether code is placed consistently within the solution.
---

# Project Structure

## Core Principles

1. **Use `.slnx` for the solution.**
2. **Keep production code under `src/` and tests under `tests/`.**
3. **Keep project names, namespaces, and folder placement consistent.**
4. **Preserve the four-project Clean Architecture boundary unless a concrete requirement justifies a structural change.**

## Standard Layout

```text
MyApp/
├── MyApp.slnx
├── src/
│   ├── MyApp.Api/
│   ├── MyApp.Application/
│   ├── MyApp.Domain/
│   └── MyApp.Infrastructure/
└── tests/
```

Application:

```text
MyApp.Application/
├── Configuration/
├── Core/
├── Features/
├── Interfaces/
└── ApplicationServicesRegistration.cs
```

Domain:

```text
MyApp.Domain/
└── Common/
```

Infrastructure:

```text
MyApp.Infrastructure/
├── Persistence/
├── Services/
└── InfrastructureServicesRegistration.cs
```

## Naming

- Solution: `MyApp.slnx`
- Projects: `MyApp.Api`, `MyApp.Application`, `MyApp.Domain`, `MyApp.Infrastructure`
- Test projects: `<ProjectName>.Tests` unless a more specific test-project name is justified.
- Folders and feature names use PascalCase.
- Namespace structure should normally follow project and folder structure.
- One type per file; file name matches the type name.

## Anti-patterns

- Do not mix test projects into `src/`.
- Do not add new top-level architectural projects without a concrete need.
- Do not create catch-all folders such as `Helpers` or `Utils` when a more specific responsibility exists.
- Do not reorganize unrelated code while implementing a focused change.

## Decision Guide

| Need | Placement |
|---|---|
| HTTP controller or HTTP-specific mapping | Api |
| Application use case or interface | Application |
| Core business concept or rule | Domain |
| Database/external service implementation | Infrastructure |
| Automated tests | tests/ |
