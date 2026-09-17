var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SupportTicketPlatform_Api>("api");

builder.AddProject<Projects.SupportTicketPlatform_Worker>("worker");

builder.Build().Run();
