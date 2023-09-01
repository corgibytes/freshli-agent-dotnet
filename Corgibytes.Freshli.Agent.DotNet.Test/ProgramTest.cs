using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test;

public class ProgramTest
{
    [Fact]
    public void DetectManifests()
    {
        var path = "./../../../";
        var command = "detect-manifests";
        var exitCode = Program.Main(command, path);
        Assert.Equal(0, exitCode);
    }
}
