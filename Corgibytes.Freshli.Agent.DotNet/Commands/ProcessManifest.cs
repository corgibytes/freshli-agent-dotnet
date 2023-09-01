using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ProcessManifest : Command
{
    private ManifestProcessor ManifestProcessor { get; }

    public ProcessManifest() :
        base("process-manifest", "Processes manifest files in the specified directory")
    {
        var manifestFileArgument = new Argument<string>("manifestFile", "Manifest file to be processed")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        AddArgument(manifestFileArgument);
        var asOfDate = new Argument<DateTimeOffset>("asOfDate") { Arity = ArgumentArity.ZeroOrOne };
        AddArgument(asOfDate);

        Handler = CommandHandler.Create<string, DateTimeOffset?>(Run);

        ManifestProcessor = new ManifestProcessor();
    }

    public void Run(string manifestFile, DateTimeOffset? asOfDate)
    {
        var bomFilePath = ManifestProcessor.ProcessManifest(manifestFile, asOfDate);
        Console.WriteLine(bomFilePath);
    }
}
