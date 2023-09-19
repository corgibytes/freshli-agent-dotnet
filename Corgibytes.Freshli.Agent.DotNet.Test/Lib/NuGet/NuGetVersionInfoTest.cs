using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Xunit;
using Moq;
using NuGet.Packaging.Core;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class NuGetVersionInfoTest
{
    private static IPackageSearchMetadata BuildMockPackageSearchMetadata(string version, DateTimeOffset releaseDate)
    {
        var mockPackageSearchMetadata = new Mock<IPackageSearchMetadata>();
        var packageIdentity = new PackageIdentity("MockPackage", new NuGetVersion(version));
        mockPackageSearchMetadata.Setup(mock => mock.Identity).Returns(packageIdentity);
        mockPackageSearchMetadata.Setup(mock => mock.IsListed).Returns(true);
        mockPackageSearchMetadata.Setup(mock => mock.Published).Returns(releaseDate);
        return mockPackageSearchMetadata.Object;
    }

    private static DateTimeOffset UnlistedPackagePublishedDate => DateTimeOffset.Parse("1900-01-01T00:00:00Z");

    private static IPackageSearchMetadata BuildMockUnlistedPackageSearchMetadata(string version)
    {
        var mockPackageSearchMetadata = new Mock<IPackageSearchMetadata>();
        var packageIdentity = new PackageIdentity("MockUnlistedPackage", new NuGetVersion(version));
        mockPackageSearchMetadata.Setup(mock => mock.Identity).Returns(packageIdentity);
        mockPackageSearchMetadata.Setup(mock => mock.IsListed).Returns(false);
        mockPackageSearchMetadata.Setup(mock => mock.Published).Returns(UnlistedPackagePublishedDate);
        return mockPackageSearchMetadata.Object;
    }

    private static NuGetVersionInfo BuildNuGetVersionInfoFrom(string version)
    {
        return BuildNuGetVersionInfoFrom(version, DateTimeOffset.UtcNow);
    }

    private static NuGetVersionInfo BuildNuGetVersionInfoFrom(string version, DateTimeOffset releaseDate)
    {
        return BuildNuGetVersionInfoFrom(version, releaseDate, Array.Empty<IPackageSearchMetadata>());
    }

    private static NuGetVersionInfo BuildNuGetVersionInfoFrom(string version, DateTimeOffset releaseDate, IEnumerable<IPackageSearchMetadata> releasesMetadata)
    {
        return new NuGetVersionInfo(
            BuildMockPackageSearchMetadata(version, releaseDate),
            releasesMetadata
        );
    }

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
        int expected
    )
    {
        var versionInfo1 = BuildNuGetVersionInfoFrom(version1);
        var versionInfo2 = BuildNuGetVersionInfoFrom(version2);

        Assert.Equal(expected, versionInfo1.CompareTo(versionInfo2));
    }

    [Theory]
    [InlineData("1.0.0-RC", true)]
    [InlineData("1.0.0", false)]
    public void IdentifiesPreRelease(
        string version,
        bool expected
    )
    {
        var versionInfo = BuildNuGetVersionInfoFrom(version);

        Assert.Equal(expected, versionInfo.IsPreRelease);
    }

    [Fact]
    public void ProvidesVersion()
    {
        const string version = "1.0.0";
        var versionInfo = BuildNuGetVersionInfoFrom(version);

        Assert.Equal(version, versionInfo.Version);
    }

    [Fact]
    public void ThrowsExceptionIfNull()
    {
        Assert.Throws<ArgumentException>(() =>
            BuildNuGetVersionInfoFrom(null!)
        );
    }

    [Fact]
    public void ThrowsExceptionIfNonMatchingType()
    {
        Assert.Throws<ArgumentException>(() =>
            BuildNuGetVersionInfoFrom("1.0.0").CompareTo(new object())
        );
    }

    [Theory]
    [MemberData(nameof(DatePublishedData))]
    public void DatePublished(string _, IPackageSearchMetadata versionMetadata, IList<IPackageSearchMetadata> releasesMetadata, DateTimeOffset expectedReleaseDate)
    {
        var versionInfo = new NuGetVersionInfo(versionMetadata, releasesMetadata);

        Assert.Equal(expectedReleaseDate, versionInfo.DatePublished);
    }

    public static IEnumerable<object[]> DatePublishedData() =>
        new List<object[]>
        {
            new object[]
            {
                "Listed Package",
                BuildMockPackageSearchMetadata("1.0.0", DateTimeOffset.Parse("2021-01-01T00:00:00Z")),
                Array.Empty<IPackageSearchMetadata>(),
                DateTimeOffset.Parse("2021-01-01T00:00:00Z")
            },

            new object[]
            {
                "Unlisted package, empty release history",
                BuildMockUnlistedPackageSearchMetadata("1.0.0"),
                Array.Empty<IPackageSearchMetadata>(),
                UnlistedPackagePublishedDate
            },

            new object[]
            {
                "Unlisted package, only unlisted releases",
                BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                new[]
                {
                    BuildMockUnlistedPackageSearchMetadata("1.0.0"),
                    BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                    BuildMockUnlistedPackageSearchMetadata("1.0.2"),
                },
                UnlistedPackagePublishedDate
            },

            new object[]
            {
                "Unlisted package as first release",
                BuildMockUnlistedPackageSearchMetadata("1.0.0"),
                new[]
                {
                    BuildMockUnlistedPackageSearchMetadata("1.0.0"),
                    BuildMockPackageSearchMetadata("1.0.1", DateTimeOffset.Parse("2022-01-01T00:00:00Z")),
                },
                DateTimeOffset.Parse("2022-01-01T00:00:00Z")
            },

            new object[]
            {
                "Unlisted package as last release",
                BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                new[]
                {
                    BuildMockPackageSearchMetadata("1.0.0", DateTimeOffset.Parse("2022-01-01T00:00:00Z")),
                    BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                },
                DateTimeOffset.Parse("2022-01-01T00:00:00Z")
            },

            new object[]
            {
                "Unlisted package as middle release",
                BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                new[]
                {
                    BuildMockPackageSearchMetadata("1.0.0", DateTimeOffset.Parse("2022-01-01T00:00:00Z")),
                    BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                    BuildMockPackageSearchMetadata("1.0.2", DateTimeOffset.Parse("2022-01-02T00:00:00Z")),
                },
                DateTimeOffset.Parse("2022-01-01T12:00:00Z")
            },

            new object[]
            {
                "Unlisted package as middle release and between unlisted packages",
                BuildMockUnlistedPackageSearchMetadata("1.0.2"),
                new[]
                {
                    BuildMockPackageSearchMetadata("1.0.0", DateTimeOffset.Parse("2022-01-01T00:00:00Z")),
                    BuildMockUnlistedPackageSearchMetadata("1.0.1"),
                    BuildMockUnlistedPackageSearchMetadata("1.0.2"),
                    BuildMockUnlistedPackageSearchMetadata("1.0.3"),
                    BuildMockPackageSearchMetadata("1.0.4", DateTimeOffset.Parse("2022-01-02T00:00:00Z")),
                },
                DateTimeOffset.Parse("2022-01-01T12:00:00Z")
            },


        };
}
