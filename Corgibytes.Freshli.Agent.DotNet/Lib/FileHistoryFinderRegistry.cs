using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Lib
{
    public class FileHistoryFinderRegistry : IFileHistoryFinderRegistry
    {
        private readonly IList<IFileHistoryFinder> _finders = new List<IFileHistoryFinder>();

        public IList<IFileHistoryFinder> Finders => _finders;

        public void Register<TFinder>() where TFinder : IFileHistoryFinder, new()
        {
            Finders.Add(new TFinder());
        }
    }
}
