using System.Text.Json;
using Azure.Messaging.ServiceBus;
using SupportTicketPlatform.Contracts.Tickets;
using SupportTicketPlatform.Worker.Application.Interfaces;

namespace SupportTicketPlatform.Worker.Infrastructure.Messaging;

public sealed class TicketAnalysisPublisher(ServiceBusClient serviceBusClient) : ITicketAnalysisPublisher
{
    private const string QueueName = "ticket-analysis";

    public async Task PublishAsync(TicketReadyForAnalysis message, CancellationToken cancellationToken)
    {
        await using var sender = serviceBusClient.CreateSender(QueueName);
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message));
        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }
}
