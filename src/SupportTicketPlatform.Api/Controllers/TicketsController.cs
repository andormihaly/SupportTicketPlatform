using Microsoft.AspNetCore.Mvc;
using SupportTicketPlatform.Api.Application.Queries.Features.TicketAnalysis.GetTicketAnalysis;
using SupportTicketPlatform.Application.Features.Tickets.Commands.AnalyzeTicket;
using SupportTicketPlatform.Application.Features.Tickets.Commands.CreateResolution;
using SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTicketsByText;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using Wolverine;

namespace SupportTicketPlatform.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController(IMessageBus bus) : ControllerBase
{
    [HttpPost("{ticketId:guid}/analysis")]
    public async Task<IActionResult> RequestAnalysis(Guid ticketId, CancellationToken cancellationToken)
    {
        await bus.InvokeAsync(new AnalyzeTicketCommand(ticketId), cancellationToken);

        return Accepted();
    }

    [HttpGet("{ticketId:guid}/analysis")]
    public async Task<IActionResult> GetAnalysis(Guid ticketId, CancellationToken cancellationToken)
    {
        var analysis = await bus.InvokeAsync<TicketAnalysisResponse?>(new GetTicketAnalysisQuery(ticketId), cancellationToken);

        return analysis is null ? NotFound() : Ok(analysis);
    }

    [HttpPost("{id:guid}/resolution")]
    public async Task<IActionResult> CreateResolution(Guid id, CreateResolutionRequest request, CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<CreateResolutionResult>(
            new CreateResolutionCommand(id, request.Content), cancellationToken);

        return result switch
        {
            CreateResolutionResult.TicketNotFound => NotFound(),
            CreateResolutionResult.ResolutionAlreadyExists => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "The ticket already has a resolution."),
            _ => NoContent()
        };
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        var ticketId = await bus.InvokeAsync<Guid>(command, cancellationToken);

        return Accepted(new { id = ticketId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await bus.InvokeAsync<TicketResponse?>(new GetTicketQuery(id), cancellationToken);

        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpGet("{id:guid}/similar")]
    public async Task<IActionResult> GetSimilar(Guid id, CancellationToken cancellationToken)
    {
        var tickets = await bus.InvokeAsync<IReadOnlyList<SimilarTicketResponse>?>(
            new GetSimilarTicketsQuery(id), cancellationToken);

        return tickets is null ? NotFound() : Ok(tickets);
    }

    [HttpPost("similar")]
    public async Task<IActionResult> GetSimilarByText(GetSimilarTicketsByTextQuery query, CancellationToken cancellationToken)
    {
        var tickets = await bus.InvokeAsync<IReadOnlyList<SimilarTicketResponse>>(query, cancellationToken);

        return Ok(tickets);
    }
}
