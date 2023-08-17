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
        foreach (IFileHistoryFinder finder in _registry.Finders.ToImmutableList()
                     .Where(finder => finder.DoesPathContainHistorySource(projectRootPath)))
        {
            return finder;
        }

        throw new FileHistoryFinderNotFoundException(projectRootPath);
    }
}
