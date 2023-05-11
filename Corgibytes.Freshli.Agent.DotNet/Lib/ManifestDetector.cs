using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestDetector
{
    private ManifestService ManifestService { get; }
    private IFileHistoryFinderRegistry FileHistoryFinderRegistry { get; }
    private FileHistoryService FileHistoryService { get; }

    public ManifestDetector()
    {
        ManifestFinderRegistry.RegisterAll();

        FileHistoryFinderRegistry = new FileHistoryFinderRegistry();
        FileHistoryFinderRegistry.Register<GitFileHistoryFinder>();
        FileHistoryFinderRegistry.Register<LocalFileHistoryFinder>();

        ManifestService = new ManifestService();
        FileHistoryService = new FileHistoryService(FileHistoryFinderRegistry);
    }

    public IEnumerable<AbstractManifestFinder> ManifestFinders(string analysisPath)
    {
        IFileHistoryFinder? fileHistoryFinder = FileHistoryService.SelectFinderFor(analysisPath);

        IEnumerable<AbstractManifestFinder>? manifestFinders =
            ManifestService.SelectFindersFor(analysisPath, fileHistoryFinder);
        return manifestFinders;
    }
}
