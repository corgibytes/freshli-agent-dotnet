using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingPackageUrls : Command
{
    readonly List<string> _packageUrls = new()
    {
        "pkg:nuget/org.nuget/packages/Corgibytes.Freshli.Lib",
        "pkg:nuget/org.nuget/packages/System.CommandLine",
        "pkg:nuget/org.nuget/packages/Microsoft.CSharp",
    };

    public ValidatingPackageUrls() :
        base("validating-package-urls",
            "Lists package urls that can be used to validate this agent")
    {
        Handler = CommandHandler.Create(Run);
    }

    public void Run()
    {
        foreach (string packageUrl in _packageUrls)
        {
            Console.WriteLine(packageUrl);
        }
    }
}
