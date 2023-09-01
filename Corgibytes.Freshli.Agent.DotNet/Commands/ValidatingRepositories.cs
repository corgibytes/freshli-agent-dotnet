using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingRepositories : Command
{
    public ValidatingRepositories() :
        base("validating-repositories",
            "Lists repositories that can be used to validate this agent")
    {
        Handler = CommandHandler.Create(Run);
    }

    public void Run()
    {
        var repositoryUrls = ValidatingData.RepositoryUrls();
        foreach (var repositoryUrl in repositoryUrls)
        {
            Console.WriteLine(repositoryUrl);
        }
    }
}
