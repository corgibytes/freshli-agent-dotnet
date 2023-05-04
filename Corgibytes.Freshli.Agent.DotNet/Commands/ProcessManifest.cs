using System.CommandLine;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ProcessManifest : Command
{
    public ProcessManifest() : 
        base("process-manifest", "Processes manifest files in the specified directory")
    {
    }
}