using System.Xml;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class VersionsTest
{
    [Theory]
    [MemberData(nameof(UpdateManifestArgs))]
    public void UpdateManifest(string manifestFilePath, string date, PackageInfo[] expectedUpdates)
    {
        Versions.UpdateManifest(manifestFilePath, DateTimeOffset.Parse(date));
        var xmldoc = new XmlDocument();
        xmldoc.Load(manifestFilePath);
        var manifest = Versions.GetManifest(manifestFilePath);
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

    public static IEnumerable<object?[]> UpdateManifestArgs =>
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
                Fixtures.Path("csproj", "Project.csproj"), DateTimeOffset.Now.ToString("o"),
                new[] { new PackageInfo("DotNetEnv", "1.4.0") }
            },
            // now assert that packages.config files are pinned appropriately
            new object?[]
            {
                Fixtures.Path("config", "packages.config"), "2015-06-10T00:00:00.0000000Z",
                new[] { new PackageInfo("NLog", "4.0.0") }
            },
            new object?[]
            {
                Fixtures.Path("config", "packages.config"), "2019-12-31T00:00:00.0000000Z",
                new[] { new PackageInfo("NLog", "4.6.8") }
            },
            new object?[]
            {
                Fixtures.Path("config", "packages.config"), DateTimeOffset.Now.ToString("o"),
                new[] { new PackageInfo("NLog", "4.7.15") }
            }
        };
}
