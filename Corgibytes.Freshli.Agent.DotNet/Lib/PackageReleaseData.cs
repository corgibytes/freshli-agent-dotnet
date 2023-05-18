namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class PackageReleaseData
{
    private static readonly string s_releasedAtFormat = "yyyy-MM-ddTHH:mm:ssK";

    public PackageReleaseData(string version, DateTime releasedAt)
    {
        Version = version;
        ReleasedAt = releasedAt;
    }

    private string Version { get; }
    private DateTime ReleasedAt { get; }

    public override string ToString() => $"{Version}\t{ReleasedAt.ToString(s_releasedAtFormat)}";
}
