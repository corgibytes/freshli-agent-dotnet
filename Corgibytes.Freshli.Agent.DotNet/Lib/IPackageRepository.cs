namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public interface IPackageRepository
{
    IVersionInfo VersionInfo(string name, string version);

    IVersionInfo Latest(
        string name,
        DateTimeOffset asOf,
        bool includePreReleases);

    IVersionInfo Latest(string name, DateTimeOffset asOf, string thatMatches);

    List<IVersionInfo> VersionsBetween(
        string name,
        DateTimeOffset asOf,
        IVersionInfo earlierVersion,
        IVersionInfo laterVersion,
        bool includePreReleases
    );

    public IEnumerable<IVersionInfo> GetReleaseHistory(
        string name,
        bool includePreReleaseVersions
    );

}
