using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class ProcessManifestTest
{
    [Fact]
    public void Run_Project()
    {
        var projectFile = Fixtures.Path("csproj", "Project.csproj");
        var processManifest = new ProcessManifest();
        processManifest.Run(projectFile, DateTimeOffset.Parse("2023-09-05T00:00:00.0000000Z"));
    }

    [Fact]
    public void Run_Packages()
    {
        var projectFile = Fixtures.Path("config", "packages.config");
        var processManifest = new ProcessManifest();
        processManifest.Run(projectFile, DateTimeOffset.Parse("2023-09-05T00:00:00.0000000Z"));
    }
}
