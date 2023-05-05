using System.CommandLine;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class StartServer : Command
{
    public StartServer() :
        base("start-server", "Starts a gRPC server running the Freshli Agent service")
    {
    }
}
