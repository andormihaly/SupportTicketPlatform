using SupportTicketPlatform.Worker.Domain.Entities;

namespace SupportTicketPlatform.Worker.Application.Interfaces;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
