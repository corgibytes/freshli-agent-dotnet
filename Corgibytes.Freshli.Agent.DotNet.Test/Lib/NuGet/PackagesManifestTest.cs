using System.Xml;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class PackagesManifestTest
{
    private const string TestContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <packages>
            <package id=""DotNetEnv"" version=""1.4.0"" />
            <package id=""Elasticsearch.Net"" version=""7.10"" />
            <package id=""HtmlAgilityPack"" version=""1.11.30"" />
            <package id=""LibGit2Sharp"" version=""0.27.0"" />
            <package id=""NLog"" version=""4.7.7"" />
            <package id=""RestSharp"" version=""106.11.7"" />
            </packages>";

    [Fact]
    public void ParsesFile()
    {
        var manifest = new PackagesManifest();
        manifest.Parse(TestContent);
        Assert.Equal(6, manifest.Count);
    }

    [Fact]
    public void Update()
    {
        var manifest = new PackagesManifest();
        manifest.Parse(TestContent);
        manifest.Update("NLog", "5.2.0");

        var tempFile = Path.GetTempFileName();
        manifest.Save(tempFile);

        var xmldoc = new XmlDocument();
        xmldoc.Load(tempFile);
        var node = xmldoc.SelectSingleNode($"*/{PackagesManifest.Element}[@{PackagesManifest.NameAttribute} = 'NLog']");
        Assert.Equal("5.2.0", node!.Attributes![PackagesManifest.VersionAttribute]!.Value);
    }
}
