---
name: testing
description: >
  Testing guidance for .NET applications. Use when designing, implementing,
  reviewing, or debugging unit tests, integration tests, test infrastructure,
  test data, asynchronous tests, or time-dependent tests.
---

# Testing

## Core Principles

1. **Choose the appropriate test level for the behavior.** Use unit tests for focused logic and integration tests when multiple components or external boundaries must work together.
2. **Test observable behavior, not implementation details.**
3. **Use Arrange-Act-Assert as the default structure.**
4. **Keep tests independent and deterministic.**
5. **Use test infrastructure that accurately represents the behavior under test.** Understand where in-memory providers or test doubles differ from production systems.

## Patterns

### Arrange-Act-Assert

```csharp
[Fact]
public async Task GetOrderAsync_ExistingOrder_ReturnsOrder()
{
    // Arrange

    // Act

    // Assert
}
```

Keep each test focused on one behavior. Multiple assertions are fine when they describe different aspects of the same result.

### Test Data Builders

Use builders or factories when test setup becomes repetitive or obscures intent. Keep defaults valid and override only what matters to the scenario.

### Time-dependent Tests

Prefer `TimeProvider` and a controllable test time provider instead of real clock access or arbitrary waiting.

### Integration Infrastructure

Use realistic infrastructure when provider-specific behavior matters. Testcontainers can be appropriate for databases, brokers, or other services, but it is not mandatory for every integration test.

## Anti-patterns

- Do not make tests depend on execution order.
- Do not share mutable state between tests without deliberate isolation.
- Do not use arbitrary `Task.Delay` calls to make asynchronous tests pass.
- Do not mock the system under test.
- Do not assert private implementation details when public behavior can be verified instead.
- Do not use an in-memory provider as proof of provider-specific SQL/database behavior.

## Naming

Prefer descriptive names following:

```text
MethodName_Scenario_ExpectedResult
```

## Decision Guide

| Scenario | Test type |
|---|---|
| Pure domain/application logic | Unit test |
| HTTP pipeline, serialization, middleware, routing | Integration test |
| Database translation, constraints, transactions | Provider-representative integration test |
| Time-dependent behavior | Unit/integration test with controllable `TimeProvider` |
| External API behavior | Test double or controlled integration environment depending on risk |
