using System.Reflection;

namespace Corgibytes.Freshli.Agent.DotNet.Test;

public static class Fixtures
{
    public static string Path(params string[] values)
    {

        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var components = new List<string>()
        {
            Directory.GetParent(assemblyPath)!.ToString(),
            "Fixtures"
        };
        components.AddRange(values);

        return System.IO.Path.Combine(components.ToArray());
    }
}
