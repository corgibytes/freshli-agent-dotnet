using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test;

public class ProgramTest
{
    [Fact]
    public void DetectManifests()
    {
        string path = "./../../../";
        string command = "detect-manifests";
        Task.Run(async () => await Program.Main(command, path));
    }
}
