using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using PackageUrl;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public sealed class ReleaseHistoryRetriever
{
    private readonly NuGetRepository _nuGetRepository = new();

    public async Task<List<PackageReleaseData>> Retrieve(string packageUrl)
    {
        try
        {
            var purl = new PackageURL(packageUrl);

            var releaseHistory = await _nuGetRepository.GetReleaseHistory(purl.Name, true);
            var results = releaseHistory.Select(release =>
                new PackageReleaseData(
                    release.Version,
                    release.DatePublished)
            ).OrderBy(value => value.ReleasedAt).ToList();
            return results;
        }
        catch (MalformedPackageUrlException)
        {
            return new List<PackageReleaseData>();
        }
    }
}
