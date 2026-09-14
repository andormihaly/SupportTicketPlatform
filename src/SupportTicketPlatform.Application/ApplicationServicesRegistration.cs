using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SupportTicketPlatform.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationServicesRegistration).Assembly);
        return services;
    }
}
