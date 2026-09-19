using Azure.Identity;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Embeddings;
using SupportTicketPlatform.Worker.Application.Configuration;
using SupportTicketPlatform.Worker.Application.Interfaces;
using System.ClientModel.Primitives;

namespace SupportTicketPlatform.Worker.Infrastructure.AI;

#pragma warning disable OPENAI001

public sealed class AzureOpenAiEmbeddingService(IOptions<WorkerOptions> options) : IEmbeddingService
{
    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        var endpoint = new Uri(options.Value.Foundry.OpenAIEndpoint);
        var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(), "https://ai.azure.com/.default");
        var embeddingClient = new EmbeddingClient(options.Value.Foundry.EmbeddingDeployment, tokenPolicy, new OpenAIClientOptions { Endpoint = endpoint });
        var result = await embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);

        return result.Value.ToFloats().ToArray();
    }
}
