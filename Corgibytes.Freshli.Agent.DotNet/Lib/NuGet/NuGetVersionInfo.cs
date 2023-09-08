using NuGet.Versioning;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

/**
    *  This serves as a wrapper for the 'NuGetVersion' class in the NuGet
    *  Client SDK.
    */
public class NuGetVersionInfo : IVersionInfo
{
    private readonly NuGetVersion _nuGetVersion;
    private int Major => _nuGetVersion.Major;
    private int Minor => _nuGetVersion.Minor;
    private int Patch => _nuGetVersion.Patch;

    public DateTimeOffset DatePublished { get; }

    public NuGetVersionInfo(
        NuGetVersion nuGetVersion,
        DateTimeOffset datePublished
    )
    {
        _nuGetVersion = nuGetVersion;
        DatePublished = datePublished;
    }

    public string Version => _nuGetVersion.ToString();

    public bool IsPreRelease => _nuGetVersion.IsPrerelease;

    public int CompareTo(object? obj)
    {
        if (obj is not NuGetVersionInfo other)
        {
            throw new ArgumentException("NuGetVersionInfo not provided " +
                                        "for CompareTo()");
        }

        var result = Major.CompareTo(other.Major);
        if (result != 0)
        {
            return result;
        }

        result = Minor.CompareTo(other.Minor);
        if (result != 0)
        {
            return result;
        }

        result = Patch.CompareTo(other.Patch);
        if (result != 0)
        {
            return result;
        }

        return 0;
    }
}
