---
name: dependency-injection
description: >
  Dependency injection guidance for .NET applications. Use when registering
      services, selecting lifetimes, designing constructors, using keyed services,
      factories, scopes, or resolving DI lifetime/captive dependency issues.
---

# Dependency Injection

## Core Principles
- Prefer constructor injection for required dependencies.
- Choose lifetimes deliberately: singleton, scoped, transient.
- Keep the composition root in Api and registrations owned by their layers.
- Prefer explicit dependencies over service-location patterns.

## Patterns
- Application registrations belong in `ApplicationServicesRegistration`.
- Infrastructure registrations belong in `InfrastructureServicesRegistration`.
- Use factories/keyed services only when they make multiple implementations clearer.

## Anti-patterns
- Do not inject `IServiceProvider` only to resolve arbitrary dependencies later.
- Do not capture scoped services in singletons.
- Do not register concrete infrastructure dependencies in Domain.
- Do not hide large dependency lists behind a service locator.

## Decision Guide
| Dependency behavior | Lifetime |
|---|---|
| Per-request state / DbContext-like service | Scoped |
| Stateless lightweight service | Often transient/scoped depending on use |
| Shared thread-safe process-wide service | Singleton |
