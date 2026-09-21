using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketPlatform.Persistence;
using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Infrastructure.AI;
using SupportTicketPlatform.Worker.Infrastructure.Repositories;

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
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IEmbeddingService, AzureOpenAiEmbeddingService>();

        return services;
    }
}
