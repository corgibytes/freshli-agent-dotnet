using System.CommandLine;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingPackageUrls : Command
{
    public ValidatingPackageUrls() :
        base("validating-package-urls",
            "Lists package urls that can be used to validate this agent")
    {
    }
}