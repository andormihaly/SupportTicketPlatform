---
name: csharp-docs
description: >
  C# documentation guidance. Use when writing or reviewing XML documentation,
  public API documentation, summaries, parameter descriptions, exceptions,
  examples, or documentation comments.
---

# C# Documentation

## Core Principles
- Document public APIs when the contract or behavior is not self-evident.
- Explain intent and constraints rather than restating the method/type name.
- Keep documentation synchronized with code.
- Document meaningful exceptions and behavioral guarantees.

## XML Documentation
Use standard tags such as `summary`, `param`, `returns`, `exception`, `remarks`,
`example`, `see`, and `seealso` when they add useful information.

## Anti-patterns
- Do not add noisy comments that merely repeat the code.
- Do not document implementation details as public guarantees.
- Do not copy stale documentation when behavior changes.
