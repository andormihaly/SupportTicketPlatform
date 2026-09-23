using System.Net.Http.Json;
using SupportTicketPlatform.Seeder.Models;

namespace SupportTicketPlatform.Seeder.Clients;

public sealed class TicketApiClient(HttpClient httpClient)
{
    public async Task<Guid> CreateTicketAsync(SeedTicket ticket, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            ticket.Title,
            ticket.Description
        };

        var response = await httpClient.PostAsJsonAsync("api/tickets", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateTicketResponse>(cancellationToken);

        if (result is null)
            throw new InvalidOperationException("The API returned an empty response when creating a ticket.");

        return result.Id;
    }

    public async Task WaitUntilTicketExistsAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 20;
        var delay = TimeSpan.FromMilliseconds(500);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var response = await httpClient.GetAsync($"api/tickets/{ticketId}", cancellationToken);

            if (response.IsSuccessStatusCode)
                return;

            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                response.EnsureSuccessStatusCode();

            await Task.Delay(delay, cancellationToken);
        }

        throw new TimeoutException($"Ticket '{ticketId}' was not created within the expected time.");
    }
    public async Task CreateResolutionAsync(Guid ticketId, string content, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Content = content
        };

        var response = await httpClient.PostAsJsonAsync($"api/tickets/{ticketId}/resolution", request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
