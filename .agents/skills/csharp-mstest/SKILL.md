---
name: csharp-mstest
description: >
  MSTest-specific testing guidance for C#/.NET. Use when the project uses
  MSTest and tests need to be created, reviewed, or maintained.
---

# MSTest Testing

Use this skill together with the general `testing` skill. The general skill defines
testing principles; this skill covers framework-specific syntax and conventions.

## Principles
- Follow the project's existing MSTest conventions.
- Keep tests independent and deterministic.
- Use Arrange-Act-Assert by default.
- Prefer descriptive test names.
- Use async test methods for asynchronous code; do not block on Tasks.
- Do not introduce another test framework into a project that already standardizes on MSTest without a concrete reason.

## Typical Attributes
- Test class: `[TestClass]`
- Test method: `[TestMethod]`

Verify framework APIs against the package version used by the project when using
advanced features such as parameterized tests, fixtures, lifecycle hooks, or parallelization.
