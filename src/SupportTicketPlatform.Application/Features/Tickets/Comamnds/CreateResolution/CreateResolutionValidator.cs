using FluentValidation;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateResolution;

public sealed class CreateResolutionValidator : AbstractValidator<CreateResolutionRequest>
{
    public CreateResolutionValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}
