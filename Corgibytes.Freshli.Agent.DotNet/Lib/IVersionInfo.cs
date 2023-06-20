namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public interface IVersionInfo : IComparable
{
    public string Version { get; }

    public DateTimeOffset DatePublished { get; }

    public bool IsPreRelease { get; }
}
