using Azure.Messaging.ServiceBus;
using SupportTicketPlatform.Contracts.Messages;
using System.Text.Json;

namespace SupportTicketPlatform.Worker;

public sealed class Worker(ServiceBusClient serviceBusClient, ILogger<Worker> logger) : BackgroundService
{
    private const string QueueName = "ticket-created";
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using ServiceBusProcessor processor = serviceBusClient.CreateProcessor(QueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });

        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);

        logger.LogInformation("Service Bus processor started. Listening on queue: {QueueName}", QueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        logger.LogInformation("Message received. MessageId: {MessageId}, DeliveryCount: {DeliveryCount}", args.Message.MessageId, args.Message.DeliveryCount);

        TicketCreated? message;

        try
        {
            message = JsonSerializer.Deserialize<TicketCreated>(args.Message.Body.ToString());

            if (message is null)
            {
                await args.DeadLetterMessageAsync(args.Message, "InvalidMessage", "TicketCreated message could not be deserialized.", args.CancellationToken);
                return;
            }
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid TicketCreated message. MessageId: {MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "InvalidJson", ex.Message, args.CancellationToken);
            return;
        }

        try
        {
            logger.LogInformation("Processing ticket {TicketId} - {Title}. DeliveryCount: {DeliveryCount}", message.TicketId, message.Title, args.Message.DeliveryCount);

            //Ticket processing logic goes here. For example, you might want to save the ticket to a database or perform some other business logic.

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ticket processing failed. TicketId: {TicketId}, DeliveryCount: {DeliveryCount}", message.TicketId, args.Message.DeliveryCount);
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Service Bus error. Entity: {EntityPath}, ErrorSource: {ErrorSource}", args.EntityPath, args.ErrorSource);
        return Task.CompletedTask;
    }
}
