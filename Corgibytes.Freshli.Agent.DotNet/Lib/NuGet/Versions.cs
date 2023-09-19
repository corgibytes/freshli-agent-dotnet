using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class Versions
{
    private static readonly ILogger<Versions> s_logger = Logging.Logger<Versions>();

    public static async void UpdateManifest(string manifestFilePath, DateTimeOffset asOfDate)
    {
        if (manifestFilePath.EndsWith(".config"))
        {
            return;
        }

        s_logger.LogTrace("Update({ManifestFilePath}, {AsOfDate})", manifestFilePath, asOfDate);
        var manifest = new NuGetManifest(manifestFilePath);
        if (manifest.Count <= 0)
        {
            return;
        }

        foreach (var node in manifest)
        {
            var versionRange = VersionRange.Parse(node.Version);
            if (!versionRange.HasLowerAndUpperBounds)
            {
                continue;
            }

            s_logger.LogTrace("Package {PackageId} has specified version range = {Version}",
                node.Name, versionRange.ToNormalizedString());

            var shouldRetrievePreReleasePackages = versionRange.MinVersion.IsPrerelease;
            var repository = new NuGetRepository();
            var releaseHistory = (await repository.GetReleaseHistory(node.Name, shouldRetrievePreReleasePackages))
                .Where(release => release.DatePublished <= asOfDate);

            var latestVersion = new NuGetVersion(0, 0, 0);
            DateTimeOffset? latestPublished = null;
            foreach (var release in releaseHistory)
            {
                var releaseVersion = new NuGetVersion(release.Version);
                if (releaseVersion.CompareTo(latestVersion) > 0 &&
                    versionRange.Satisfies(releaseVersion))
                {
                    latestVersion = releaseVersion;
                    latestPublished = release.DatePublished;
                }
            }

            if (latestVersion.CompareTo(new NuGetVersion(0, 0, 0)) != 0)
            {
                manifest.Update(node.Name, latestVersion.ToString());
                s_logger.LogDebug("Updating {PackageId} to version = {Version} published on {PublishedOn}",
                    node.Name, latestVersion, latestPublished);
            }
        }

        manifest.Save();
    }

    public static void RestoreManifest(string manifestFilePath)
    {
        var manifest = new NuGetManifest(manifestFilePath);
        manifest.Restore();
    }
}
