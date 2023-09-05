using System.Xml;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class VersionsTest
{
    [Theory]
    [MemberData(nameof(UpdateNuGetManifestArgs))]
    public void UpdateNuGetManifest(string manifestFilePath, string date, PackageInfo[] expectedUpdates)
    {
        Versions.UpdateManifest(manifestFilePath, DateTimeOffset.Parse(date));
        var xmldoc = new XmlDocument();
        xmldoc.Load(manifestFilePath);
        var manifest = new NuGetManifest();
        Assert.NotNull(manifest);
        manifest.Parse(xmldoc);
        foreach (var expected in expectedUpdates)
        {
            var packageInfo = manifest[expected.Name];
            Assert.Equal(expected.Version, packageInfo.Version);
        }

        Versions.RestoreManifest(manifestFilePath);
        Assert.False(File.Exists(manifestFilePath + Versions.BackupSuffix));
    }

    [Theory]
    [MemberData(nameof(UpdatePackagesManifestArgs))]
    public void UpdatePackagesManifest(string manifestFilePath, string date, PackageInfo[] expectedUpdates)
    {
        Versions.UpdateManifest(manifestFilePath, DateTimeOffset.Parse(date));
        var xmldoc = new XmlDocument();
        xmldoc.Load(manifestFilePath);
        var manifest = new PackagesManifest();
        Assert.NotNull(manifest);
        manifest.Parse(xmldoc);
        foreach (var expected in expectedUpdates)
        {
            var packageInfo = manifest[expected.Name];
            Assert.Equal(expected.Version, packageInfo.Version);
        }

        Versions.RestoreManifest(manifestFilePath);
        Assert.False(File.Exists(manifestFilePath + Versions.BackupSuffix));
    }

    public static IEnumerable<object?[]> UpdateNuGetManifestArgs =>
        new List<object?[]>
        {
            // If passing no arguments, the default git path should be 'git'
            new object?[]
            {
                Fixtures.Path("csproj", "Project.csproj"), "2017-12-10T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.1.0") }
            },
            new object?[]
            {
                Fixtures.Path("csproj", "Project.csproj"), "2019-12-05T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.2.0") }
            },
            new object?[]
            {
                Fixtures.Path("csproj", "Project.csproj"), "2019-12-06T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.3.1") }
            },
            new object?[]
            {
                Fixtures.Path("csproj", "Project.csproj"), "2023-09-05T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.4.0") }
            },
            new object?[]
            {
                Fixtures.Path("csproj", "Project.csproj"), "2023-09-05T00:00:00.0000000Z",
                new[] { new PackageInfo("NLog", "4.7.7") }
            },
        };

    public static IEnumerable<object?[]> UpdatePackagesManifestArgs =>
        new List<object?[]>
        {
            new object?[]
            {
                Fixtures.Path("config", "packages.config"), "2015-06-10T00:00:00.0000000Z",
                new[] { new PackageInfo("NLog", "4.0.0") }
            },
            new object?[]
            {
                Fixtures.Path("config", "packages.config"), "2019-12-31T00:00:00.0000000Z",
                new[] { new PackageInfo("NLog", "4.0.0") }
            },
            new object?[]
            {
                Fixtures.Path("config", "packages.config"), DateTimeOffset.Now.ToString("o"),
                new[] { new PackageInfo("NLog", "4.0.0") }
            }
        };

}
