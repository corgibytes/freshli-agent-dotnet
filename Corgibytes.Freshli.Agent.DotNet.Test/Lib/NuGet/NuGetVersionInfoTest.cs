using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using NuGet.Versioning;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class NuGetVersionInfoTest
{
    [Theory]
    [InlineData("1", "2", -1)]
    [InlineData("1", "1", 0)]
    [InlineData("2", "1", 1)]
    [InlineData("1.1", "1", 1)]
    [InlineData("1", "1.0.0", 0)]
    [InlineData("2", "1.0.0", 1)]
    [InlineData("2.1", "2.1.0", 0)]
    [InlineData("2.1.0", "2.1.1", -1)]
    public void CompareToCorrectlySortsByVersion(
        string version1,
        string version2,
        int result
    )
    {
        var versionInfo1 = new NuGetVersionInfo(
            new NuGetVersion(version1), DateTimeOffset.UtcNow
        );
        var versionInfo2 = new NuGetVersionInfo(
            new NuGetVersion(version2), DateTimeOffset.UtcNow
        );

        Assert.Equal(versionInfo1.CompareTo(versionInfo2), result);
    }

    [Theory]
    [InlineData("1.0.0-RC", true)]
    [InlineData("1.0.0", false)]
    public void IdentifiesPreRelease(
        string version,
        bool result
    )
    {
        var versionInfo = new NuGetVersionInfo(
            new NuGetVersion(version), DateTimeOffset.UtcNow
        );

        Assert.Equal(versionInfo.IsPreRelease, result);
    }

    [Fact]
    public void ProvidesVersion()
    {
        var version = "1.0.0";
        var versionInfo = new NuGetVersionInfo(
            new NuGetVersion(version), DateTimeOffset.UtcNow
        );

        Assert.Equal(versionInfo.Version, version);
    }

    [Fact]
    public void ThrowsExceptionIfNull()
    {
        Assert.Throws<ArgumentException>
        (() => new NuGetVersionInfo(
            new NuGetVersion("1.0.0"),
            DateTimeOffset.UtcNow).CompareTo(null));
    }

    [Fact]
    public void ThrowsExceptionIfNonMatchingType()
    {
        Assert.Throws<ArgumentException>
        (() => new NuGetVersionInfo(
                new NuGetVersion("1.0.0"),
                DateTimeOffset.UtcNow).CompareTo(
                new NuGetVersion("1.0.0")
            )
        );
    }
}
