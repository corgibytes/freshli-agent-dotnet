using System.Text.Json;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestProcessorTest
{
    private readonly ManifestProcessor _manifestProcessor = new();

    [Fact]
    public void ProcessProjectManifest()
    {
        var path = Fixtures.Path("csproj", "Project.csproj");
        var projectFile = new DirectoryInfo(path);
        var analysisPath = projectFile.FullName;
        var bomFilePath = _manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Now.AddMonths(-3));
        Assert.NotEmpty(bomFilePath);

        var expectedBomFilePath = projectFile.Parent!.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
        Assert.True(File.Exists(bomFilePath));
        var json =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(bomFilePath));
        Assert.Equal(7, json?.Count);
        var components = json!["components"];
        Assert.Equal(79, components.GetArrayLength());
        File.Delete(expectedBomFilePath);
    }

    [Fact]
    public void ProcessPackagesManifest()
    {
        var path = Fixtures.Path("config", "packages.config");

        var projectFile = new DirectoryInfo(path);
        var analysisPath = projectFile.FullName;
        var bomFilePath = _manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Now.AddMonths(-3));
        Assert.NotEmpty(bomFilePath);

        var expectedBomFilePath = projectFile.Parent!.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
        Assert.True(File.Exists(bomFilePath));
        var json =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(bomFilePath));
        Assert.Equal(7, json?.Count);
        var components = json!["components"];
        Assert.Equal(4, components.GetArrayLength());
        File.Delete(expectedBomFilePath);
    }

    [Fact]
    public void ProcessOpserverManifest()
    {
        // This manifest file results in an error, because one of the packages is no longer
        // listed. The CylcloneDX DotNet tool attempts to run NuGet restore on the manifest
        // file, and that process fails when the package is unlisted.
        var path = Fixtures.Path("config", "Opserver.Core", "packages.config");
        var asOfDate = DateTimeOffset.Parse("2015-05-01T00:00:00.0000Z");
        Assert.Throws<ManifestProcessingException>(() =>
        {
            _manifestProcessor.ProcessManifest(path, asOfDate);
        });
    }
}
