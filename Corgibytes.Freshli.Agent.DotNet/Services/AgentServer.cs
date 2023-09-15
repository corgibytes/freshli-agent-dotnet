using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Services;

public class AgentServer
{
    // ReSharper disable all
    public int Port { get; set; }

    // ReSharper disable all
    private WebApplication? _application;

    public AgentServer(int port) => Port = port;

    public void Start()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions() { ContentRootPath = assemblyPath });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, Port, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        builder.Services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.Zero;
            options.Period = TimeSpan.FromSeconds(10);
        });

        builder.Logging.ClearProviders().AddConsole();

        builder.Services.AddGrpc(configure => configure.EnableDetailedErrors = true);
        builder.Services.AddGrpcHealthChecks(configure =>
        {
            configure.Services.Map("", result => true);
            configure.Services.Map("com.corgibytes.freshli.agent.Agent", result => true);
        })
            .AddCheck("com.corgibytes.freshli.agent.Agent", () => HealthCheckResult.Healthy());

        builder.Services.AddGrpcReflection();

        builder.Services.AddSingleton(this);

        _application = builder.Build();
        _application.MapGrpcService<AgentService>();
        _application.MapGrpcHealthChecksService();
        _application.MapGrpcReflectionService();

        _application.Run($"http://0.0.0.0:{Port}");
    }

    public void Stop()
    {
        Task.Run(async () =>
        {
            await _application!.StopAsync();
        });
    }
}
