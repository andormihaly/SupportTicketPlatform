---
name: error-handling
description: >
  Error-handling guidance for .NET APIs. Use when designing HTTP errors,
      centralized exception handling, ProblemDetails, expected failures,
      validation boundaries, or exception propagation.
---

# Error Handling

## Core Principles
- Use consistent machine-readable HTTP errors such as ProblemDetails.
- Handle unexpected exceptions at an appropriate centralized application boundary.
- Validate untrusted input at system boundaries.
- Do not hide unexpected failures.

## Patterns
- Map known error conditions deliberately to HTTP status codes.
- Let unexpected exceptions reach centralized exception handling.
- Add meaningful context only where it helps diagnosis without leaking sensitive data.

## Anti-patterns
- Do not return bare error strings or ad-hoc error shapes.
- Do not catch `Exception` deep in application code merely to continue.
- Do not catch and immediately rethrow without adding value.
- Do not expose stack traces or secrets to API clients.

## Decision Guide
| Failure | Guidance |
|---|---|
| Invalid external input | Validate at boundary |
| Expected business/application outcome | Represent explicitly and map consistently |
| Unexpected exception | Centralized handler + safe ProblemDetails |
