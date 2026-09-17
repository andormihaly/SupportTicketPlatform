using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SupportTicketPlatform.Worker.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseNpgsql(
          "Host=localhost;Database=supportticket;Username=design-time;Password=design-time",
          options => options.UseVector())
      .Options;

        return new AppDbContext(options, TimeProvider.System);
    }
}
