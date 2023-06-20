namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class FileHistoryFinderRegistry : IFileHistoryFinderRegistry
{
    public IList<IFileHistoryFinder> Finders { get; } = new List<IFileHistoryFinder>();

    public void Register<TFinder>() where TFinder : IFileHistoryFinder, new()
    {
        Finders.Add(new TFinder());
    }
}
