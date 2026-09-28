using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Infrastructure.Repositories;
using SupportTicketPlatform.Persistence;

namespace SupportTicketPlatform.Infrastructure;

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

        var redisConnectionString = configuration.GetConnectionString("RedisConnectionString")  ?? throw new InvalidOperationException("Redis connection string is missing.");

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(redisConnectionString);
            options.Ssl = true;
            options.Protocol = RedisProtocol.Resp3;
            options.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential()).GetAwaiter().GetResult();

            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}
