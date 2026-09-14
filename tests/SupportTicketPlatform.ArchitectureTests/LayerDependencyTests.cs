using SupportTicketPlatform.Application;
using SupportTicketPlatform.Domain.Common;
using SupportTicketPlatform.Infrastructure;
using NetArchTest.Rules;

namespace SupportTicketPlatform.ArchitectureTests;

public class LayerDependencyTests
{
    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Application()
    {
        var result = Types.InAssembly(typeof(BaseEntity).Assembly).ShouldNot().HaveDependencyOn("SupportTicketPlatform.Application").GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(BaseEntity).Assembly).ShouldNot().HaveDependencyOn("SupportTicketPlatform.Infrastructure").GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Api()
    {
        var result = Types.InAssembly(typeof(BaseEntity).Assembly).ShouldNot().HaveDependencyOn("SupportTicketPlatform.Api").GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(ApplicationServicesRegistration).Assembly).ShouldNot().HaveDependencyOn("SupportTicketPlatform.Infrastructure").GetResult();

        Assert.True(result.IsSuccessful);
    }
    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Api()
    {
        var result = Types.InAssembly(typeof(ApplicationServicesRegistration).Assembly).ShouldNot().HaveDependencyOn("SupportTicketPlatform.Api").GetResult();

        Assert.True(result.IsSuccessful);
    }
    [Fact]
    public void Infrastructure_Should_Not_Have_Dependency_On_Api()
    {
        var result = Types.InAssembly(typeof(InfrastructureServicesRegistration).Assembly).ShouldNot().HaveDependencyOn("SupportTicketPlatform.Api").GetResult();

        Assert.True(result.IsSuccessful);
    }
}
