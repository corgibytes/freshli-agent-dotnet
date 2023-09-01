using System.Collections.Immutable;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class FileHistoryService
{
    private readonly IFileHistoryFinderRegistry _registry;

    public FileHistoryService(IFileHistoryFinderRegistry registry)
    {
        _registry = registry;
    }

    public IFileHistoryFinder SelectFinderFor(string projectRootPath)
    {
        var matchingFinders = _registry.Finders.ToImmutableList()
            .Where(finder => finder.DoesPathContainHistorySource(projectRootPath));
        foreach (var finder in matchingFinders)
        {
            return finder;
        }

        throw new FileHistoryFinderNotFoundException(projectRootPath);
    }
}
