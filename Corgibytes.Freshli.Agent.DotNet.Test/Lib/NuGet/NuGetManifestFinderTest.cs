using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class NuGetManifestFinderTest
{
    private readonly AbstractManifestFinder _manifestFinder = new NuGetManifestFinder();

    [Fact]
    public void IsNuGetFinder()
    {
        string path = Fixtures.Path("csproj", "Project.csproj");
        Assert.True(_manifestFinder.IsFinderFor(path));
    }

    [Fact]
    public void IsNotNuGetFinder()
    {
        string path = Fixtures.Path("config", "packages.config");
        Assert.False(_manifestFinder.IsFinderFor(path));
    }
}
