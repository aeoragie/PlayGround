var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PlayGround_Server>("Playground-Server");

builder.Build().Run();
