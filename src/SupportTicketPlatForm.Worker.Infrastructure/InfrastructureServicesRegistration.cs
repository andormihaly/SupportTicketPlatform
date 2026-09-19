using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SupportTicketPlatform.Worker.Application.Configuration;
using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Infrastructure.AI;
using SupportTicketPlatform.Worker.Infrastructure.Messaging;
using SupportTicketPlatform.Worker.Infrastructure.Persistence;
using SupportTicketPlatform.Worker.Infrastructure.Persistence.Repositories;

namespace SupportTicketPlatform.Worker.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        var connectionString = configuration.GetConnectionString("PostgreSQLConnectionString")  ?? throw new InvalidOperationException("PostgreSQL connection string is missing.");

        var dataSource = PostgreSqlDataSourceFactory.Create(connectionString);

        services.AddSingleton(dataSource);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                dataSource,
                npgsqlOptions => npgsqlOptions.UseVector()));

        services.AddScoped<IAppDbContext>(sp =>  sp.GetRequiredService<AppDbContext>());

        services.AddScoped<ITicketRepository, TicketRepository>();

        services.AddSingleton<AIProjectClient>(sp =>
        {
            var workerOptions = sp.GetRequiredService<IOptions<WorkerOptions>>().Value;

            return new AIProjectClient(new Uri(workerOptions.Foundry.ProjectEndpoint), new DefaultAzureCredential());
        });

        services.AddSingleton<AzureOpenAIClient>(sp =>
        {
            var workerOptions = sp.GetRequiredService<IOptions<WorkerOptions>>().Value;
            return new AzureOpenAIClient(new Uri(workerOptions.Foundry.OpenAIEndpoint), new DefaultAzureCredential());
        });

        services.AddScoped<IEmbeddingService, AzureOpenAiEmbeddingService>();
        services.AddScoped<ITicketAnalysisPublisher, TicketAnalysisPublisher>();

        return services;
    }
}
