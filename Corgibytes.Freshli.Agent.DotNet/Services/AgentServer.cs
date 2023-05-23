using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        WebApplicationBuilder builder = WebApplication.CreateBuilder(
            new WebApplicationOptions() { ContentRootPath = assemblyPath });
        builder.Logging.ClearProviders().AddConsole();
        builder.Services.AddGrpc();
        builder.Services.AddGrpcHealthChecks();
        builder.Services.AddGrpcReflection();

        _application = builder.Build();


        _application.MapGrpcService<AgentService>();
        _application.MapGet("/",
            () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
        );
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
