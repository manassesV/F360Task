var builder = Host.CreateApplicationBuilder(args);

builder.AddConsumer();

var app = builder.Build();
await app.RunAsync();
