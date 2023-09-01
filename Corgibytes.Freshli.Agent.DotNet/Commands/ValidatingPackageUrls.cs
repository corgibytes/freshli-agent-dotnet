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
        var packageUrls = ValidatingData.PackageUrls();
        foreach (var packageUrl in packageUrls)
        {
            Console.WriteLine(packageUrl);
        }
    }
}
