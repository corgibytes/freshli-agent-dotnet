using System.CommandLine;


namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class MainCommand : RootCommand
{
    public MainCommand(string description = "") : base(description)
    {
        Add(new ValidatingPackageUrls());
        Add(new RetrieveReleaseHistory());
        Add(new ValidatingRepositories());
        Add(new DetectManifests());
        Add(new ProcessManifest());
        Add( new StartServer());
    }
}