namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class LocalFileHistory : IFileHistory
{
    private readonly string _rootDirectory;
    private readonly string _targetPath;

    public LocalFileHistory(string rootDirectory, string targetPath)
    {
        _rootDirectory = rootDirectory;
        _targetPath = targetPath;
    }

    public IList<DateTimeOffset> Dates => new List<DateTimeOffset> {
        DateTimeOffset.UtcNow
    };

    public string ContentsAsOf(DateTimeOffset date)
    {
        return File.ReadAllText(Path.Combine(_rootDirectory, _targetPath));
    }

    public string ShaAsOf(DateTimeOffset date)
    {
        return "N/A";
    }
}