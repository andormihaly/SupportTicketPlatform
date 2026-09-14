---
name: container-publish
description: >
  Dockerfile-less container publishing with the .NET 10 SDK. Use when the
  project should build or publish an OCI container directly from `dotnet
  publish`, when configuring SDK container properties, or when deciding
  between SDK container publishing and a Dockerfile.
---

# .NET SDK Container Publishing

## Core Principles

1. **Use SDK container publishing when the application does not need custom Dockerfile build steps.**
2. **Run production containers as non-root.**
3. **Prefer small, hardened runtime images when application requirements allow them.**
4. **Use a Dockerfile when OS packages, custom build stages, or other image customization require it.**

## Patterns

### Basic Publish

```bash
dotnet publish src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj /t:PublishContainer --os linux --arch x64
```

### Project Configuration

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <ContainerRepository>myapp-api</ContainerRepository>
</PropertyGroup>
```

Add container-family/base-image settings only when they are appropriate for the application's globalization, diagnostics, and native-library needs.

### Registry Publishing

Authenticate with the target registry before pushing and pass registry/repository/tag settings explicitly.

```bash
dotnet publish src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj /t:PublishContainer --os linux --arch x64 -p:ContainerRegistry=myregistry.azurecr.io -p:ContainerImageTag=1.0.0
```

### Multi-architecture

Use supported runtime identifiers when both x64 and arm64 images are required. Verify all native dependencies support each target architecture.

## Anti-patterns

- Do not run production containers as root unless there is a demonstrated requirement.
- Do not choose a chiseled/distroless image without checking globalization, diagnostics, shell, and native-library requirements.
- Do not embed registry credentials in project files or source control.
- Do not force SDK container publishing when a Dockerfile is the clearer solution.

## Decision Guide

| Scenario | Recommendation |
|---|---|
| Standard ASP.NET Core API with no custom OS packages | SDK container publishing is a strong default |
| Needs `apt`, native packages, custom build stages, or shell tooling | Dockerfile |
| Multi-arch deployment | Configure supported runtime identifiers |
| Hardened production image | Consider chiseled/distroless-compatible runtime images |
| Registry push | Authenticate securely, then publish with explicit registry/tag settings |
