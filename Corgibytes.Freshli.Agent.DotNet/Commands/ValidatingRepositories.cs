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
        List<string> repositoryUrls = ValidatingData.RepositoryUrls();
        foreach (string repositoryUrl in repositoryUrls)
        {
            Console.WriteLine(repositoryUrl);
        }
    }
}
