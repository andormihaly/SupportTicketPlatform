using Microsoft.AspNetCore.Mvc;
using SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Domain.Entities;
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
    public async Task<IActionResult> Get(Guid id)
    {
        var ticket = await bus.InvokeAsync<Ticket?>(new GetTicketQuery(id));

        return ticket is null ? NotFound() : Ok(ticket);
    }
}
