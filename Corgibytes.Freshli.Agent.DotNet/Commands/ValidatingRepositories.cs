using System.CommandLine;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingRepositories : Command
{
    public ValidatingRepositories() :
        base("validating-repositories", "Lists repositories that can be used to validate this agent")
    {
    }
}
