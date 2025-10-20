using MatchScoop.Console;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();

builder.Logging.AddConsole();
builder.Services.AddApplicationServices();

var host = builder.Build();

await host.RunAsync();
