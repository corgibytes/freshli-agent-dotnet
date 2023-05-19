using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Services;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class StartServer : Command
{
    public StartServer() :
        base("start-server", "Starts a gRPC server running the Freshli Agent service")
    {
        var portArgument = new Argument<int>("port", "port number to run server on")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        AddArgument(portArgument);
        Handler = CommandHandler.Create<int>(Run);
    }

    private void Run(int port)
    {
        var server = new AgentServer(port);
        server.Start();
    }
}
