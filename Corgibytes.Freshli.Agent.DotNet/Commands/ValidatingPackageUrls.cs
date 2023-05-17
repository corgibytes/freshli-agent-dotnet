using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Corgibytes.Freshli.Agent.DotNet.Commands;

public class ValidatingPackageUrls : Command
{
    readonly List<string> _packageUrls = new()
    {
        "pkg:nuget/Corgibytes.Freshli.Lib@0.5.0",
        "pkg:nuget/System.CommandLine@2.0.0-beta4.22272.1",
        "pkg:nuget/Microsoft.CSharp@4.7.0",
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
