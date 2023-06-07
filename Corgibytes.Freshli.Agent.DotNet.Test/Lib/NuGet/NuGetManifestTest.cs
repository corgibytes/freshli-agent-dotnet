using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class NuGetManifestTest
{
    [Fact]
    public void ParsesFile()
    {
        var manifest = new NuGetManifest();
        string testContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
            <ItemGroup>
            <PackageReference Include=""DotNetEnv"" Version=""1.4.0"" />
            <PackageReference Include=""Elasticsearch.Net"" Version=""7.10"" />
            <PackageReference Include=""HtmlAgilityPack"" Version=""1.11.30"" />
            <PackageReference Include=""LibGit2Sharp"" Version=""0.27.0"" />
            <PackageReference Include=""NLog"" Version=""4.7.7"" />
            <PackageReference Include=""RestSharp"" Version=""106.11.7"" />
            </ItemGroup>
        </Project>";

        manifest.Parse(testContent);

        Assert.Equal(6, manifest.Count);
    }
}
