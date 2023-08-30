using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Services;
using Microsoft.AspNetCore.Connections;

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

    private int Run(int port)
    {
        var server = new AgentServer(port);
        try
        {
            server.Start();
        }
        catch (IOException error)
        {
            if (error.InnerException is AddressInUseException)
            {
                Console.WriteLine($"Unable to start the gRPC service. Port {port} is in use.");
                return -1;
            }

            throw;
        }

        return 0;
    }
}
