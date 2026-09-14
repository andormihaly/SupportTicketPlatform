---
name: logging
description: >
  Structured logging guidance for .NET applications. Use when adding log
      statements, choosing levels, defining message templates, correlation, scopes,
      or reviewing logging quality and sensitive-data exposure.
---

# Logging

## Core Principles
- Prefer structured logging with stable message templates.
- Choose log levels according to operational significance.
- Include useful identifiers through structured properties/scopes.
- Never log secrets or authentication material; avoid PII unless justified and protected.

## Patterns
- Use `ILogger<T>` in application services.
- Log state transitions and important external-boundary failures.
- Use scopes/correlation identifiers for request or operation context.
- Prefer properties over string concatenation.

## Anti-patterns
- Do not log the same exception repeatedly at every layer.
- Do not log sensitive request bodies/tokens by default.
- Do not use Information level for noisy per-item loops.
- Do not swallow exceptions after logging unless handling is complete.

## Decision Guide
| Event | Typical level |
|---|---|
| Expected diagnostic detail | Debug/Trace |
| Normal significant operation | Information |
| Recoverable abnormal condition | Warning |
| Failed operation requiring attention | Error |
