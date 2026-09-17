using Azure.Core;
using Azure.Identity;
using Npgsql;

namespace SupportTicketPlatform.Worker.Infrastructure.Persistence;

public static class PostgreSqlDataSourceFactory
{
    public static NpgsqlDataSource Create(string connectionString)
    {
        var credential = new DefaultAzureCredential();
        var builder = new NpgsqlDataSourceBuilder(connectionString);

        builder.UseVector();

        builder.UsePeriodicPasswordProvider(async (_, cancellationToken) =>
        {
            var token = await credential.GetTokenAsync(new TokenRequestContext(["https://ossrdbms-aad.database.windows.net/.default"]), cancellationToken);
            return token.Token;
        }, TimeSpan.FromMinutes(55), TimeSpan.FromSeconds(5));

        return builder.Build();
    }
}
