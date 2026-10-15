using OpenAI.Embeddings;
using SupportTicketPlatform.Worker.Application.Interfaces;

namespace SupportTicketPlatform.Worker.Infrastructure.AI;

public sealed class AzureOpenAiEmbeddingService(EmbeddingClient embeddingClient) : IEmbeddingService
{
    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        var result = await embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);

        return result.Value.ToFloats().ToArray();
    }
}
