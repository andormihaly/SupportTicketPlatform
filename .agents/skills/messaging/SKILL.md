---
name: messaging
description: >
  Messaging guidance for .NET backends. Use when integrating queues, topics,
      brokers, producers/consumers, retries, idempotency, dead-letter handling,
      message contracts, or eventual consistency.
---

# Messaging

## Core Principles
- Treat messages as durable contracts.
- Design consumers to tolerate duplicate delivery when the broker can redeliver.
- Use explicit retry and dead-letter strategies.
- Keep transport-specific code in Infrastructure.

## Patterns
- Version message contracts compatibly.
- Include correlation/causation identifiers when distributed tracing needs them.
- Make side-effecting consumers idempotent where practical.
- Acknowledge messages only after required work is safely completed.

## Anti-patterns
- Do not assume exactly-once delivery without end-to-end guarantees.
- Do not put large arbitrary payloads on a broker when object storage/reference is more appropriate.
- Do not retry permanently invalid messages forever.
- Do not expose broker SDK types to Domain.

## Decision Guide
| Need | Guidance |
|---|---|
| Decouple services / asynchronous processing | Messaging may fit |
| Immediate strongly consistent response required | Synchronous call may be simpler |
