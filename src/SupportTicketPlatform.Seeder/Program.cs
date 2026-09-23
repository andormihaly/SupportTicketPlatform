using Microsoft.Extensions.Configuration;
using SupportTicketPlatform.Seeder.Clients;
using SupportTicketPlatform.Seeder.Data;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var baseUrl = configuration["Api:BaseUrl"] ?? throw new InvalidOperationException("API BaseUrl is missing.");

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(baseUrl)
};

var ticketApiClient = new TicketApiClient(httpClient);

Console.WriteLine($"Seeding {SeedData.Tickets.Count} tickets...");

foreach (var ticket in SeedData.Tickets)
{
    Console.WriteLine($"Creating: {ticket.Title}");

    var ticketId = await ticketApiClient.CreateTicketAsync(ticket);

    await ticketApiClient.WaitUntilTicketExistsAsync(ticketId);

    if (ticket.Resolution is not null)
        await ticketApiClient.CreateResolutionAsync(ticketId, ticket.Resolution);

    Console.WriteLine($"Created: {ticketId}");
}

Console.WriteLine("Seeding completed.");
