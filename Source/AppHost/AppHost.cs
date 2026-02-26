var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Server>("playground-server");

builder.Build().Run();
