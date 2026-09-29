using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using StackExchange.Redis;
using SupportTicketPlatform.Persistence;
using SupportTicketPlatform.Persistence.Redis;
using SupportTicketPlatform.Worker.Application.Configuration;
using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Infrastructure.AI;
using SupportTicketPlatform.Worker.Infrastructure.Repositories;
using System.ClientModel.Primitives;

namespace SupportTicketPlatform.Worker.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        var connectionString = configuration.GetConnectionString("PostgreSQLConnectionString") ?? throw new InvalidOperationException("PostgreSQL connection string is missing.");
        var dataSource = PostgreSqlDataSourceFactory.Create(connectionString);

        services.AddSingleton(dataSource);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource, npgsqlOptions => npgsqlOptions.UseVector()));

        #pragma warning disable OPENAI001

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<WorkerOptions>>().Value;
            var foundry = options.Foundry;

            if (string.IsNullOrWhiteSpace(foundry.TicketAnalysisDeployment))
            {
                throw new InvalidOperationException("Foundry:TicketAnalysisDeployment is required for ticket analysis.");
            }

            var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(),"https://ai.azure.com/.default");

            return new ChatClient(foundry.TicketAnalysisDeployment,tokenPolicy, new OpenAIClientOptions{Endpoint = new Uri(foundry.OpenAIEndpoint)});
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<WorkerOptions>>().Value;
            var foundry = options.Foundry;

            if (string.IsNullOrWhiteSpace(foundry.EmbeddingDeployment))
                throw new InvalidOperationException("Foundry:EmbeddingDeployment is required for embeddings.");

            var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(), "https://ai.azure.com/.default");

            return new EmbeddingClient(foundry.EmbeddingDeployment, tokenPolicy, new OpenAIClientOptions { Endpoint = new Uri(foundry.OpenAIEndpoint) });
        });

        var redisConnectionString = configuration.GetConnectionString("RedisConnectionString") ?? throw new InvalidOperationException("Redis connection string is missing.");

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString);
            configuration.Ssl = true;
            configuration.Protocol = RedisProtocol.Resp3;
            configuration.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential()).GetAwaiter().GetResult();

            var redis = ConnectionMultiplexer.Connect(configuration);
            RedisSimilarityCacheIndex.CreateAsync(redis).GetAwaiter().GetResult();

            return redis;
        });

        services.AddSingleton<RedisSimilarityCache>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IEmbeddingService, AzureOpenAiEmbeddingService>();
        services.AddScoped<ITicketAnalysisService, AzureOpenAiTicketAnalysisService>();

        return services;
    }
}
