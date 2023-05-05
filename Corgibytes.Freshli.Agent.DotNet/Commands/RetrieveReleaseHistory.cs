using System.CommandLine;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class RetrieveReleaseHistory : Command
{
    public RetrieveReleaseHistory() :
        base("retrieve-release-history", "Retrieves release history for a specific package")
    {
    }
}
