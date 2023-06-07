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

    }

    [Fact]
    public void ExtractFile()
    {
        string output = @"
Found the following local nuget package cache locations:
    /home/username/.nuget/packages/

» Analyzing: /home/username/workspace/freshli-agent-dotnet/Corgibytes.Freshli.Agent.DotNet/Corgibytes.Freshli.Agent.DotNet.csproj
  Attempting to restore packages
  Packages restored
Retrieving GitHub license for repository corgibytes/freshli-lib - URL: https://api.github.com/repos/corgibytes/freshli-lib/license
Retrieving GitHub license for repository tonerdo/dotnet-env - URL: https://api.github.com/repos/tonerdo/dotnet-env/license
Retrieving GitHub license for repository zzzprojects/html-agility-pack and ref master - URL: https://api.github.com/repos/zzzprojects/html-agility-pack/license?ref=master
Retrieving GitHub license for repository libgit2/libgit2sharp - URL: https://api.github.com/repos/libgit2/libgit2sharp/license
Retrieving GitHub license for repository libgit2/libgit2sharp.nativebinaries.git - URL: https://api.github.com/repos/libgit2/libgit2sharp.nativebinaries.git/license
GitHub API failed with status code NotFound - will try again without '.git' on the repository name
Retrieving GitHub license for repository libgit2/libgit2sharp.nativebinaries - URL: https://api.github.com/repos/libgit2/libgit2sharp.nativebinaries/license
Retrieving GitHub license for repository dotnet/corefx and ref master - URL: https://api.github.com/repos/dotnet/corefx/license?ref=master
GitHub API failed with status code NotFound and message Not Found.
No license found on GitHub for repository dotnet/corefx using ref master
Retrieving GitHub license for repository dotnet/corefx - URL: https://api.github.com/repos/dotnet/corefx/license
GitHub API failed with status code NotFound and message Not Found.
No license found on GitHub for repository dotnet/corefx using ref
Retrieving GitHub license for repository sprache/Sprache and ref master - URL: https://api.github.com/repos/sprache/Sprache/license?ref=master
GitHub API failed with status code NotFound and message Not Found.
No license found on GitHub for repository sprache/Sprache using ref master
Retrieving GitHub license for repository sprache/Sprache - URL: https://api.github.com/repos/sprache/Sprache/license

Creating CycloneDX BOM
Writing to: /home/username/workspace/freshli-agent-dotnet/Corgibytes.Freshli.Agent.DotNet.Test/obj/bom.json
";

        string extractedFile = ManifestProcessor.ExtractFile(output);
        Assert.Equal(
            "/home/username/workspace/freshli-agent-dotnet/Corgibytes.Freshli.Agent.DotNet.Test/obj/bom.json",
            extractedFile
        );
    }
}
