using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using PackageUrl;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public sealed class ReleaseHistoryRetriever
{
    public List<PackageReleaseData> Retrieve(string packageUrl)
    {
        var purl = new PackageURL(packageUrl);
        IEnumerable<IPackageSearchMetadata> packageMetadata = GetMetadata(purl.Name);
        var results = packageMetadata
            .Select<IPackageSearchMetadata, PackageReleaseData>(metadata =>
                new PackageReleaseData(
                    metadata.Identity.Version.ToString(),
                    metadata.Published!.Value.DateTime.ToUniversalTime())).ToList();
        return results;
    }

    /**
     * This is a duplicate of Corgibytes.Freshli.Lib:Languages/CSharp/NuGetRepository.GetMetadata()
     * but since that function is not public it can't be used directly. It could be good to make that
     * function public and then reuse it.
     */
    private static IEnumerable<IPackageSearchMetadata> GetMetadata(string name)
    {
        CancellationToken cancellationToken = CancellationToken.None;
        var cache = new SourceCacheContext();
        SourceRepository repository = Repository.Factory.GetCoreV3(
            "https://api.nuget.org/v3/index.json"
        );
        PackageMetadataResource resource =
            repository.GetResourceAsync<PackageMetadataResource>(cancellationToken).Result;

        IEnumerable<IPackageSearchMetadata> packages = resource.GetMetadataAsync(
            name,
            includePrerelease: true,
            includeUnlisted: true,
            cache,
            NullLogger.Instance,
            cancellationToken).Result;

        return packages;
    }
}
