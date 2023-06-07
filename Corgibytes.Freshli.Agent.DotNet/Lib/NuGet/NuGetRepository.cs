using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetRepository : IPackageRepository
{
    private readonly IDictionary<string, IEnumerable<NuGetVersionInfo>> _packages
        = new Dictionary<string, IEnumerable<NuGetVersionInfo>>();

    private IEnumerable<IVersionInfo> GetReleaseHistory(
        string name,
        bool includePreReleaseVersions
    )
    {
        if (_packages.TryGetValue(name, out IEnumerable<NuGetVersionInfo>? history))
        {
            return history;
        }

        IEnumerable<IPackageSearchMetadata> versions = GetMetadata(name);
        _packages[name] = versions
            .OrderByDescending(nv => nv.Published)
            .Select(v => new NuGetVersionInfo(
                v.Identity.Version,
                v.Published!.Value.UtcDateTime
            ));

        return _packages[name];
    }

    private IEnumerable<IPackageSearchMetadata> GetMetadata(string name)
    {
        ILogger logger = NullLogger.Instance;
        CancellationToken cancellationToken = CancellationToken.None;

        var cache = new SourceCacheContext();
        SourceRepository repository = Repository.Factory.GetCoreV3(
            "https://api.nuget.org/v3/index.json"
        );
        PackageMetadataResource resource =
            repository.GetResourceAsync<PackageMetadataResource>().Result;

        IEnumerable<IPackageSearchMetadata> packages = resource.GetMetadataAsync(
            name,
            includePrerelease: true,
            includeUnlisted: false,
            cache,
            logger,
            cancellationToken).Result;

        return packages;
    }

    public IVersionInfo Latest(
        string name,
        DateTimeOffset asOf,
        bool includePreReleases)
    {
        try
        {
            return GetReleaseHistory(name, includePreReleases)
                .First(v => asOf >= v.DatePublished);
        }
        catch (Exception e)
        {
            throw new LatestVersionNotFoundException(name, asOf, e);
        }
    }

    public IVersionInfo VersionInfo(string name, string version)
    {
        try
        {
            return GetReleaseHistory(name, includePreReleaseVersions: true)
                .First(v => v.Version == version);
        }
        catch (Exception e)
        {
            throw new VersionNotFoundException(name, version, e);
        }
    }

    public List<IVersionInfo> VersionsBetween(
        string name,
        DateTimeOffset asOf,
        IVersionInfo earlierVersion,
        IVersionInfo laterVersion,
        bool includePreReleases)
    {
        return GetReleaseHistory(name, includePreReleases)
            .Where(v => asOf >= v.DatePublished)
            .Where(predicate: v => v.CompareTo(earlierVersion) == 1)
            .Where(predicate: v => v.CompareTo(laterVersion) == -1)
            .ToList();
    }

    public IVersionInfo Latest(
        string name,
        DateTimeOffset asOf,
        string thatMatches
    )
    {
        throw new NotImplementedException();
    }
}
