using Microsoft.AspNetCore.Mvc;
using SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using Wolverine;

namespace SupportTicketPlatform.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController(IMessageBus bus) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketCommand command)
    {
        await bus.InvokeAsync(command);

        return Accepted();
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
