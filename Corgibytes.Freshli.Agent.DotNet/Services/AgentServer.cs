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
    private static AgentServer? s_instance;

    // ReSharper disable all
    public int Port { get; set; }

    // ReSharper disable all
    private WebApplication? _application;

    public AgentServer(int port) => Port = port;

    public void Start()
    {
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        WebApplicationBuilder builder = WebApplication.CreateBuilder(
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
            configure.Services.MapService("", result => true);
            configure.Services.MapService("com.corgibytes.freshli.agent.Agent", result => true);
        })
            .AddCheck("com.corgibytes.freshli.agent.Agent", () => HealthCheckResult.Healthy());

        builder.Services.AddGrpcReflection();

        _application = builder.Build();
        _application.MapGrpcService<AgentService>();
        _application.MapGrpcHealthChecksService();
        _application.MapGrpcReflectionService();

        _application.Run($"http://0.0.0.0:{Port}");
        s_instance = this;
    }

    public void Stop()
    {
        Task.Run(async () => { await _application!.DisposeAsync(); });
    }

    public static AgentServer? Instance() => s_instance;
}
