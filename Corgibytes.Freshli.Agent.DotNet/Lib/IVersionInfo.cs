namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public interface IVersionInfo : IComparable
{
    public string Version { get; }

    public DateTimeOffset DatePublished { get; }

    // ReSharper disable once UnusedMemberInSuper.Global
    public bool IsPreRelease { get; }
}
