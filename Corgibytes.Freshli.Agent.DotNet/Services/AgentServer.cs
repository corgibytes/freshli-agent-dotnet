using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        _application = builder.Build();


        _application.MapGrpcService<AgentService>();
        _application.MapGet("/",
            () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
        );
        if (_application.Environment.IsDevelopment())
        {
            _application.MapGrpcReflectionService();
        }

        _application.Run();
        s_instance = this;
    }

    public void stop()
    {
        Task.Run(async () => { await _application!.DisposeAsync(); });
    }

    public static AgentServer? Instance() => s_instance;
}
