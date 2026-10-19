# AI Support Ticket Platform

A cloud-native support ticket platform built with **.NET 10**, **Azure Kubernetes Service (AKS)**, and **Azure AI**. The system processes tickets asynchronously, stores semantic embeddings in PostgreSQL, retrieves related resolved cases, and generates AI-assisted ticket analyses using Retrieval-Augmented Generation (RAG).

The API and background Worker are independently containerized and deployed to AKS. Azure-managed identities secure service-to-service access, while GitHub Actions automates build, test, image publishing, and deployment.

## Architecture

```text
                          Client / Support Agent
                                   |
                                   v
                           ASP.NET Core API
                           /       |        \
                Ticket reads   Ticket creation   Analysis request
                    |              |                  |
                    |              v                  v
                    |       Azure Service Bus (ticket-created / ticket-analysis)
                    |                         |
                    |                         v
                    |                   .NET Worker
                    |                    /       \
                    |           Embedding       RAG analysis
                    |                |                |
                    |                v                v
                    +---------- PostgreSQL       Azure OpenAI
                               + pgvector  <----------+
                                    ^
                                    |
                           Optional Redis cache
                           (similarity retrieval)

          API + Worker --> OpenTelemetry --> Azure Monitor / Application Insights

          GitHub Actions --> Azure Container Registry --> Azure Kubernetes Service
```

The API handles HTTP requests, publishes asynchronous commands, and serves ticket and analysis queries. The Worker consumes Service Bus messages, generates embeddings, persists tickets, retrieves related cases, and generates analyses. **PostgreSQL is the source of truth** for ticket data, resolutions, analyses, and semantic similarity search.

## Technology Stack

| Area | Technology |
| --- | --- |
| Backend | C# / .NET 10, ASP.NET Core |
| Application messaging | Wolverine, Azure Service Bus |
| Validation | FluentValidation |
| Data access | Entity Framework Core, Npgsql |
| Database | Azure Database for PostgreSQL Flexible Server |
| Semantic search | pgvector, cosine similarity |
| AI | Azure OpenAI / Azure AI Foundry (embeddings and chat generation) |
| Optional caching | Azure Managed Redis, vector similarity cache |
| Containers | Docker, Azure Container Registry |
| Orchestration | Azure Kubernetes Service |
| Configuration | Azure App Configuration, Azure Key Vault |
| Authentication | Microsoft Entra ID, Managed Identity, AKS Workload Identity |
| Observability | OpenTelemetry, Azure Monitor, Application Insights |
| CI/CD | GitHub Actions, GitHub OIDC federation |
| API documentation | Scalar |

## Key Capabilities

### Asynchronous Ticket Processing

1. A client submits a ticket using `POST /api/tickets`.
2. The API publishes a `TicketCreated` message to Azure Service Bus and responds with HTTP `202 Accepted`.
3. The Worker receives the message and generates a semantic embedding with Azure OpenAI.
4. The Worker persists the ticket and embedding in PostgreSQL.
5. The ticket becomes available through `GET /api/tickets/{id}`.

This workflow separates HTTP request latency from embedding generation and persistence. Ticket creation is eventually consistent.

### Resolutions and Semantic Search

Resolved tickets form the retrieval knowledge base. Each resolution is associated with a ticket, and similarity queries use stored embeddings to find related resolved cases.

- `POST /api/tickets/{id}/resolution` — save a resolution and mark a ticket as resolved.
- `GET /api/tickets/{id}/similar` — find resolved tickets similar to an existing ticket.
- `POST /api/tickets/similar` — search for similar resolved tickets using arbitrary input text.

Similarity results are ranked using **pgvector cosine distance** and include resolution content. Search excludes unresolved cases and records without the required embedding or resolution.

### Retrieval-Augmented Ticket Analysis

1. A client requests analysis with `POST /api/tickets/{ticketId}/analysis`.
2. The API publishes `TicketAnalysisRequested` to Azure Service Bus and returns HTTP `202 Accepted`.
3. The Worker retrieves the ticket and semantically related resolved cases.
4. The Worker builds grounded context from the matching tickets and their resolutions.
5. Azure OpenAI generates a ticket analysis based on that context.
6. The generated result is stored as a separate `TicketAnalysis` record in PostgreSQL.
7. The result can be retrieved using `GET /api/tickets/{ticketId}/analysis`.

