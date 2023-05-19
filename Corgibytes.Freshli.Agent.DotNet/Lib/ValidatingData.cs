namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ValidatingData
{
    static readonly List<string> _packageUrls = new()
    {
        "pkg:nuget/Corgibytes.Freshli.Lib@0.5.0",
        "pkg:nuget/System.CommandLine@2.0.0-beta4.22272.1",
        "pkg:nuget/Microsoft.CSharp@4.7.0",
    };

    static readonly List<string> _repositoryUrls = new()
    {
        "https://github.com/corgibytes/freshli-fixture-csharp-test",
    };

    public static List<string> PackageUrls() => _packageUrls;

    public static List<string> RepositoryUrls() => _repositoryUrls;
}
