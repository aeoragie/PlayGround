var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PlayGround_Server>("playground-server");

builder.Build().Run();
