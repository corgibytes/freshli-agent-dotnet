using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib.NuGet;

public class PackagesManifestFinderTest
{
    private readonly AbstractManifestFinder _manifestFinder = new PackagesManifestFinder();

    [Fact]
    public void IsPackagesFinder()
    {
        var path = Fixtures.Path("config", "packages.config");
        Assert.True(_manifestFinder.IsFinderFor(path));
    }

    [Fact]
    public void IsNotPackagesFinder()
    {
        var path = Fixtures.Path("csproj", "Project.csproj");
        Assert.False(_manifestFinder.IsFinderFor(path));
    }
}
