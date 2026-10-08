using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;
using System.Text.Json;

namespace SupportTicketPlatform.Infrastructure.Messaging;

public sealed class ServiceBusTicketMessagePublisher(ServiceBusClient serviceBusClient, ILogger<ServiceBusTicketMessagePublisher> logger)  : ITicketMessagePublisher
{
    private const string QueueName = "ticket-created";
    private const string TicketAnalysisQueueName = "ticket-analysis";

    public async Task PublishAsync(TicketCreated message, CancellationToken cancellationToken = default)
    {
        await using ServiceBusSender sender =  serviceBusClient.CreateSender(QueueName);

        string body = JsonSerializer.Serialize(message);

        var serviceBusMessage = new ServiceBusMessage(body)
        {
            ContentType = "application/json",
            Subject = nameof(TicketCreated),
            MessageId = message.TicketId.ToString()
        };

        logger.LogInformation("Publishing {MessageType} message to queue {QueueName}. MessageId: {MessageId}, TicketId: {TicketId}",
                               nameof(TicketCreated),QueueName, serviceBusMessage.MessageId, message.TicketId);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        logger.LogInformation("Published {MessageType} message to queue {QueueName}. MessageId: {MessageId}, TicketId: {TicketId}",
                              nameof(TicketCreated), QueueName, serviceBusMessage.MessageId,  message.TicketId);
    }

    public async Task PublishAsync(TicketAnalysisRequested message, CancellationToken cancellationToken = default)
    {
        await using ServiceBusSender sender = serviceBusClient.CreateSender(TicketAnalysisQueueName);

        string body = JsonSerializer.Serialize(message);

        var serviceBusMessage = new ServiceBusMessage(body)
        {
            ContentType = "application/json",
            Subject = nameof(TicketAnalysisRequested),
            MessageId = message.TicketId.ToString()
        };

        logger.LogInformation("Publishing {MessageType} message to queue {QueueName}. MessageId: {MessageId}, TicketId: {TicketId}", nameof(TicketAnalysisRequested),
            TicketAnalysisQueueName, serviceBusMessage.MessageId, message.TicketId);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        logger.LogInformation("Published {MessageType} message to queue {QueueName}. MessageId: {MessageId}, TicketId: {TicketId}", nameof(TicketAnalysisRequested),
            TicketAnalysisQueueName, serviceBusMessage.MessageId, message.TicketId);
    }
}
