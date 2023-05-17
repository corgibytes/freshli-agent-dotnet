using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Lib;
using Corgibytes.Freshli.Lib.Languages.CSharp;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestDetector
{
    private IFileHistoryFinderRegistry FileHistoryFinderRegistry { get; }
    private FileHistoryService FileHistoryService { get; }

    public ManifestDetector()
    {
        ManifestFinderRegistry.RegisterAll();

        FileHistoryFinderRegistry = new FileHistoryFinderRegistry();
        FileHistoryFinderRegistry.Register<GitFileHistoryFinder>();
        FileHistoryFinderRegistry.Register<LocalFileHistoryFinder>();

        FileHistoryService = new FileHistoryService(FileHistoryFinderRegistry);
    }

    public IEnumerable<AbstractManifestFinder> ManifestFinders(string analysisPath)
    {
        IFileHistoryFinder? fileHistoryFinder = FileHistoryService.SelectFinderFor(analysisPath);
        if (fileHistoryFinder is null)
        {
            throw new ManifestProcessingException();
        }

        IEnumerable<AbstractManifestFinder> manifestFinders =
            ManifestFinderRegistry.Finders
                .Where(finder => finder is NuGetManifestFinder)
                .Select(finder =>
                {
                    finder.FileFinder = fileHistoryFinder;
                    return finder;
                });
        return manifestFinders;
    }
}
