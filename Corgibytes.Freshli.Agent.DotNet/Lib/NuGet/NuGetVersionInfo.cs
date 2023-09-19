using System.Text.RegularExpressions;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

/**
    *  This serves as a wrapper for the 'NuGetVersion' class in the NuGet
    *  Client SDK.
    */
public partial class NuGetVersionInfo : IVersionInfo
{
    private readonly IPackageSearchMetadata _packageSearchMetadata;
    private readonly IEnumerable<IPackageSearchMetadata> _siblingReleases;

    public NuGetVersionInfo(IPackageSearchMetadata packageSearchMetadata, IEnumerable<IPackageSearchMetadata> siblingReleases)
    {
        _packageSearchMetadata = packageSearchMetadata;
        _siblingReleases = siblingReleases;
    }

    private NuGetVersion NuGetVersion => _packageSearchMetadata.Identity.Version;

    private static readonly Regex s_versionMetadataRegex = VersionMetadataRegex();

    private static string StripVersionMetadata(string version)
    {
        return s_versionMetadataRegex.Replace(version, "");
    }

    private static string SanitizeVersion(string version)
    {
        return StripVersionMetadata(version);
    }

    public string Version =>
        SanitizeVersion(NuGetVersion.OriginalVersion ?? NuGetVersion.ToString());

    public bool IsPreRelease => NuGetVersion.IsPrerelease;

    public DateTimeOffset DatePublished
    {
        get
        {
            if (_packageSearchMetadata.IsListed)
            {
                return _packageSearchMetadata.Published!.Value.UtcDateTime;
            }

            // TODO: Log a warning that we're approximating the release date of an unlisted version
            return ApproximateDatePublished();
        }
    }

    private DateTimeOffset ApproximateDatePublished()
    {
        var listedReleases = _siblingReleases
            .Where(r => r.IsListed)
            .ToList();

        var previousRelease = listedReleases
            .Where(r => r.Identity.Version < NuGetVersion)
            .MaxBy(r => r.Identity.Version);

        var nextRelease = listedReleases
            .Where(r => r.Identity.Version > NuGetVersion)
            .MinBy(r => r.Identity.Version);

        if (previousRelease == null && nextRelease == null)
        {
            // there are no listed releases, use the unlisted release date even though it's going to
            // skew the results
            // TODO: Log a warning that we were unable to approximate the release date
            return _packageSearchMetadata.Published!.Value.UtcDateTime;
        }

        if (previousRelease == null && nextRelease != null)
        {
            // the version we're checking is the first version, use the date of the next release
            return nextRelease.Published!.Value.UtcDateTime;
        }

        if (nextRelease == null && previousRelease != null)
        {
            // the version we're checking is the latest version, use the date of the previous release
            return previousRelease.Published!.Value.UtcDateTime;
        }

        // interpolate the date between the previous and next release
        var previousReleaseDate = previousRelease!.Published!.Value.UtcDateTime;
        var nextReleaseDate = nextRelease!.Published!.Value.UtcDateTime;

        if (previousReleaseDate > nextReleaseDate)
        {
            (nextReleaseDate, previousReleaseDate) = (previousReleaseDate, nextReleaseDate);
        }

        var span = nextReleaseDate - previousReleaseDate;
        return previousReleaseDate + span / 2.0;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not NuGetVersionInfo other)
        {
            throw new ArgumentException(
                "NuGetVersionInfo not provided for CompareTo()", nameof(obj)
            );
        }

        return NuGetVersion.CompareTo(other.NuGetVersion);
    }

    [GeneratedRegex("\\+[a-zA-Z0-9]+")]
    private static partial Regex VersionMetadataRegex();
}
