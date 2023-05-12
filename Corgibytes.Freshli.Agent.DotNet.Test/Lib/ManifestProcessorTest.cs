using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestProcessorTest
{

    [Fact]
    public void ProcessManifest()
    {
        var projectFile = new DirectoryInfo("./../../../Corgibytes.Freshli.Agent.DotNet.Test.csproj");
        string analysisPath = projectFile.FullName;
        var manifestProcessor = new ManifestProcessor();
        string bomFilePath = manifestProcessor.ProcessManifest(analysisPath, DateTimeOffset.Now.AddMonths(-3));
        Assert.NotEmpty(bomFilePath);

        string expectedBomFilePath = projectFile.Parent.FullName + "/obj/bom.json";
        Assert.Equal(expectedBomFilePath, bomFilePath);
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
