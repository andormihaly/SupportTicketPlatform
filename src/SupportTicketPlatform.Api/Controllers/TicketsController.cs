using Microsoft.AspNetCore.Mvc;
using SupportTicketPlatform.Application.Features.Tickets.Commands.CreateResolution;
using SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using Wolverine;

namespace SupportTicketPlatform.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController(IMessageBus bus) : ControllerBase
{
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
}
