using SupportTicketPlatform.Seeder.Models;

namespace SupportTicketPlatform.Seeder.Data;

public static class SeedData
{
    public static IReadOnlyList<SeedTicket> Tickets { get; } =
    [
        // Entra ID / Authentication & Access

        new(
            "Entra ID authentication fails after client secret rotation",
            "Our ASP.NET Core application uses an Entra ID app registration to authenticate against Microsoft Graph. We rotated the client secret in Azure yesterday and updated the new value in Key Vault, but the application now receives AADSTS7000215: Invalid client secret provided. The old secret has already been removed from the app registration.",
            "The application was still using a cached configuration value containing the previous client secret. The application instance was restarted after confirming that the new Key Vault secret version was active. Authentication succeeded with the new secret."),

        new(
            "Managed Identity cannot access Azure Key Vault secrets",
            "Our Azure App Service uses a system-assigned Managed Identity to access Key Vault. The identity is enabled and the application uses DefaultAzureCredential, but reading a secret returns HTTP 403 Forbidden. Access works locally when using my developer account.",
            "The App Service Managed Identity did not have permission on the Key Vault. The Key Vault Secrets User RBAC role was assigned to the Managed Identity at the Key Vault scope. After RBAC propagation completed, the application could read the secrets without credentials in configuration."),

        new(
            "App Service API returns 401 after enabling Entra ID authentication",
            "We enabled Microsoft Entra authentication on an Azure App Service hosting an ASP.NET Core API. Since enabling authentication, all API calls from our React frontend return HTTP 401. The frontend successfully signs the user in and sends an access token in the Authorization header.",
            "The access token was requested for the wrong audience. A scope for the API app registration was exposed and the frontend was configured to request that scope. After obtaining an access token with the API's Application ID URI as the audience, authenticated requests succeeded."),

        new(
            "Azure SQL connection fails with Managed Identity",
            "Our App Service connects to Azure SQL using Managed Identity. Password-based connections work, but after switching to Authentication=Active Directory Default, the application receives a login failed error for the Managed Identity.",
            "A contained database user for the App Service Managed Identity had not been created in Azure SQL. The Managed Identity was created as an external database user and granted the required database permissions. The application could then connect using Entra authentication."),

        new(
            "Worker cannot connect to Azure Service Bus with Managed Identity",
            "A .NET background worker running in Azure Container Apps uses DefaultAzureCredential to connect to an Azure Service Bus namespace. The application starts successfully but receives an authorization error when attempting to consume messages from the queue.",
            "The Container App Managed Identity was missing the required Service Bus data-plane permission. The Azure Service Bus Data Receiver role was assigned to the identity on the Service Bus namespace. After role propagation, the worker successfully consumed messages."),

        new(
            "DefaultAzureCredential authenticates with the wrong account locally",
            "Our .NET application uses DefaultAzureCredential. When running locally from Visual Studio, access to an Azure resource fails with a permissions error even though my normal Azure account has access. The same application works correctly after deployment to Azure.",
            "Visual Studio was authenticated with a different Microsoft account than the one that had access to the Azure resources. The incorrect account was removed from Visual Studio and the correct Azure account was selected. DefaultAzureCredential then acquired a token for the expected identity."),

        new(
            "Entra access token works with Microsoft Graph but not with our API",
            "Our client application authenticates users through Microsoft Entra ID. The obtained token works correctly when calling Microsoft Graph, but sending the same token to our ASP.NET Core API results in HTTP 401 Unauthorized.",
            "Microsoft Graph and the custom API are different token audiences. The client application was updated to request a separate access token using the scope exposed by the API app registration. The API then accepted the token with the correct audience."),

        new(
            "Application lost Key Vault access after changing permission model to RBAC",
            "Our application previously accessed Key Vault successfully using a Managed Identity and an access policy. We changed the Key Vault permission model from Vault Access Policy to Azure role-based access control. Immediately afterwards, the application started receiving HTTP 403 when retrieving secrets.",
            "Access policies are no longer evaluated when the vault uses the Azure RBAC permission model. The application's Managed Identity was assigned the Key Vault Secrets User role. Once the RBAC assignment propagated, secret access worked again."),

        new(
            "Azure Container App cannot authenticate with its Managed Identity",
            "We enabled a system-assigned Managed Identity on an Azure Container App and changed the application to use DefaultAzureCredential. The application still fails to authenticate to Azure resources, although the identity appears correctly in the Azure portal.",
            "The Managed Identity existed but had not been granted access to the target Azure resources. The required RBAC data-plane roles were assigned to the Container App identity on each resource it needed to access. Authentication then succeeded without connection credentials."),

        new(
            "User can sign in with Entra ID but receives 403 from the API",
            "A user can successfully authenticate with Microsoft Entra ID and receives a valid access token. Requests to our ASP.NET Core API reach the application, but endpoints protected by authorization policies return HTTP 403 Forbidden. Other users can access the same endpoints.",
            "Authentication was working correctly, but the affected user did not have the application role required by the API authorization policy. The appropriate app role was assigned to the user in the Enterprise Application. After obtaining a new access token containing the role claim, the protected endpoints were accessible."),

        // App Service / Container Apps – Deployment & Runtime

        new(
            "App Service returns HTTP 500 after successful deployment",
            "Our ASP.NET Core API is deployed to Azure App Service through GitHub Actions. The deployment completes successfully, but every request returns HTTP 500 after the new version is deployed. The application works correctly when running locally.",
            "The production environment was missing a required configuration value used during application startup. The setting was added to the App Service configuration and the application was restarted. The API started successfully and requests returned normally."),

        new(
            "New Azure Container App revision fails during startup",
            "A new Docker image was deployed to Azure Container Apps. The deployment created a new revision, but the revision never becomes healthy and no traffic is routed to it. The previous revision continues to work normally.",
            "The container was configured to listen on port 8080 while the Container App ingress target port was configured as 80. The ingress target port was changed to 8080 and a new revision was deployed. The revision became healthy and started receiving traffic."),

        new(
            "App Service fails during startup when loading Key Vault configuration",
            "Our ASP.NET Core application loads configuration values from Azure Key Vault during startup. It works locally, but after deployment to App Service the application fails to start and the logs show that Key Vault secrets cannot be retrieved.",
            "The App Service system-assigned Managed Identity was enabled but did not have permission to read secrets from the vault. The Key Vault Secrets User role was assigned to the Managed Identity. After RBAC propagation and an application restart, configuration loaded successfully."),

        new(
            "Azure Container App repeatedly restarts after deployment",
            "Our .NET worker deployed to Azure Container Apps starts successfully but restarts every few minutes. Container logs show no unhandled application exception, and the same Docker image runs continuously when started locally.",
            "A liveness probe was configured against an HTTP endpoint even though the worker did not expose an HTTP server. The probe continuously failed and caused the platform to restart the container. The invalid HTTP liveness probe was removed and the worker remained running."),

        new(
            "App Service still serves old version after deployment",
            "Our deployment pipeline reports that the latest ASP.NET Core build was successfully deployed to Azure App Service. However, the application still behaves like the previous version and recently added API endpoints are not available.",
            "The deployment pipeline was publishing an outdated build artifact from a previous pipeline stage. The pipeline was corrected to deploy the artifact generated by the current build. After redeployment, App Service started running the expected application version."),

        new(
            "Azure Container App fails to pull image from ACR",
            "We deployed a new Azure Container App using an image stored in Azure Container Registry. The revision fails before the application starts, and the Container App reports that the image cannot be pulled from the registry.",
            "The Container App Managed Identity did not have permission to pull images from the registry. The AcrPull role was assigned to the identity on the Azure Container Registry and the registry configuration was updated to use that identity. The next revision successfully pulled and started the image."),

        new(
            "API endpoints return 404 after deployment to Azure App Service",
            "Our ASP.NET Core API works correctly on localhost, but after deploying it to Azure App Service all API endpoints return HTTP 404. The application itself appears to be running and the App Service default domain is reachable.",
            "The deployment package contained the project directory instead of the output produced by dotnet publish. The deployment workflow was updated to publish the application first and deploy the generated publish directory. After redeployment, the API routes were available."),

        new(
            "Service Bus worker in Container Apps scales down to zero replicas",
            "Our background worker processes messages from an Azure Service Bus queue and runs in Azure Container Apps. When the queue is empty, the portal shows zero running replicas. We initially assumed the worker had stopped and would no longer process new messages.",
            "The Container App was configured with a Service Bus KEDA scaling rule and a minimum replica count of zero. Scaling to zero when the queue is empty is expected behavior. A new message was added to the queue and the scaler automatically created a worker replica, which processed the message successfully."),

        new(
            "Container App starts without required environment variables",
            "Our CI/CD pipeline successfully deploys a new container image to Azure Container Apps, but the application fails during startup because several required configuration values are missing. The values are present in the local development configuration.",
            "The settings existed only in the local appsettings.Development.json file and were never configured in the Container App environment. The required values were added as Container App environment variables and sensitive values were configured using Container Apps secrets. A new revision started successfully."),

        new(
            "ASP.NET Core application fails to start after framework upgrade",
            "Our application was upgraded to a newer .NET version and works correctly on developer machines. The deployment to Azure App Service succeeds, but the application fails immediately during startup with a framework-related error.",
            "The App Service runtime stack was still configured for the previous .NET runtime version. The App Service stack settings were updated to a runtime compatible with the deployed application and the application was restarted. The new version then started successfully."),

        // Azure SQL / PostgreSQL / Database & Data Access

        new(
            "App Service cannot connect to Azure SQL after database firewall changes",
            "Our ASP.NET Core application running in Azure App Service was connecting successfully to Azure SQL. After tightening the SQL Server networking configuration, the application started returning database connection errors while developers can still connect from approved local IP addresses.",
            "The SQL Server firewall configuration no longer allowed connections originating from the App Service. Network access was updated to allow the application through the selected Azure networking configuration. Database connectivity was restored without changing the application connection string."),

        new(
            "EF Core database migration fails in production",
            "Our deployment pipeline runs EF Core migrations against the production PostgreSQL database. The application itself can read and write data successfully, but the migration fails when attempting to create a new table and index.",
            "The application's runtime database identity intentionally had only data-access permissions and could not modify the schema. A separate deployment identity with the required schema permissions was configured for migrations. The migration completed successfully while the runtime identity remained restricted."),

        new(
            "PostgreSQL connections fail when API traffic increases",
            "Our ASP.NET Core API uses Azure Database for PostgreSQL. Everything works normally under light traffic, but during traffic spikes requests intermittently fail with errors indicating that additional database connections cannot be opened.",
            "The application was creating new Npgsql connections without consistently disposing them. Database access code was corrected to properly release connections, and connection pooling was used for subsequent requests. Connection usage stabilized and the intermittent failures stopped."),

        new(
            "API sometimes returns old data after a successful database update",
            "Our API updates customer information successfully and the database contains the new values. However, a GET request immediately after the update sometimes returns the previous values even though the update operation completed successfully.",
            "The GET endpoint was returning data from an application-level cache that was not invalidated after the update. Cache invalidation was added to the update flow so subsequent reads retrieve the current database value."),

        new(
            "Ticket search query becomes slow with larger database",
            "An EF Core query that searches support tickets was fast when the system contained only a few thousand records. After the table grew significantly, the same endpoint started taking several seconds to respond. Database CPU also increases while the query is running.",
            "The generated SQL filtered and sorted using columns that did not have appropriate indexes. The execution plan was reviewed and an index matching the frequently used filter and ordering columns was added. Query execution time dropped significantly."),

        new(
            "pgvector similarity search returns no results",
            "Our application stores OpenAI embeddings in a PostgreSQL vector column and uses cosine distance to find similar support tickets. A ticket definitely has an embedding, but the similarity query returns an empty result even though several semantically related tickets exist.",
            "The query filtered candidates to resolved tickets with non-null resolutions and embeddings. The existing test records had embeddings but were still in Open status and therefore correctly excluded. After resolving several tickets and adding their resolutions, the similarity query returned the expected candidates."),

        new(
            "Cannot save OpenAI embedding to PostgreSQL vector column",
            "Our worker generates embeddings using Azure OpenAI and stores them in a PostgreSQL pgvector column configured as vector(1536). After changing the embedding deployment, database inserts started failing with a vector dimension error.",
            "The new embedding configuration produced vectors with a different number of dimensions than the PostgreSQL column expected. The embedding configuration and database schema were aligned to use the same vector dimensions. New embeddings could then be stored successfully."),

        new(
            "Retried Service Bus message creates duplicate database records",
            "A Service Bus worker receives a message and inserts a record into PostgreSQL. Occasionally the worker completes the database operation but fails before the Service Bus message is completed. When the message is delivered again, another identical database record is created.",
            "Message processing was made idempotent using the message's business identifier. A unique database constraint was added and the worker checks whether the operation has already been processed before inserting the record. Retried messages no longer create duplicates."),

        new(
            "PostgreSQL Entra authentication fails after application runs for several hours",
            "Our .NET application connects to Azure Database for PostgreSQL using Microsoft Entra authentication. The initial connection works, but after the application has been running for several hours new database connections begin failing authentication.",
            "An Entra access token had been acquired once and reused as if it were a permanent database password. The Npgsql data source was configured with a periodic password provider that obtains fresh Entra tokens through DefaultAzureCredential. New pooled connections can now authenticate after previous tokens expire."),

        new(
            "EF Core endpoint consumes excessive memory when reading tickets",
            "An API endpoint returns a summary of support tickets containing only the ticket ID, title and status. As the number of tickets increased, memory usage and response time became unexpectedly high even though the response itself contains very little data.",
            "The query loaded complete tracked Ticket entities, including related data, before mapping them to the response model in memory. The query was changed to use AsNoTracking() and project directly to the required response DTO with Select. This reduced the amount of data transferred and avoided unnecessary EF Core tracking."),

        // Azure Service Bus / Messaging / Background Processing

        new(
            "Service Bus messages are not consumed by the worker",
            "Our .NET background worker is running and reports that the Service Bus processor started successfully. However, messages continue accumulating in the queue and the message handler is never called.",
            "The worker was configured with a queue name different from the queue used by the producer. The Service Bus configuration was corrected so both applications use the same queue. Existing messages were then consumed successfully."),

        new(
            "Worker occasionally processes the same Service Bus message twice",
            "Our worker receives commands from an Azure Service Bus queue. In production we occasionally see the same operation executed more than once even though the producer sends the message only once.",
            "Service Bus uses an at-least-once delivery model, so message redelivery can occur if processing completes but message settlement does not. The consumer was made idempotent using the business operation ID and a persistent record of completed operations. Duplicate deliveries no longer repeat the business operation."),

        new(
            "Service Bus messages disappear after repeated processing failures",
            "A worker throws an exception whenever it processes a specific message. Service Bus retries the message several times, but eventually it is no longer visible in the active queue and the worker stops receiving it.",
            "The message reached the queue's maximum delivery count and was automatically moved to the dead-letter queue. The DLQ was inspected to identify the failed message and its dead-letter information. The underlying processing problem was corrected before the message was resubmitted."),

        new(
            "Service Bus worker cannot deserialize incoming message",
            "Our API publishes a TicketCreated message to Azure Service Bus. The worker receives the message, but System.Text.Json throws a deserialization exception while converting the message body to the shared contract.",
            "The producer and consumer were using incompatible representations of an enum property in the message contract. The shared contract and JSON serialization settings were aligned so both applications use the same representation. New messages were deserialized successfully."),

        new(
            "Long-running Service Bus handler causes duplicate processing",
            "A Service Bus message triggers a background operation that can take several minutes. Occasionally another worker receives the same message before the first worker finishes, causing the same operation to run twice.",
            "The message lock expired before processing completed. Automatic lock renewal was configured for the expected processing duration, and the operation was also made idempotent to protect against redelivery. Long-running messages could then complete without duplicate side effects."),

        new(
            "API creates a new Service Bus client for every request",
            "An ASP.NET Core API publishes a message whenever a support ticket is created. Under higher traffic the endpoint becomes slow and the application opens a large number of Service Bus connections.",
            "A new ServiceBusClient was being created and disposed for every HTTP request. The client was registered as a long-lived singleton and reused across requests, while senders were also reused where appropriate. Connection usage and request latency improved significantly."),

        new(
            "Invalid Service Bus message repeatedly blocks processing",
            "Our worker consumes ticket messages from a Service Bus queue. One malformed message cannot be deserialized and is repeatedly delivered to the worker, generating the same exception each time and delaying useful messages.",
            "Invalid messages were explicitly dead-lettered when deserialization or required contract validation failed. The dead-letter reason and error description were recorded for diagnostics. Valid messages could then continue processing normally."),

        new(
            "Service Bus subscription receives unrelated events",
            "Several applications publish events to the same Azure Service Bus topic. Our notification service should process only customer notification events, but its subscription receives every event published to the topic.",
            "The subscription did not have an appropriate SQL filter and was using the default rule that matches all messages. A subscription filter based on message metadata was added and the default match-all rule was removed. The subscriber then received only notification events."),

        new(
            "Service Bus message is completed before database operation finishes",
            "Our worker receives a Service Bus message and writes data to PostgreSQL. We discovered that when the process crashes during the database operation, the message is no longer available for retry and the database update is missing.",
            "The worker was completing the Service Bus message before the database operation had successfully finished. Message completion was moved to the end of successful processing and automatic completion was disabled. Failed processing now leaves the message available for retry."),

        new(
            "Azure Container App does not scale out for queued Service Bus messages",
            "Our Service Bus queue contains hundreds of pending messages, but the Azure Container App worker remains at a single replica even though a Service Bus scaling rule has been configured.",
            "The KEDA scaling rule referenced the wrong queue name, so the scaler was monitoring an empty queue. The rule was updated with the correct Service Bus queue configuration. As the active message count increased, Container Apps created additional worker replicas."),

        // Azure Storage / Blob Storage / File Processing

        new(
            "App Service receives 403 when uploading files to Blob Storage",
            "Our ASP.NET Core application running in Azure App Service uploads documents to Azure Blob Storage using DefaultAzureCredential. Uploads work locally with a developer account, but the deployed application receives HTTP 403 AuthorizationPermissionMismatch.",
            "The App Service Managed Identity had access to the storage account management plane but did not have a Blob Storage data role. The Storage Blob Data Contributor role was assigned to the Managed Identity. After role propagation, the application could upload blobs successfully."),

        new(
            "Users receive 404 when opening uploaded Blob Storage files",
            "Our application successfully uploads documents to a private Blob Storage container. The generated blob URL is stored in the database, but users receive HTTP 404 when they open the URL directly in their browser.",
            "The container was private, so the blob URL alone did not authorize access to the file. The application was changed to generate short-lived SAS URLs for authorized download requests instead of exposing the raw blob URL."),

        new(
            "Large file uploads to Azure Blob Storage fail",
            "Small documents upload successfully through our ASP.NET Core API, but files larger than approximately 100 MB consistently fail. The Blob Storage account itself has enough capacity and direct uploads using Azure tools work correctly.",
            "The upload was passing through the API and exceeded the configured HTTP request size limit before reaching Blob Storage. The application was changed to issue a limited SAS token so large files could be uploaded directly from the client to Blob Storage."),

        new(
            "Application cannot access Blob Storage after public access was disabled",
            "Our application was accessing an Azure Storage account successfully. After disabling public network access on the storage account, every blob operation from the application began timing out even though Managed Identity permissions remained unchanged.",
            "The problem was network connectivity rather than authorization. Private connectivity to the storage account was configured and DNS resolution was updated so the storage endpoint resolved through the private endpoint. Blob operations then succeeded without restoring public network access."),

        new(
            "Users receive outdated blob after file replacement",
            "Our application replaces an existing document in Azure Blob Storage using the same blob name. The new content is visible when inspecting the blob directly, but users occasionally continue receiving the previous document.",
            "The file was being served through a caching layer that still contained the previous response. The caching strategy was changed to use versioned blob names for updated documents, preventing clients and intermediate caches from reusing stale content."),

        new(
            "Uploaded blob is occasionally processed more than once",
            "A background process starts when a new document is uploaded to Azure Blob Storage. Occasionally the same file is processed twice, resulting in duplicate downstream records.",
            "The processing workflow incorrectly assumed that each storage event would be delivered exactly once. Processing was made idempotent by storing the blob identifier and version before creating downstream records. Duplicate event deliveries are now detected and safely ignored."),

        new(
            "Blob listing does not return files from nested paths",
            "Our application stores files using blob names such as tickets/2026/09/document.pdf. A listing operation intended to return all ticket documents only returns files from the top-level prefix and misses documents stored deeper in the hierarchy.",
            "The application was using hierarchical listing without traversing the returned prefixes. The listing logic was changed to either enumerate the hierarchy recursively or use a flat blob listing with the required prefix. All matching blobs are now returned."),

        new(
            "Concurrent uploads overwrite files in Blob Storage",
            "Two users uploaded different documents with the same generated blob name at almost the same time. Both requests reported success, but only the second document remained in storage.",
            "The upload operation allowed unconditional replacement of an existing blob. Unique blob names were introduced using the document identifier, and conditional upload behavior was added where overwriting must be prevented. Concurrent uploads no longer replace unrelated files."),

        new(
            "Blob Storage usage remains high after deleting old files",
            "We deleted a large number of obsolete documents from an Azure Storage account, but the reported storage consumption did not decrease as expected. The application no longer lists the files in the container.",
            "Blob soft delete and versioning were enabled, so deleted blobs and previous versions were still retained and billed during their retention period. The retention configuration and lifecycle management rules were reviewed and adjusted to automatically remove obsolete versions and deleted data according to the required retention policy."),

        new(
            "Background worker fails to download newly uploaded document",
            "Our API uploads a document to Blob Storage and publishes a Service Bus message containing the blob name. A background worker receives the message but occasionally reports that the blob does not exist.",
            "The API published the Service Bus message before the blob upload operation had fully completed successfully. The workflow was changed so the upload is awaited and confirmed before publishing the message. The worker now receives the blob reference only after the file is available.")
    ];
}
