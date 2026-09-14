---
name: azure-resource-visualizer
description: >
  Guidance for visualizing Azure resources and their relationships. Use when
  creating architecture/resource diagrams from an Azure environment or explaining
  how deployed Azure resources connect to each other.
---

# Azure Resource Visualizer

## Core Principles
- Base diagrams on discovered or explicitly supplied resources.
- Distinguish observed relationships from inferred relationships.
- Group resources by logical boundary such as resource group, network, workload, or environment.
- Show important identity, network, and data-flow relationships without making the diagram unreadable.
- Never invent Azure resources that were not discovered or requested.

## Recommended Output
- Resource inventory.
- Architecture/resource diagram.
- Important dependencies.
- Security/network boundaries.
- Unknown or ambiguous relationships called out explicitly.
