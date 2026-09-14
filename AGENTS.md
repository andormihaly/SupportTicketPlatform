# Codex Project Instructions

## Project Overview

This repository is a .NET 10 backend application based on Clean Architecture.

The solution is organized into the following layers:

- Domain
- Application
- Infrastructure
- API
- Tests

Follow the existing architecture, conventions, and patterns when modifying the codebase.

## Architecture

Respect Clean Architecture dependency rules:

- Domain must not depend on Application, Infrastructure, or API.
- Application may depend on Domain.
- Infrastructure may depend on Application and Domain.
- API acts as the composition root and may depend on Application and Infrastructure.
- Keep business logic out of controllers and infrastructure components.
- Prefer extending existing abstractions and patterns over introducing new architectural concepts.

Do not move responsibilities between layers without a clear architectural reason.

## .NET and C#

- Target .NET 10 and use modern C# features where appropriate.
- Keep nullable reference types enabled.
- Prefer primary constructors for dependency injection.
- Use async/await for I/O-bound operations.
- Propagate CancellationToken through asynchronous call chains.
- Avoid unnecessary abstractions and premature generalization.
- Prefer clear and maintainable code over clever implementations.
- Follow the naming and formatting conventions already present in the repository.

## Dependency Injection

Use the built-in Microsoft dependency injection abstractions.

Register dependencies in the appropriate composition or infrastructure registration layer.

Do not use service locator patterns.

Prefer constructor injection.

## API

Keep API endpoints thin.

Endpoints should:

1. Validate or bind the incoming request.
2. Delegate application behavior to the Application layer.
3. Translate the result into the appropriate HTTP response.

Do not place business logic directly in controllers or endpoints.

## Data Access

Keep persistence concerns inside Infrastructure.

Do not expose database-specific implementation details to Domain or Application.

Use asynchronous database APIs where available.

Avoid unnecessary queries and loading data that is not required.

## Error Handling

Use the project's existing error-handling strategy.

Do not introduce local try/catch blocks unless they provide meaningful recovery, translation, or additional context.

Do not silently swallow exceptions.

## Logging

Use the existing Microsoft logging abstractions.

Prefer structured logging.

Do not log secrets, credentials, tokens, connection strings, or other sensitive information.

Avoid unnecessary informational logging in high-frequency code paths.

## Security

Treat all external input as untrusted.

Do not hard-code secrets or credentials.

Do not weaken authentication, authorization, validation, transport security, or other existing security controls to simplify an implementation.

Use secure defaults.

## Testing

Maintain or add tests when behavior changes.

Prefer focused tests that verify observable behavior.

Do not change production behavior only to make a test easier to write.

When modifying existing functionality, run the relevant tests.

## Working with the Repository

Before implementing a change:

1. Inspect the relevant existing code.
2. Understand the current architecture and conventions.
3. Identify existing implementations that solve similar problems.
4. Prefer consistency with the repository over introducing a new pattern.

When the requested change is ambiguous or has significant architectural consequences, explain the options before making broad changes.

## Validation

After making code changes:

1. Build the affected project or solution.
2. Run the relevant tests.
3. Review the resulting changes for unintended modifications.
4. Report any build or test failures clearly.

Do not claim that a build or test succeeded unless it was actually executed successfully.

## Scope Control

Make only the changes required for the requested task.

Do not perform unrelated refactoring.

Do not rename or reorganize existing files, folders, projects, or public APIs unless required by the task.

Preserve backward compatibility unless the requested change explicitly requires otherwise.

## Skills

Use the repository-provided Codex skills when their instructions apply to the current task.

Skills contain more detailed guidance for specific areas such as:

- Clean Architecture
- asynchronous programming
- dependency injection
- HTTP and API development
- LINQ
- logging
- security
- SQL and data access
- testing

Prefer the relevant skill instructions over inventing new project conventions.

## Subagents

Use the repository subagents when focused expertise is useful.

- Delegate architecture, solution structure, dependency boundary, and architectural trade-off tasks to `dotnet-architect`.
- Delegate code review, PR review, correctness, security, performance, and maintainability review tasks to `code-reviewer`.
- Delegate test strategy, test implementation, testability, and test review tasks to `test-engineer`.

Prefer delegation when the task clearly matches one of these specializations.
The main agent remains responsible for integrating the result into the final response.

## Roslyn MCP Code Navigation

Use the `dotnet-roslyn` MCP server as the preferred source of semantic information about the loaded .NET solution.

- Prefer Roslyn MCP over grep, globbing, repository-wide text search, or broad source-file reads when the task is about C# symbols, usages, implementations, callers, inheritance, dependencies, project structure, ASP.NET endpoints, or dependency injection.
- Start with the smallest semantic query that can answer the discovery question. Use MCP results to narrow the scope before reading source.
- Treat successful MCP discovery as a replacement for equivalent repository-wide discovery. Do not repeat the same lookup with grep, glob, text search, or broad file reads.
- Wait for MCP discovery results before deciding which source files need to be read. Do not launch equivalent repository discovery in parallel with the MCP call.
- Read source only when implementation details are still needed or when a file must be modified. Prefer targeted member/source retrieval or a file outline before reading a full file.
- For project configuration, dependencies, endpoints, and DI registrations, prefer the corresponding Roslyn MCP capability before manually inspecting `.csproj`, `Directory.*.props`, controller registrations, `Program.cs`, or DI extension files.
- Before changing a shared symbol, public API, abstraction, endpoint, dependency, or architectural boundary, use semantic impact analysis where relevant.
- Do not force MCP usage when the required code is already known and present in context, or when MCP cannot answer the question precisely.
- Keep MCP usage sequential and purposeful: semantic discovery → targeted source inspection when needed → modification → verification.
- Avoid redundant tool calls. Once a trustworthy MCP result answers a question, reuse it instead of rediscovering the same information.
- Prefer semantic information from Roslyn MCP when it can answer a codebase question more precisely than repository inspection.
- Use information already obtained from MCP to guide subsequent exploration and avoid unnecessary or redundant context gathering.
- Keep repository exploration proportional to the task: inspect only the additional source or configuration needed to understand or implement the change.
The goal is not to call every MCP tool. The goal is to use Roslyn semantics whenever they reduce ambiguity, repository scanning, or unnecessary context consumption.
