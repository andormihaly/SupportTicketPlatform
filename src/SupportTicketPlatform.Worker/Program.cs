using Azure.Identity;
using Azure.Messaging.ServiceBus;
using SupportTicketPlatform.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"]!), new DefaultAzureCredential());

builder.Services.AddSingleton(_ => new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));

var host = builder.Build();

host.Run();
