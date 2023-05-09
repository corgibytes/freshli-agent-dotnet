using Corgibytes.Freshli.Agent.DotNet.Commands;
using FluentAssertions;
using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class DetectManifestsTest
{
    private readonly StringWriter _consoleOutput = new();

    public DetectManifestsTest() => Console.SetOut(_consoleOutput);

    [Fact]
    public void Run_onProjectPath()
    {
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
        _consoleOutput.ToString().Should().Be(expected);
    }
}
