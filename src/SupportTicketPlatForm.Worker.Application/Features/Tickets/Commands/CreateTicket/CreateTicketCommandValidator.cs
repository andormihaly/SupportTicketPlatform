using FluentValidation;
using SupportTicketPlatform.Worker.Domain.Entities;

namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Status).NotEmpty().Must(status => Enum.TryParse<TicketStatus>(status, true, out var parsedStatus) && Enum.IsDefined(parsedStatus)).WithMessage("The ticket status is invalid.");
        RuleFor(x => x.OccurredAt).NotEmpty();
    }
}
