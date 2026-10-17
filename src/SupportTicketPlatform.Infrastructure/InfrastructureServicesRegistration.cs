using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Embeddings;
using StackExchange.Redis;
using SupportTicketPlatform.Application.Configuration;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Infrastructure.AI;
using SupportTicketPlatform.Infrastructure.Repositories;
using SupportTicketPlatform.Infrastructure.Telemetry;
using SupportTicketPlatform.Persistence;
using SupportTicketPlatform.Persistence.Redis;
using System.ClientModel.Primitives;

namespace SupportTicketPlatform.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        var apiOptions = new ApiOptions();
        configuration.Bind(apiOptions);
       
        var postgreSqlConnectionString = apiOptions.ConnectionStrings.PostgreSQLConnectionString;

        if (string.IsNullOrWhiteSpace(postgreSqlConnectionString))
            throw new InvalidOperationException("PostgreSQL connection string is missing.");

        var dataSource = PostgreSqlDataSourceFactory.Create(postgreSqlConnectionString);

        services.AddSingleton(dataSource);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource, npgsqlOptions => npgsqlOptions.UseVector()));
        services.AddScoped<ITicketRepository, TicketRepository>();

        var redisConnectionString = apiOptions.ConnectionStrings.RedisConnectionString;

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

        var redisOptions = configuration.GetSection("Redis").Get<RedisOptions>() ?? throw new InvalidOperationException("Redis configuration is missing.");
        services.AddSingleton(redisOptions);
        services.AddSingleton<RedisSimilarityCache>();

        services.AddSingleton<IApplicationTelemetry, OpenTelemetryApplicationTelemetry>();

#pragma warning disable OPENAI001
        services.AddSingleton(_ =>
        {
            var deployment = apiOptions.Foundry.EmbeddingDeployment;
            var endpoint = apiOptions.Foundry.OpenAIEndpoint;

            if (string.IsNullOrWhiteSpace(deployment))
                throw new InvalidOperationException("Foundry:EmbeddingDeployment is required for embeddings.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("Foundry:OpenAIEndpoint is required for embeddings.");

            var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(), "https://cognitiveservices.azure.com/.default");

            return new EmbeddingClient(deployment, tokenPolicy, new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
        });
#pragma warning restore OPENAI001

        services.AddScoped<IEmbeddingService, AzureOpenAiEmbeddingService>();

        return services;
    }
}
