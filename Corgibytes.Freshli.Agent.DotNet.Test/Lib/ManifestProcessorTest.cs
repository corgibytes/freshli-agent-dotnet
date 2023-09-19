using System.Text.Json;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestProcessorTest
{
    private readonly ManifestProcessor _manifestProcessor = new();

    [Fact]
    public async Task ProcessProjectManifest()
    {
        var path = Fixtures.Path("csproj", "Project.csproj");
        var projectFile = new DirectoryInfo(path);
        var analysisPath = projectFile.FullName;
        var bomFilePath = await _manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Parse("2023-09-06T00:00:00.0000000Z"));
        Assert.NotEmpty(bomFilePath);

        var expectedBomFilePath = projectFile.Parent!.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
        Assert.True(File.Exists(bomFilePath));
        var json =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(await File.ReadAllTextAsync(bomFilePath));
        Assert.Equal(7, json?.Count);
        var components = json!["components"];
        Assert.Equal(79, components.GetArrayLength());
        File.Delete(expectedBomFilePath);
    }

    [Fact]
    public async Task ProcessPackagesManifest()
    {
        var path = Fixtures.Path("config", "packages.config");

        var projectFile = new DirectoryInfo(path);
        var analysisPath = projectFile.FullName;
        var bomFilePath = await _manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Parse("2023-09-05T00:00:00.0000000Z"));
        Assert.NotEmpty(bomFilePath);

        var expectedBomFilePath = projectFile.Parent!.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
        Assert.True(File.Exists(bomFilePath));
        var json =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(await File.ReadAllTextAsync(bomFilePath));
        Assert.Equal(7, json?.Count);
        var components = json!["components"];
        Assert.Equal(4, components.GetArrayLength());
        File.Delete(expectedBomFilePath);
    }

    [Fact]
    public async Task ProcessOpserverManifest()
    {
        // This manifest file results in an error, because one of the packages is no longer
        // listed. The CycloneDX DotNet tool attempts to run NuGet restore on the manifest
        // file, and that process fails when the package is unlisted.
        var path = Fixtures.Path("config", "Opserver.Core", "packages.config");
        var asOfDate = DateTimeOffset.Parse("2015-05-01T00:00:00.0000Z");
        await Assert.ThrowsAsync<ManifestProcessingException>(async () =>
        {
            await _manifestProcessor.ProcessManifest(path, asOfDate);
        });
    }
}
