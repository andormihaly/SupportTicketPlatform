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

builder.AddServiceDefaults();

builder.Services.AddHostedService<TicketWorker>();
builder.Services.AddHostedService<TicketAnalysisWorker>();

builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"]!), new DefaultAzureCredential());

builder.Services.Configure<WorkerOptions>(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton(_ => new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));


builder.Services.AddWolverine(options =>
{
    options.Discovery.IncludeAssembly(typeof(AssemblyReference).Assembly);
    options.CodeGeneration.AlwaysUseServiceLocationFor<ITicketRepository>();
    options.UseFluentValidation();
});

var host = builder.Build();

host.Run();