The analysis pipeline is a **deterministic RAG workflow**, not an autonomous agent. The generated analysis is kept separate from the original ticket and from human-provided resolutions.

### Optional Semantic Cache

Redis can cache similarity-search results using both exact-key lookup and vector similarity. When caching is disabled or no cached result is found, retrieval uses **PostgreSQL + pgvector** directly. Redis is an optimization layer, not the authoritative data store.

The `Redis:Enabled` configuration setting controls whether the Redis cache is registered and used.

## Solution Structure

```text
src/
  SupportTicketPlatform.Api/                  HTTP endpoints and API host
  SupportTicketPlatform.Application/          API commands, queries, handlers
  SupportTicketPlatform.Infrastructure/       API infrastructure implementations
  SupportTicketPlatform.Worker/               Background processing host
  SupportTicketPlatform.Worker.Application/   Worker application logic
  SupportTicketPlatform.Worker.Infrastructure/Worker infrastructure implementations
  SupportTicketPlatform.Domain/               Domain entities and business model
  SupportTicketPlatform.Contracts/            Shared integration messages
  SupportTicketPlatform.Persistence/          EF Core, migrations, PostgreSQL access
  SupportTicketPlatform.ServiceDefaults/      Shared observability and service defaults
  SupportTicketPlatform.AppHost/              .NET Aspire orchestration
  SupportTicketPlatform.Seeder/               Resolved-ticket knowledge base seeding

tests/
  SupportTicketPlatform.ArchitectureTests/    Architecture dependency tests

.github/workflows/
  deploy-aks.yml                              CI/CD pipeline
```

The API and Worker have separate application and infrastructure layers. Domain models, message contracts, and persistence infrastructure are shared where appropriate.

## Azure Infrastructure

| Resource | Purpose |
| --- | --- |
| Azure Kubernetes Service (`support-ticket-aks`) | Runs the API and Worker deployments |
| Azure Container Registry (`andorsupportticketacr`) | Stores versioned container images |
| Azure Service Bus | Decouples ticket creation and analysis processing |
| Azure Database for PostgreSQL | Persists tickets, resolutions, analyses, and vectors |
| Azure OpenAI / Azure AI Foundry | Embedding generation and RAG analysis |
| Azure App Configuration | Centralizes application configuration |
| Azure Key Vault | Stores sensitive configuration referenced by App Configuration |
| Azure Monitor / Application Insights | Collects distributed traces, logs, and metrics |
| Azure Managed Redis (optional) | Accelerates repeated semantic searches |

AKS uses the Kubernetes namespace `support-ticket`, with separate `support-ticket-api` and `support-ticket-worker` Deployments. The API is exposed internally through the `support-ticket-api` Kubernetes Service.

## Identity and Access Management

The deployment uses **two distinct user-assigned managed identities** to separate application permissions from CI/CD permissions.

### Application Identity — `support-ticket-identity`

The API and Worker use **AKS Workload Identity** with the Kubernetes ServiceAccount `support-ticket-sa`. Kubernetes-issued service account tokens are federated with Microsoft Entra ID, allowing application code to use `DefaultAzureCredential` without storing credentials in containers.

The identity supports access to the application's Azure dependencies, including:

- Azure App Configuration for centralized settings.
- Azure Key Vault for secrets and Key Vault references.
- Azure Service Bus for message publishing and consumption.
- Azure OpenAI for embedding and chat model operations.
- PostgreSQL through an Entra-authenticated database principal.
- Redis through Entra authentication when the cache is enabled.

Service Bus access uses data-plane sender and receiver permissions appropriate to the API and Worker workloads. PostgreSQL permissions are controlled by the database principal and database grants.

### Deployment Identity — `support-ticket-github-identity`

GitHub Actions uses a separate managed identity with **OpenID Connect (OIDC) federation**. The federated credential trusts GitHub-issued tokens for the `andormihaly/SupportTicketPlatform` repository on the `main` branch. This avoids long-lived Azure client secrets in GitHub.

The deployment identity has these Azure RBAC roles:

| Role | Scope | Purpose |
| --- | --- | --- |
| `AcrPush` | Container registry | Push API and Worker images |
| `Azure Kubernetes Service Cluster User Role` | AKS cluster | Retrieve non-admin cluster credentials |
| `Azure Kubernetes Service RBAC Writer` | AKS cluster | Update Kubernetes workloads during deployment |

