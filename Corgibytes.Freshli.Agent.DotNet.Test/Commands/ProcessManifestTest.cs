using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class ProcessManifestTest
{

    [Fact]
    public void Run()
    {
        var projectFile = new DirectoryInfo("./../../../Corgibytes.Freshli.Agent.DotNet.Test.csproj");
        var processManifest = new ProcessManifest();
        processManifest.Run(projectFile.FullName, DateTimeOffset.Now.AddMonths(-3));
    }
}
