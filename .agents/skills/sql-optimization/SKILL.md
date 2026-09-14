---
name: sql-optimization
description: >
  SQL performance optimization guidance. Use when investigating slow queries,
  execution plans, indexes, pagination, joins, scans, sorts, blocking, or
  database-query performance.
---

# SQL Optimization

## Workflow
1. Capture the actual problematic query and representative parameters.
2. Measure baseline duration/resource usage.
3. Inspect the execution plan where available.
4. Identify scans, expensive joins/sorts, implicit conversions, cardinality issues, or blocking.
5. Check existing indexes before proposing new ones.
6. Change one meaningful factor at a time.
7. Measure again.

## Principles
- Optimize measured bottlenecks.
- Preserve query correctness.
- Prefer sargable predicates.
- Return only required columns and rows.
- Use deterministic pagination.
- Consider keyset pagination for large/high-frequency paging scenarios.

## Anti-patterns
- Do not add indexes to every filtered column.
- Do not rely on `NOLOCK` as a generic performance fix.
- Do not tune against unrealistic data volumes.
