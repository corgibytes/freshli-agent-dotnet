using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ReleaseHistoryRetrieverTest
{
    private readonly ITestOutputHelper _output;

    public ReleaseHistoryRetrieverTest(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Retrieve()
    {
        var packageReleases = new ReleaseHistoryRetriever()
            .Retrieve("pkg:nuget/Corgibytes.Freshli.Lib@0.5.0");
        Assert.Equal(38, packageReleases.Count);
        packageReleases.ForEach(release =>
            _output.WriteLine(release.ToString())
        );
    }
}
