using Corgibytes.Freshli.Agent.DotNet.Lib;
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

        Assert.Equal(12, manifestFiles.Length);
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
}
