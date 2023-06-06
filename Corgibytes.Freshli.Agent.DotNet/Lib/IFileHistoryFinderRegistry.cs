namespace Corgibytes.Freshli.Agent.DotNet.Lib
{
    public interface IFileHistoryFinderRegistry
    {
        IList<IFileHistoryFinder> Finders { get; }

        void Register<TFinder>() where TFinder : IFileHistoryFinder, new();
    }
}
