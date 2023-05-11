using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class DetectManifests : Command
{
    private ManifestDetector ManifestDetector { get; }

    public DetectManifests() : base("detect-manifests", "Detects manifest files in the specified directory")
    {
        var pathArgument =
            new Argument<DirectoryInfo>("path", "Source code repository path") { Arity = ArgumentArity.ExactlyOne };
        AddArgument(pathArgument);
        ManifestDetector = new ManifestDetector();

        Handler = CommandHandler.Create<DirectoryInfo>(Run);
    }

    public void Run(DirectoryInfo path)
    {
        string analysisPath = path.FullName;
        IEnumerable<AbstractManifestFinder> manifestFinders = ManifestDetector.ManifestFinders(analysisPath);
        foreach (AbstractManifestFinder manifestFinder in manifestFinders)
        {
            foreach (string? manifestFile in manifestFinder.GetManifestFilenames(analysisPath))
            {
                Console.WriteLine(
                    "{0}", manifestFile.Replace(analysisPath, "")
                );
            }
        }
    }
}
