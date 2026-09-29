using FluentValidation;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTicketsByText;

public sealed class GetSimilarTicketsByTextValidator : AbstractValidator<GetSimilarTicketsByTextQuery>
{
    public GetSimilarTicketsByTextValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
