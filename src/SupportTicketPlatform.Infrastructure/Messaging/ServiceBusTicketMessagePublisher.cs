using Azure.Messaging.ServiceBus;
using System.Text.Json;
using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;

namespace SupportTicketPlatform.Infrastructure.Messaging;

public sealed class ServiceBusTicketMessagePublisher(ServiceBusClient serviceBusClient)  : ITicketMessagePublisher
{
    private const string QueueName = "ticket-created";

    public async Task PublishAsync(TicketCreated message, CancellationToken cancellationToken = default)
    {
        ServiceBusSender sender =  serviceBusClient.CreateSender(QueueName);

        string body = JsonSerializer.Serialize(message);

        var serviceBusMessage = new ServiceBusMessage(body)
        {
            ContentType = "application/json",
            Subject = nameof(TicketCreated),
            MessageId = message.TicketId.ToString()
        };

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }
}
