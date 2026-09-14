---
name: security-review
description: >
  Security review guidance for .NET backend applications. Use when reviewing code,
  configuration, authentication, authorization, input handling, secrets, logging,
  dependencies, HTTP behavior, or cloud integrations for security risks.
---

# Security Review

## Review Areas
- Authentication and authorization.
- Input validation and injection risks.
- Secret and credential handling.
- Sensitive-data exposure in logs and responses.
- SQL and command injection.
- SSRF and unsafe outbound requests.
- CORS and transport security.
- File/path handling.
- Dependency vulnerabilities.
- Cloud identity and least privilege.
- Error handling and information disclosure.

## Principles
- Prioritize exploitable risks over stylistic findings.
- Explain the attack scenario and affected boundary.
- Recommend the smallest practical remediation.
- Never weaken authentication, authorization, validation, or TLS to fix a functional problem.
