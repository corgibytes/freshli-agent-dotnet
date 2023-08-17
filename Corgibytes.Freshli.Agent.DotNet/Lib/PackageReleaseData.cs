namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class PackageReleaseData
{
    private static readonly string s_releasedAtFormat = "yyyy-MM-ddTHH:mm:ssK";

    public PackageReleaseData(string version, DateTime releasedAt)
    {
        Version = version;
        ReleasedAt = releasedAt;
    }

    public string Version { get; }
    public DateTime ReleasedAt { get; }

    public override string ToString() => $"{Version}\t{ReleasedAt.ToString(s_releasedAtFormat)}";
}
