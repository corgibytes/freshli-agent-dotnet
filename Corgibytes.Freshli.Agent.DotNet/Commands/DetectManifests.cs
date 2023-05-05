using System.CommandLine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class DetectManifests : Command
{
    public ManifestService ManifestService { get; private set; }
    public IFileHistoryFinderRegistry FileHistoryFinderRegistry { get; private set; }

    public DetectManifests() : base("detect-manifests", "Detects manifest files in the specified directory")
    {
        ManifestFinderRegistry.RegisterAll();

        // TODO: The file history registry should be injected
        FileHistoryFinderRegistry = new FileHistoryFinderRegistry();
        FileHistoryFinderRegistry.Register<GitFileHistoryFinder>();
        FileHistoryFinderRegistry.Register<LocalFileHistoryFinder>();

        // TODO: inject this dependency
        ManifestService = new ManifestService();
    }
}
