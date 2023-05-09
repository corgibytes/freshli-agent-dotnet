using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class DetectManifestsTest
{
    [Fact]
    public void Run_onProjectPath()
    {
        string path = "./../../../";
        DirectoryInfo? directoryInfo = new DirectoryInfo(path).Parent;
        if (directoryInfo == null)
        {
            Assert.Fail($"Could not read {path} as directory");
        }

        // Ensure command executes without error
        new DetectManifests().Run(directoryInfo);
    }
}
