---
name: modern-csharp
description: >
  Modern C# guidance for .NET 10 projects. Use when implementing or reviewing
      C# code, choosing language features, records, pattern matching, primary
      constructors, nullable types, collections, or modern syntax.
---

# Modern C#

## Core Principles
- Use modern language features when they improve clarity and maintainability.
- Prefer readable code over clever or compressed syntax.
- Follow the project's coding-style rules.
- Keep nullable reference type intent explicit.

## Patterns
- Primary constructors are appropriate for simple DI-focused classes.
- Records fit DTOs and value-like immutable data.
- Pattern matching and switch expressions can simplify branching.
- Collection expressions are useful when they remain clear.

## Anti-patterns
- Do not adopt a new syntax merely because it is newer.
- Do not hide complex behavior inside dense expressions.
- Do not use `null!` to silence warnings without proving safety.
- Do not expose mutable state unnecessarily.

## Decision Guide
| Need | Guidance |
|---|---|
| Immutable data contract | Consider record |
| Simple constructor-only DI | Consider primary constructor |
| Multi-branch type/value logic | Consider pattern matching/switch expression |
