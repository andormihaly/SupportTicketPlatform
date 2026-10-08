using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using SupportTicketPlatform.Api.ExceptionHandling;
using SupportTicketPlatform.Api.Validation;
using SupportTicketPlatform.Application;
using SupportTicketPlatform.Application.Configuration;
using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Infrastructure;
using SupportTicketPlatform.Infrastructure.Messaging;
using SupportTicketPlatform.Persistence;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);



var keyVaultUri = builder.Configuration["KeyVaultUri"];

if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

var appConfigurationEndpoint = builder.Configuration["AppConfiguration:Endpoint"];


if (!string.IsNullOrWhiteSpace(appConfigurationEndpoint))
{
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(appConfigurationEndpoint), credential).ConfigureKeyVault(keyVault =>{keyVault.SetCredential(credential);});
    });
}

builder.Services.Configure<ApiOptions>(builder.Configuration);

builder.AddServiceDefaults();

builder.Host.UseWolverine(options =>
{
    options.Discovery.IncludeAssembly(typeof(AssemblyReference).Assembly);
    options.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
});

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;

    if (string.IsNullOrWhiteSpace(options.ServiceBus.FullyQualifiedNamespace))
    {
        throw new InvalidOperationException( "ServiceBus:FullyQualifiedNamespace is not configured.");
    }

    return new ServiceBusClient(options.ServiceBus.FullyQualifiedNamespace, new DefaultAzureCredential());
});


builder.Services.AddSingleton<ITicketMessagePublisher, ServiceBusTicketMessagePublisher>();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers(options => options.Filters.Add<FluentValidationActionFilter>());
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(options => { });

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
