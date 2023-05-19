using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Agent.DotNet.Lib;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingPackageUrls : Command
{

    public ValidatingPackageUrls() :
        base("validating-package-urls",
            "Lists package urls that can be used to validate this agent")
    {
        Handler = CommandHandler.Create(Run);
    }

    public void Run()
    {
        List<string> _packageUrls = ValidatingData.PackageUrls();
        foreach (string packageUrl in _packageUrls)
        {
            Console.WriteLine(packageUrl);
        }
    }
}
