using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public sealed class RetrieveReleaseHistory : Command
{
    public RetrieveReleaseHistory() :
        base("retrieve-release-history", "Retrieves release history for a specific package")
    {
        var packageUrl = new Argument<string>("packageUrl", "Manifest file to be processed")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        AddArgument(packageUrl);
        Handler = CommandHandler.Create<string>(Run);
    }

    private void Run(string packageUrl)
    {
        new ReleaseHistoryRetriever()
            .Retrieve(packageUrl)
            .ForEach(release => Console.WriteLine(release.ToString()));
    }
}
