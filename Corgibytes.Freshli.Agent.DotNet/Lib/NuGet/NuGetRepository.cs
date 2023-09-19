using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetRepository
{
    private readonly IDictionary<string, IEnumerable<NuGetVersionInfo>> _packages
        = new Dictionary<string, IEnumerable<NuGetVersionInfo>>();

    public async Task<IEnumerable<IVersionInfo>> GetReleaseHistory(
        string name,
        bool includePreReleaseVersions
    )
    {
        if (_packages.TryGetValue(name, out var history))
        {
            return history;
        }

        var versions = (await GetMetadata(name, includePreReleaseVersions)).ToList();
        _packages[name] = versions
            .OrderByDescending(nv => nv.Published)
            .Select(v => new NuGetVersionInfo(v, versions));

        return _packages[name];
    }

    private static async Task<IEnumerable<IPackageSearchMetadata>> GetMetadata(string name, bool includePreReleaseVersions)
    {
        var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;

        var cache = new SourceCacheContext();
        var repository = Repository.Factory.GetCoreV3(
            "https://api.nuget.org/v3/index.json"
        );
        var resource =
            await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);

        var packages = await resource.GetMetadataAsync(
            name,
            includePrerelease: includePreReleaseVersions,
            includeUnlisted: true,
            cache,
            logger,
            cancellationToken);

        return packages;
    }
}
