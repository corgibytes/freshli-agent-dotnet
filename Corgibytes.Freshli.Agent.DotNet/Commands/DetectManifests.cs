using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class DetectManifests : Command
{
    private ManifestService ManifestService { get; }
    private IFileHistoryFinderRegistry FileHistoryFinderRegistry { get; }

    public DetectManifests() : base("detect-manifests", "Detects manifest files in the specified directory")
    {
        var pathArgument =
            new Argument<DirectoryInfo>("path", "Source code repository path") { Arity = ArgumentArity.ExactlyOne };
        AddArgument(pathArgument);

        ManifestFinderRegistry.RegisterAll();

        FileHistoryFinderRegistry = new FileHistoryFinderRegistry();
        FileHistoryFinderRegistry.Register<GitFileHistoryFinder>();
        FileHistoryFinderRegistry.Register<LocalFileHistoryFinder>();

        ManifestService = new ManifestService();

        Handler = CommandHandler.Create<DirectoryInfo>(Run);
    }

    public void Run(DirectoryInfo path)
    {
        string analysisPath = path.FullName;
        var fileHistoryService = new FileHistoryService(FileHistoryFinderRegistry);
        IFileHistoryFinder? fileHistoryFinder = fileHistoryService.SelectFinderFor(analysisPath);

        IEnumerable<AbstractManifestFinder>? manifestFinders = ManifestService.SelectFindersFor(analysisPath, fileHistoryFinder);
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
