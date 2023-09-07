using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class DetectManifestsTest
{
    [Fact]
    public void Run_onProjectPath()
    {
        var path = Fixtures.Path();
        var directoryInfo = new DirectoryInfo(path);

        // Ensure command executes without error
        new DetectManifests().Run(directoryInfo);
    }
}
