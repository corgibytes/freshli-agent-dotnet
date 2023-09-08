using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetRepository
{
    private readonly IDictionary<string, IEnumerable<NuGetVersionInfo>> _packages
        = new Dictionary<string, IEnumerable<NuGetVersionInfo>>();

    public IEnumerable<IVersionInfo> GetReleaseHistory(
        string name,
        bool includePreReleaseVersions
    )
    {
        if (_packages.TryGetValue(name, out var history))
        {
            return history;
        }

        var versions = GetMetadata(name, includePreReleaseVersions);
        _packages[name] = versions
            .OrderByDescending(nv => nv.Published)
            .Select(v => new NuGetVersionInfo(
                v.Identity.Version,
                v.Published!.Value.UtcDateTime
            ));

        return _packages[name];
    }

    private static IEnumerable<IPackageSearchMetadata> GetMetadata(string name, bool includePreReleaseVersions)
    {
        var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;

        var cache = new SourceCacheContext();
        var repository = Repository.Factory.GetCoreV3(
            "https://api.nuget.org/v3/index.json"
        );
        var resource =
            repository.GetResourceAsync<PackageMetadataResource>(cancellationToken).Result;

        var packages = resource.GetMetadataAsync(
            name,
            includePrerelease: includePreReleaseVersions,
            includeUnlisted: false,
            cache,
            logger,
            cancellationToken).Result;

        return packages;
    }
}