The AKS cluster uses Microsoft Entra integration with Azure RBAC for Kubernetes Authorization. The GitHub identity's Writer permission is assigned at **cluster scope**.

## Configuration

Application settings are centrally managed through **Azure App Configuration**, with sensitive values stored in **Azure Key Vault** and accessed through Key Vault references.

Representative settings include:

| Configuration key | Purpose |
| --- | --- |
| `ServiceBus:FullyQualifiedNamespace` | Azure Service Bus namespace |
| `Foundry:OpenAIEndpoint` | Azure OpenAI endpoint |
| `ConnectionStrings:PostgreSQLConnectionString` | PostgreSQL connection configuration |
| `ConnectionStrings:SupportTicketInsights` | Azure Monitor exporter connection string |
| `Redis:Enabled` | Enable or disable semantic caching |
| `Redis:SimilarityThreshold` | Semantic cache match threshold |
| `Redis:CacheLifetimeMinutes` | Cache entry lifetime |
| `SimilarTickets:MaxResults` | Similar-ticket query limit |
| `TicketAnalysis:MaxSimilarTickets` | Number of retrieved cases for analysis |

Configuration is loaded before shared service defaults so that telemetry exporters can resolve their connection settings.

## Observability

The API and Worker use shared OpenTelemetry configuration to export telemetry to Azure Monitor / Application Insights. Traces correlate HTTP requests, Service Bus operations, Worker processing, PostgreSQL calls, and Azure OpenAI dependencies.

Instrumentation includes structured application logs, custom activities for ticket creation and analysis, and business metrics:

| Metric | Description |
| --- | --- |
| `support_tickets.created` | Successfully persisted tickets |
| `support_tickets.analysis.completed` | Successfully completed analyses |
| `support_tickets.analysis.duration` | Analysis processing duration |

Logs include operational identifiers such as ticket IDs and message IDs without intentionally logging full ticket descriptions or generated AI content.

## CI/CD — GitHub Actions to AKS

The workflow is defined in [`.github/workflows/deploy-aks.yml`](.github/workflows/deploy-aks.yml) and runs automatically on pushes to `main`. Manual execution is also supported.

The pipeline:

1. Restores dependencies and builds the .NET 10 solution in Release mode.
2. Runs automated tests, including architecture tests.
3. Authenticates to Azure using GitHub OIDC and the dedicated deployment identity.
4. Builds and pushes separate API and Worker images to ACR, tagged with the Git commit SHA.
5. Authenticates to AKS with `kubectl` and `kubelogin`.
6. Updates the existing Kubernetes Deployments with the new image tags.
7. Waits for successful API and Worker rollouts.

The pipeline uses the following GitHub Actions repository variables: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, and `AZURE_SUBSCRIPTION_ID`.

## Build and Test

From the repository root, with the **.NET 10 SDK** installed:

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

The solution also includes a .NET Aspire AppHost for local orchestration. Local execution requires access to the Azure dependencies and appropriate developer credentials/configuration.

## AKS Verification and API Access

After deployment, verify the workloads:

```powershell
kubectl get pods -n support-ticket
kubectl rollout status deployment/support-ticket-api -n support-ticket --timeout=300s
kubectl rollout status deployment/support-ticket-worker -n support-ticket --timeout=300s
```

Forward the internal API service to your workstation:

```powershell
kubectl port-forward svc/support-ticket-api 8080:80 -n support-ticket
```

Open the interactive API documentation at **http://localhost:8080/scalar**.

## API Overview

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/tickets` | Submit a new ticket |
| `GET` | `/api/tickets/{id}` | Retrieve a ticket |
| `POST` | `/api/tickets/{id}/resolution` | Save a resolution |
| `GET` | `/api/tickets/{id}/similar` | Find similar resolved tickets |
| `POST` | `/api/tickets/similar` | Search similar tickets from text |
| `POST` | `/api/tickets/{ticketId}/analysis` | Request AI-generated analysis |
| `GET` | `/api/tickets/{ticketId}/analysis` | Retrieve a stored analysis |

Ticket creation and analysis requests are asynchronous and return HTTP `202 Accepted`; processing completes in the Worker.

## Deployment Status

The GitHub Actions workflow completed successfully, both AKS workloads reached `Running` with zero restarts, and the API was verified through the Scalar interface after deployment.
