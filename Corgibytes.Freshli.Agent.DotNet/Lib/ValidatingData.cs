namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public static class ValidatingData
{
    public static List<string> PackageUrls() => new()
    {
        "pkg:nuget/Corgibytes.Freshli.Lib@0.5.0",
        "pkg:nuget/System.CommandLine@2.0.0-beta4.22272.1",
        "pkg:nuget/Microsoft.CSharp@4.7.0",
    };

    public static List<string> RepositoryUrls() => new()
    {
        "https://github.com/corgibytes/freshli-fixture-csharp-test",
    };
}
