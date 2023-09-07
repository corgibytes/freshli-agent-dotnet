using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestDetectorTest
{
    private readonly ManifestDetector _manifestDetector = new();

    [Fact]
    public void FindManifests()
    {
        var analysisPath = Fixtures.Path();
        var manifests = _manifestDetector.FindManifests(analysisPath);
        var manifestFiles = manifests as string[] ?? manifests.ToArray();
        foreach (var manifestFile in manifestFiles)
        {
            Assert.Equal(Path.GetFullPath(manifestFile), manifestFile);
        }

        Assert.Equal(3, manifestFiles.Length);
    }

    [Theory]
    [InlineData("packages.config", true)]
    [InlineData("packages.xml", false)]
    [InlineData("packages.json", false)]
    [InlineData("SampleProject.csproj", true)]
    [InlineData("SampleProject.xml", false)]
    [InlineData("SampleProject.exproj", false)]
    public void IsManifestFile(string fileName, bool expected)
    {
        var actual = ManifestDetector.IsManifestFile(fileName);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("config/packages.config", typeof(PackagesManifest), 4)]
    [InlineData("csproj/Project.csproj", typeof(NuGetManifest), 5)]
    [InlineData("config/Opserver.Core/packages.config", typeof(PackagesManifest), 11)]
    public void LoadManifest(string fileName, Type expectedType, int expectedCount)
    {
        var manifestFile = Fixtures.Path(fileName);
        var manifest = ManifestDetector.LoadManifest(manifestFile);
        Assert.Equal(expectedType, manifest.GetType());
        Assert.Equal(expectedCount, manifest.Count);
    }
}
