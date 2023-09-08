using System.CommandLine;


namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class MainCommand : RootCommand
{
    public MainCommand(string description = "") : base(description)
    {
        Add(new StartServer());
    }
}
