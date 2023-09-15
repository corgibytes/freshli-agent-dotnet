using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class Versions
{
    private static readonly ILogger<Versions> s_logger = Logging.Logger<Versions>();
    public const string BackupSuffix = ".versionsBackup";

    public static void UpdateManifest(string manifestFilePath, DateTimeOffset asOfDate)
    {
        if (manifestFilePath.EndsWith(".config"))
        {
            return;
        }

        s_logger.LogTrace("Update({ManifestFilePath}, {AsOfDate})", manifestFilePath, asOfDate);
        File.Copy(manifestFilePath, manifestFilePath + BackupSuffix, true);
        var manifest = new NuGetManifest();
        var repository = new NuGetRepository();

        manifest.Parse(File.ReadAllText(manifestFilePath));
        if (manifest.Count <= 0)
        {
            return;
        }

        var manifestIsDirty = false;
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
            var releaseHistory = repository.GetReleaseHistory(node.Name, shouldRetrievePreReleasePackages)
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
                manifestIsDirty = true;
                s_logger.LogDebug("Updating {PackageId} to version = {Version} published on {PublishedOn}",
                    node.Name, latestVersion, latestPublished);
            }
        }

        if (manifestIsDirty)
        {
            manifest.Save(manifestFilePath);
        }
    }

    public static void RestoreManifest(string manifestFilePath)
    {
        if (File.Exists(manifestFilePath + BackupSuffix))
        {
            File.Move(manifestFilePath + BackupSuffix, manifestFilePath, true);
        }
    }
}
