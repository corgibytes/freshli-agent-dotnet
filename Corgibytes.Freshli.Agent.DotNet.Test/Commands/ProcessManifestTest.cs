using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class ProcessManifestTest
{
    [Fact]
    public void Run_Project()
    {
        string projectFile = Fixtures.Path("csproj", "Project.csproj");
        var processManifest = new ProcessManifest();
        processManifest.Run(projectFile, DateTimeOffset.Now.AddMonths(-3));
    }

    [Fact]
    public void Run_Packages()
    {
        string projectFile = Fixtures.Path("config", "packages.config");
        var processManifest = new ProcessManifest();
        processManifest.Run(projectFile, DateTimeOffset.Now.AddMonths(-3));
    }
}
