namespace SupportTicketPlatform.Seeder.Models;

public sealed record SeedTicket(
    string Title,
    string Description,
    string? Resolution);
