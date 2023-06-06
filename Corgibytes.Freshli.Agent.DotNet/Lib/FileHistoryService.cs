using System.Collections.Immutable;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;

namespace Corgibytes.Freshli.Agent.DotNet.Lib
{
    public class FileHistoryService
    {
        private IFileHistoryFinderRegistry _registry;

        public FileHistoryService(IFileHistoryFinderRegistry registry)
        {
            _registry = registry;
        }
        public IFileHistoryFinder SelectFinderFor(string projectRootPath)
        {
            foreach (var finder in _registry.Finders.ToImmutableList())
            {
                if (finder.DoesPathContainHistorySource(projectRootPath))
                {
                    return finder;
                }
            }

            throw new FileHistoryFinderNotFoundException(projectRootPath);
        }
    }
}
