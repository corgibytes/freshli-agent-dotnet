using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using PackageUrl;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public sealed class ReleaseHistoryRetriever
{
    private readonly NuGetRepository _nuGetRepository = new();

    public List<PackageReleaseData> Retrieve(string packageUrl)
    {
        var purl = new PackageURL(packageUrl);

        IEnumerable<IVersionInfo> releaseHistory = _nuGetRepository.GetReleaseHistory(purl.Name, true);
        var results = releaseHistory.Select(release =>
            new PackageReleaseData(
                release.Version,
                release.DatePublished.DateTime.ToUniversalTime())
        ).ToList();
        return results;
    }
}
