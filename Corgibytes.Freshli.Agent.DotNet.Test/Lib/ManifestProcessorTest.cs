using System.Text.Json;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestProcessorTest
{
    private readonly ManifestProcessor _manifestProcessor = new();

    [Fact]
    public void ProcessProjectManifest()
    {
        string path = Fixtures.Path("csproj", "Project.csproj");
        var projectFile = new DirectoryInfo(path);
        string analysisPath = projectFile.FullName;
        string bomFilePath = _manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Now.AddMonths(-3));
        Assert.NotEmpty(bomFilePath);

        string expectedBomFilePath = projectFile.Parent!.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
        Assert.True(File.Exists(bomFilePath));
        Dictionary<string, JsonElement>? json =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(bomFilePath));
        Assert.Equal(7, json?.Count);
        JsonElement components = json!["components"];
        Assert.Equal(79, components.GetArrayLength());
        File.Delete(expectedBomFilePath);
    }

    [Fact]
    public void ProcessPackagesManifest()
    {
        string path = Fixtures.Path("config", "packages.config");

        var projectFile = new DirectoryInfo(path);
        string analysisPath = projectFile.FullName;
        string bomFilePath = _manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Now.AddMonths(-3));
        Assert.NotEmpty(bomFilePath);

        string expectedBomFilePath = projectFile.Parent!.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
        Assert.True(File.Exists(bomFilePath));
        Dictionary<string, JsonElement>? json =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(bomFilePath));
        Assert.Equal(7, json?.Count);
        JsonElement components = json!["components"];
        Assert.Equal(4, components.GetArrayLength());
        File.Delete(expectedBomFilePath);
    }

    [Fact]
    public void ProcessOpserverManifest()
    {
        string path = Fixtures.Path("config", "Opserver.Core", "packages.config");
        var asOfDate = DateTimeOffset.Parse("2015-05-01T00:00:00.0000Z");
        string bomPath = _manifestProcessor.ProcessManifest(path, asOfDate);
        Assert.Empty(bomPath);
    }
}
