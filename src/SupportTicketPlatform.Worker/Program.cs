using Azure.Identity;
using Azure.Messaging.ServiceBus;
using SupportTicketPlatform.Worker;
using SupportTicketPlatform.Worker.Application;
using SupportTicketPlatform.Worker.Application.Configuration;
using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Infrastructure;
using Wolverine;
using Wolverine.FluentValidation;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<TicketWorker>();
builder.Services.AddHostedService<TicketAnalysisWorker>();

builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"]!), new DefaultAzureCredential());

var appConfigurationEndpoint = builder.Configuration["AppConfiguration:Endpoint"];

if (!string.IsNullOrWhiteSpace(appConfigurationEndpoint))
{
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(appConfigurationEndpoint), credential).ConfigureKeyVault(keyVault => { keyVault.SetCredential(credential); });
    });
}

builder.Services.Configure<WorkerOptions>(builder.Configuration);

var workerOptions = new WorkerOptions();
builder.Configuration.Bind(workerOptions);

builder.AddServiceDefaults();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton(_ =>
{
    var fullyQualifiedNamespace = workerOptions.ServiceBus.FullyQualifiedNamespace;

    if (string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
        throw new InvalidOperationException("ServiceBus:FullyQualifiedNamespace is not configured.");

    return new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
});

builder.Services.AddWolverine(options =>
{
    options.Discovery.IncludeAssembly(typeof(AssemblyReference).Assembly);
    options.CodeGeneration.AlwaysUseServiceLocationFor<ITicketRepository>();
    options.UseFluentValidation();
});

var host = builder.Build();

host.Run();
