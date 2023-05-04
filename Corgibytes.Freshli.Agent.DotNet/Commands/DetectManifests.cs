using System.CommandLine;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class DetectManifests : Command
{
    public DetectManifests() : 
        base("detect-manifests", "Detects manifest files in the specified directory")
    {
    }
}