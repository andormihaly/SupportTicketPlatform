---
name: ddd
description: >
  Domain-Driven Design guidance for entities, value objects, aggregates,
      domain services, invariants, and domain events. Use only when the domain
      complexity justifies DDD concepts or when the user explicitly asks for them.
---

# Domain-Driven Design

## Core Principles
- Use DDD patterns to model genuine domain complexity, not as ceremony.
- Put invariants with the domain object that owns them.
- Use value objects for concepts defined by value rather than identity.
- Keep aggregate boundaries small and consistency-focused.

## Patterns
- Entities have identity and behavior.
- Value objects are immutable and compare by value.
- Aggregates protect transactional invariants.
- Domain events represent meaningful facts that already happened.

## Anti-patterns
- Do not create aggregates for simple CRUD models without a domain need.
- Do not expose mutable collections that bypass invariants.
- Do not put infrastructure behavior in domain objects.
- Do not use domain events merely as an internal method-call replacement.

## Decision Guide
| Situation | Guidance |
|---|---|
| Concept has no identity, defined by attributes | Value object |
| Business object has lifecycle/identity | Entity |
| Strong consistency boundary spans related objects | Consider aggregate |
| Simple data maintenance | Keep model simple |
