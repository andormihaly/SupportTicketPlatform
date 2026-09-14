# AppHost and Resources

AppHost is the orchestration model for the distributed application.

- Add .NET projects as project resources.
- Add caches, databases, brokers, containers, and executables with the appropriate integration.
- Use `.WithReference(...)` to supply dependency information and service discovery.
- Use `.WaitFor(...)` only when startup ordering/readiness matters.
- Keep resource names stable and meaningful.
- Do not put application business rules in AppHost.

When adding an integration, verify its current API and package documentation because Aspire integrations evolve quickly.
