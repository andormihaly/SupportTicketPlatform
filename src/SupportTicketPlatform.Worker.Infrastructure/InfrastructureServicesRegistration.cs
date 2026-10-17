using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using SupportTicketPlatform.Worker.Infrastructure.Telemetry;
using System.ClientModel.Primitives;

namespace SupportTicketPlatform.Worker.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        var workerOptions = new WorkerOptions();
        configuration.Bind(workerOptions);

        var postgreSqlConnectionString = workerOptions.ConnectionStrings.PostgreSQLConnectionString;

        if (string.IsNullOrWhiteSpace(postgreSqlConnectionString))
            throw new InvalidOperationException("PostgreSQL connection string is missing.");

        var dataSource = PostgreSqlDataSourceFactory.Create(postgreSqlConnectionString);

        services.AddSingleton(dataSource);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource, npgsqlOptions => npgsqlOptions.UseVector()));

#pragma warning disable OPENAI001
        services.AddSingleton(_ =>
        {
            var foundry = workerOptions.Foundry;

            if (string.IsNullOrWhiteSpace(foundry.TicketAnalysisDeployment))
                throw new InvalidOperationException("Foundry:TicketAnalysisDeployment is required for ticket analysis.");

            if (string.IsNullOrWhiteSpace(foundry.OpenAIEndpoint))
                throw new InvalidOperationException("Foundry:OpenAIEndpoint is required for ticket analysis.");

            var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(), "https://cognitiveservices.azure.com/.default");

            return new ChatClient(foundry.TicketAnalysisDeployment, tokenPolicy, new OpenAIClientOptions { Endpoint = new Uri(foundry.OpenAIEndpoint) });
        });

        services.AddSingleton(_ =>
        {
            var foundry = workerOptions.Foundry;

            if (string.IsNullOrWhiteSpace(foundry.EmbeddingDeployment))
                throw new InvalidOperationException("Foundry:EmbeddingDeployment is required for embeddings.");

            if (string.IsNullOrWhiteSpace(foundry.OpenAIEndpoint))
                throw new InvalidOperationException("Foundry:OpenAIEndpoint is required for embeddings.");

            var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(), "https://cognitiveservices.azure.com/.default");

            return new EmbeddingClient(foundry.EmbeddingDeployment, tokenPolicy, new OpenAIClientOptions { Endpoint = new Uri(foundry.OpenAIEndpoint) });
        });
#pragma warning restore OPENAI001

        var redisOptions = configuration.GetSection("Redis").Get<RedisOptions>() ?? new RedisOptions();

        services.AddSingleton(redisOptions);

        if (redisOptions.Enabled)
        {
            var redisConnectionString = workerOptions.ConnectionStrings.RedisConnectionString;

            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new InvalidOperationException("Redis connection string is missing.");

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.Ssl = true;
                options.Protocol = RedisProtocol.Resp3;
                options.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential()).GetAwaiter().GetResult();

                var redis = ConnectionMultiplexer.Connect(options);

                RedisSimilarityCacheIndex.CreateAsync(redis).GetAwaiter().GetResult();

                return redis;
            });

            services.AddSingleton<RedisSimilarityCache>();
        }
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IEmbeddingService, AzureOpenAiEmbeddingService>();
        services.AddScoped<ITicketAnalysisService, AzureOpenAiTicketAnalysisService>();
        services.AddSingleton<IApplicationTelemetry, OpenTelemetryApplicationTelemetry>();
        services.AddSingleton<IApplicationMetrics, OpenTelemetryApplicationMetrics>();

        return services;
    }
}
