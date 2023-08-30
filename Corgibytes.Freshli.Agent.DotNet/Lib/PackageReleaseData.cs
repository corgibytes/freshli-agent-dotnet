namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class PackageReleaseData
{
    private const string ReleasedAtFormat = "yyyy-MM-ddTHH:mm:ssK";

    public PackageReleaseData(string version, DateTimeOffset releasedAt)
    {
        Version = version;
        ReleasedAt = releasedAt;
    }

    public string Version { get; }
    public DateTimeOffset ReleasedAt { get; }

    public override string ToString() => $"{Version}\t{ReleasedAt.ToString(ReleasedAtFormat)}";
}
