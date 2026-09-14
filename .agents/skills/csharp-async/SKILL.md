---
name: csharp-async
description: >
  Best practices for asynchronous programming in modern C#. Use when implementing
  or reviewing async/await, Task, ValueTask, CancellationToken, parallel async
  operations, async streams, or asynchronous resource handling.
---

# C# Async Programming

## Core Principles
- Async all the way: do not use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` in normal async application code.
- Use the `Async` suffix for asynchronous methods unless a framework contract dictates otherwise.
- Propagate `CancellationToken` through the complete call chain.
- Use `Task`/`Task<T>` as the default async return types.
- Consider `ValueTask<T>` only for measured high-throughput paths that frequently complete synchronously.
- Avoid `async void` except for event handlers.

## Patterns
- Await asynchronous I/O APIs.
- Use `Task.WhenAll` for independent concurrent operations when concurrency is safe and intended.
- Use `IAsyncEnumerable<T>` for streaming asynchronous sequences when it improves memory/latency behavior.
- Use `await using` for asynchronously disposable resources.
- Preserve exception propagation unless the current layer can meaningfully handle the failure.

## Anti-patterns
- Do not add `ConfigureAwait(false)` mechanically to ASP.NET Core application code.
- Do not wrap naturally asynchronous work in `Task.Run`.
- Do not use arbitrary delays for synchronization.
- Do not drop cancellation tokens.
- Do not replace a normal `throw` in an async method with `Task.FromException` merely for style.
