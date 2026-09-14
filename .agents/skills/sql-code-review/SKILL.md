---
name: sql-code-review
description: >
  SQL code review guidance for security, correctness, maintainability, and
  performance. Use when reviewing SQL queries, stored procedures, schema changes,
  indexes, or SQL generated/used by a .NET application.
---

# SQL Code Review

## Review Areas
- Parameterization and injection safety.
- Correct joins and predicates.
- Null semantics.
- Data types and implicit conversions.
- Index usage and sargability.
- Unbounded result sets.
- Transaction scope and concurrency.
- Destructive schema/data operations.
- Permissions and least privilege.

## Anti-patterns
- Never concatenate untrusted values into SQL.
- Avoid `SELECT *` in stable application contracts.
- Do not add indexes blindly without considering write cost and existing indexes.
- Do not optimize a query by changing its semantics.
