using Azure.Messaging.ServiceBus;
using SupportTicketPlatform.Contracts.Messages;
using SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.AnalyzeTicket;
using System.Text.Json;
using Wolverine;

namespace SupportTicketPlatform.Worker;

public sealed class TicketAnalysisWorker(ServiceBusClient serviceBusClient, IServiceScopeFactory scopeFactory, ILogger<TicketAnalysisWorker> logger) : BackgroundService
{
    private const string QueueName = "ticket-analysis";
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

        TicketAnalysisRequested? message;

        try
        {
            message = JsonSerializer.Deserialize<TicketAnalysisRequested>(args.Message.Body.ToString());

            if (message is null)
            {
                await args.DeadLetterMessageAsync(args.Message, "InvalidMessage", "TicketAnalysisRequested message could not be deserialized.", args.CancellationToken);
                return;
            }
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid TicketAnalysisRequested message. MessageId: {MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "InvalidJson", ex.Message, args.CancellationToken);
            return;
        }

        try
        {
            logger.LogInformation("Processing analysis request for ticket {TicketId}. DeliveryCount: {DeliveryCount}", message.TicketId, args.Message.DeliveryCount);

            var command = new AnalyzeTicketCommand(message.TicketId, message.OccurredAt);

            await using var scope = scopeFactory.CreateAsyncScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            await bus.InvokeAsync(command, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);

            logger.LogInformation("Ticket analysis message processed successfully. TicketId: {TicketId}, MessageId: {MessageId}", message.TicketId, args.Message.MessageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Analysis request processing failed. TicketId: {TicketId}, DeliveryCount: {DeliveryCount}", message.TicketId, args.Message.DeliveryCount);
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Service Bus error. Entity: {EntityPath}, ErrorSource: {ErrorSource}", args.EntityPath, args.ErrorSource);
        return Task.CompletedTask;
    }
}
