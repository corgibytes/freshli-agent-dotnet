using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class VersionsTest
{
    [Theory]
    [MemberData(nameof(UpdateNuGetManifestArgs))]
    public void UpdateNuGetManifest(string[] manifestFixturePath, string date, PackageInfo[] expectedUpdates)
    {
        var manifestFilePath = Fixtures.Path(manifestFixturePath);
        File.WriteAllText(manifestFilePath, File.ReadAllText(manifestFilePath));
        Versions.UpdateManifest(manifestFilePath, DateTimeOffset.Parse(date));
        try
        {
            var manifest = new NuGetManifest();
            manifest.Parse(File.ReadAllText(manifestFilePath));
            foreach (var expected in expectedUpdates)
            {
                var packageInfo = manifest[expected.Name];
                Assert.Equal(expected.Version, packageInfo.Version);
            }
        }
        finally
        {
            Versions.RestoreManifest(manifestFilePath);
            Assert.False(File.Exists(manifestFilePath + Versions.BackupSuffix));
            if (File.Exists(manifestFilePath))
            {
                File.Delete(manifestFilePath);
            }
        }
    }

    [Theory]
    [MemberData(nameof(UpdatePackagesManifestArgs))]
    public void UpdatePackagesManifest(string[] manifestFixturePath, string date)
    {
        var manifestFilePath = Fixtures.Path(manifestFixturePath);
        var expectedHash = Hash(File.ReadAllText(manifestFilePath));

        Versions.UpdateManifest(manifestFilePath, DateTimeOffset.Parse(date));
        try
        {
            var actualHash = Hash(File.ReadAllText(manifestFilePath));
            Assert.Equal(expectedHash, actualHash);
        }
        finally
        {
            Versions.RestoreManifest(manifestFilePath);
            Assert.False(File.Exists(manifestFilePath + Versions.BackupSuffix));
            if (File.Exists(manifestFilePath))
            {
                File.Delete(manifestFilePath);
            }
        }
    }

    private static string Hash(string input)
    {
        return Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(input)));
    }

    public static IEnumerable<object?[]> UpdateNuGetManifestArgs =>
        new List<object?[]>
        {
            // If passing no arguments, the default git path should be 'git'
            new object?[]
            {
                new[] {"csproj", "Project.csproj" }, "2017-12-10T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.1.0") }
            },
            new object?[]
            {
                new[] { "csproj", "Project.csproj" }, "2019-12-05T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.2.0") }
            },
            new object?[]
            {
                new[] { "csproj", "Project.csproj" }, "2019-12-06T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.3.1") }
            },
            new object?[]
            {
                new[] { "csproj", "Project.csproj" }, "2023-09-05T00:00:00.0000000Z",
                new[] { new PackageInfo("DotNetEnv", "1.4.0") }
            },
            new object?[]
            {
                new[] { "csproj", "Project.csproj" }, "2023-09-05T00:00:00.0000000Z",
                new[] { new PackageInfo("NLog", "4.7.7") }
            },
        };

    public static IEnumerable<object?[]> UpdatePackagesManifestArgs =>
        new List<object?[]>
        {
            new object?[]
            {
                new[] { "config", "packages.config" }, "2015-06-10T00:00:00.0000000Z",
            },
            new object?[]
            {
                new[] { "config", "packages.config" }, "2019-12-31T00:00:00.0000000Z",
            },
            new object?[]
            {
                new[] { "config", "packages.config" }, "2023-09-05T00:00:00.0000000Z",
            }
        };

}
