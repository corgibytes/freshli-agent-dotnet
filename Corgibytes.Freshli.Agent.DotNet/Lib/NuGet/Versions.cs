using System.Xml;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class Versions
{
    private static readonly ILogger<Versions> s_logger = Logging.Logger<Versions>();
    private static readonly ManifestDetector s_manifestDetector = new();
    public const string BackupSuffix = ".versionsBackup";

    public static void UpdateManifest(string manifestFilePath, DateTimeOffset asOfDate)
    {
        s_logger.LogTrace("Update({ManifestFilePath}, {AsOfDate})", manifestFilePath, asOfDate);
        File.Copy(manifestFilePath, manifestFilePath + BackupSuffix, true);
        var manifest = GetManifest(manifestFilePath);
        var repository = GetRepository(manifestFilePath);

        var xmldoc = new XmlDocument();
        xmldoc.Load(manifestFilePath);
        manifest.Parse(xmldoc);
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

            var releaseHistory = repository.GetReleaseHistory(node.Name, false)
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
                manifest.Update(xmldoc, node.Name, latestVersion.ToString());
                manifestIsDirty = true;
                s_logger.LogDebug("Updating {PackageId} to version = {Version} published on {PublishedOn}",
                    node.Name, latestVersion, latestPublished);
            }
        }

        if (manifestIsDirty)
        {
            xmldoc.Save(manifestFilePath);
        }
    }

    public static void RestoreManifest(string manifestFilePath)
    {
        if (File.Exists(manifestFilePath + BackupSuffix))
        {
            File.Move(manifestFilePath + BackupSuffix, manifestFilePath, true);
        }
    }

    public static IPackageRepository GetRepository(string manifestFilePath)
    {
        var finders = s_manifestDetector.ManifestFinders(manifestFilePath);
        foreach (var finder in finders)
        {
            return finder.RepositoryFor(manifestFilePath);
        }

        return new NuGetRepository();
    }

    public static IManifest GetManifest(string manifestFilePath)
    {
        var finders = s_manifestDetector
            .ManifestFinders(manifestFilePath)
            .First(finder => finder.IsFinderFor(manifestFilePath));
        return finders.ManifestFor(manifestFilePath);
    }
}
