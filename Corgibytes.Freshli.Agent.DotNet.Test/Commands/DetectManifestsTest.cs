using Corgibytes.Freshli.Agent.DotNet.Commands;
using FluentAssertions;
using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class DetectManifestsTest
{
    [Fact]
    public void Run_onProjectPath()
    {
        StringWriter consoleOutput = new();
        TextWriter previousOut = Console.Out;
        Console.SetOut(consoleOutput);

        string path = "./../../../";
        DirectoryInfo? directoryInfo = new DirectoryInfo(path).Parent;
        if (directoryInfo == null)
        {
            Assert.Fail($"Could not read {path} as directory");
        }
        new DetectManifests().Run(directoryInfo);

        string agentProjectFile = "Corgibytes.Freshli.Agent.DotNet/Corgibytes.Freshli.Agent.DotNet.csproj";
        string testProjectFile = "Corgibytes.Freshli.Agent.DotNet.Test/Corgibytes.Freshli.Agent.DotNet.Test.csproj";
        string expected =
            $"{agentProjectFile}{Environment.NewLine}{testProjectFile}{Environment.NewLine}";
        consoleOutput.ToString().Should().Be(expected);

        Console.SetOut(previousOut);
    }
}
