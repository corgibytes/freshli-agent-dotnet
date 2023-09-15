using System.Xml;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class NuGetManifestTest
{
    [Fact]
    public void ParsesFile()
    {
        var manifest = new NuGetManifest(Fixtures.Path("csproj-simple", "Project.csproj"));
        Assert.Equal(6, manifest.Count);
    }

    [Fact]
    public void Update()
    {
        var manifestPath = Fixtures.Path("csproj-simple", "Project.csproj");
        var manifest = new NuGetManifest(manifestPath);
        manifest.Update("NLog", "5.2.0");

        manifest.Save();

        var xml = new XmlDocument();
        xml.Load(manifestPath);
        var node = xml.SelectSingleNode($"/Project/ItemGroup/{NuGetManifest.Element}[@{NuGetManifest.NameAttribute} = 'NLog']");
        Assert.Equal("5.2.0", node!.Attributes![NuGetManifest.VersionAttribute]!.Value);
    }
}
