using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test;

public class ProgramTest
{
    [Fact]
    public void DetectManifests()
    {
        string path = "./../../../";
        string command = "detect-manifests";
        int exitCode = Program.Main(command, path);
        Assert.Equal(0, exitCode);
    }
}
