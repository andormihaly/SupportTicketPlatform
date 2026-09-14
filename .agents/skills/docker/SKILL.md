---
name: docker
description: >
  Docker and Linux containerization guidance for .NET 10 ASP.NET Core backend
  applications. Use when creating or reviewing Dockerfiles, .dockerignore files,
  multi-stage builds, runtime images, ports, health checks, non-root execution,
  native/system dependencies, environment configuration, or container build issues.
---

# Docker

## Core Principles

1. **Use multi-stage builds for Dockerfile-based .NET images.** Build with the .NET SDK image and run with the ASP.NET Core runtime image.
2. **Keep the final runtime image minimal.** Do not ship the SDK, source code, build caches, or tools that are only needed during compilation.
3. **Run production containers as non-root whenever possible.**
4. **Use explicit, compatible .NET 10 image tags.** Do not depend on `latest`.
5. **Keep secrets out of images, Dockerfiles, build arguments, and source control.**
6. **Choose the base image according to actual runtime requirements.** Debian/Ubuntu, Alpine, chiseled, and Azure Linux have different globalization, native library, shell, and diagnostics characteristics.

## Project Detection

Before creating or changing a Dockerfile:

- Identify the ASP.NET Core startup project.
- Read the project's `TargetFramework` rather than guessing the .NET version.
- Identify project references that must be restored and built.
- Check whether `NuGet.config`, private feeds, native libraries, additional files, or system packages are required.
- Check whether the application already exposes a health endpoint before adding a container health check.

## Multi-stage Dockerfile Pattern

A standard ASP.NET Core image should normally have:

1. **Build stage**
   - Use a compatible `.NET 10 SDK` image.
   - Copy project files first when doing so improves Docker layer caching.
   - Restore dependencies.
   - Copy the remaining source.
   - Publish the startup project in Release configuration.

2. **Runtime stage**
   - Use a compatible `.NET 10 ASP.NET Core runtime` image.
   - Copy only published output.
   - Configure the working directory and required ports.
   - Run as a non-root user.
   - Set the application entry point.

Example shape:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj", "src/SupportTicketPlatform.Api/"]
COPY ["src/SupportTicketPlatform.Application/SupportTicketPlatform.Application.csproj", "src/SupportTicketPlatform.Application/"]
COPY ["src/SupportTicketPlatform.Domain/SupportTicketPlatform.Domain.csproj", "src/SupportTicketPlatform.Domain/"]
COPY ["src/SupportTicketPlatform.Infrastructure/SupportTicketPlatform.Infrastructure.csproj", "src/SupportTicketPlatform.Infrastructure/"]

RUN dotnet restore "src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj"

COPY . .
RUN dotnet publish "src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
ENTRYPOINT ["dotnet", "SupportTicketPlatform.Api.dll"]
```

Adapt image variants, ports, files, and dependencies to the application instead of copying the example blindly.

## `.dockerignore`

Create a `.dockerignore` that excludes unnecessary build context. It should normally include at least:

```text
**/bin/
**/obj/
.git/
.github/
.vs/
.vscode/
*.user
*.suo
```

Also exclude project-specific temporary files, local secrets, generated artifacts, and directories that are not required to build the image.

## Ports and Environment

- Prefer the platform/runtime defaults unless the application has a concrete port requirement.
- Use environment variables or secure external configuration for environment-specific settings.
- Never embed production connection strings, credentials, tokens, or certificate passwords into the image.
- Persist application data outside the container filesystem when persistence is required.

## Health Checks

Add a container health check only when the application exposes a suitable health endpoint and the chosen runtime image contains—or intentionally adds—the required probe tool.

Do not install packages such as `curl` solely for a health check without considering the image-size and attack-surface trade-off. Platform-level health probes may be preferable.

## System and Native Dependencies

When the application requires OS packages or native libraries:

- install only what is required;
- use the package manager that matches the selected Linux distribution;
- remove package-manager caches when practical;
- verify architecture compatibility for x64/arm64 builds;
- reconsider chiseled/distroless images when shell or native-package installation is required.

## Verification

After changing containerization:

```bash
docker build -t supportticketplatform-api:local .
```

If runtime verification is possible, run the image and verify:

- the process starts successfully;
- the configured port is reachable;
- configuration is supplied externally;
- health checks work when configured;
- the process runs as the expected non-root user.

## Anti-patterns

- Do not use `latest` as the only production image identity.
- Do not copy the entire SDK/build environment into the final image.
- Do not run as root unless there is a demonstrated requirement.
- Do not bake secrets into image layers.
- Do not install unnecessary OS packages.
- Do not assume Alpine or chiseled images are drop-in replacements for every workload.
- Do not add a Docker health check that calls a nonexistent application endpoint.
- Do not modify unrelated application code merely to containerize the service.

## Decision Guide

| Scenario | Recommendation |
|---|---|
| Standard ASP.NET Core API requiring a Dockerfile | Multi-stage Dockerfile |
| No custom OS/build steps required | Also consider the `container-publish` skill |
| Needs custom OS packages or native dependencies | Dockerfile with compatible Linux base |
| Wants smallest hardened image | Consider chiseled/distroless only after compatibility review |
| Needs persistent data | External volume or managed storage |
| Needs multi-architecture image | Verify dependencies and build for supported RIDs/platforms |
