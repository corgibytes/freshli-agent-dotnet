using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingRepositories : Command
{
    readonly List<string> _repositoryUrls = new()
    {
        "https://github.com/dotnet/docs",
        "https://github.com/questdb/questdb",
        "https://github.com/protocolbuffers/protobuf",
        "https://github.com/corgibytes/freshli-fixture-csharp-test",
    };

    public ValidatingRepositories() :
        base("validating-repositories",
            "Lists repositories that can be used to validate this agent")
    {
        Handler = CommandHandler.Create(Run);
    }

    public void Run()
    {
        foreach (string repositoryUrl in _repositoryUrls)
        {
            Console.WriteLine(repositoryUrl);
        }
    }
}
