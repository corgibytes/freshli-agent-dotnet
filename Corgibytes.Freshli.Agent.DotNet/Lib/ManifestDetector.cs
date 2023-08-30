using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestDetector
{
    private readonly ILogger<ManifestDetector> _logger = Logging.Logger<ManifestDetector>();

    private IFileHistoryFinderRegistry FileHistoryFinderRegistry { get; }
    private FileHistoryService FileHistoryService { get; }
    private ManifestFinderRegistry ManifestFinderRegistry { get; }

    public ManifestDetector()
    {
        ManifestFinderRegistry = new ManifestFinderRegistry();
        ManifestFinderRegistry.Register<NuGetManifestFinder>();
        ManifestFinderRegistry.Register<PackagesManifestFinder>();

        FileHistoryFinderRegistry = new FileHistoryFinderRegistry();
        FileHistoryFinderRegistry.Register<GitFileHistoryFinder>();
        FileHistoryFinderRegistry.Register<LocalFileHistoryFinder>();

        FileHistoryService = new FileHistoryService(FileHistoryFinderRegistry);
    }

    public IEnumerable<AbstractManifestFinder> ManifestFinders(string analysisPath)
    {
        _logger.LogDebug("ManifestFinders({AnalysisPath})", analysisPath);
        IFileHistoryFinder? fileHistoryFinder = FileHistoryService.SelectFinderFor(analysisPath);
        if (fileHistoryFinder is null)
        {
            throw new ManifestProcessingException();
        }

        IEnumerable<AbstractManifestFinder> manifestFinders =
            ManifestFinderRegistry.Finders
                .Select(finder =>
                {
                    finder.FileFinder = fileHistoryFinder;
                    return finder;
                });
        return manifestFinders;
    }

    public IEnumerable<string> FindManifests(string analysisPath)
    {
        _logger.LogDebug("FindManifests({AnalysisPath})", analysisPath);
        IList<string> manifests = new List<string>();
        foreach (AbstractManifestFinder finder in ManifestFinders(analysisPath))
        {
            manifests.AddRange(finder.GetManifestFilenames(analysisPath));
        }

        return manifests.Order();
    }
}
