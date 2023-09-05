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

        var result = System.IO.Path.Combine(components.ToArray());

        if (!Directory.Exists(result) && !File.Exists(result))
        {
            throw new FixtureNotFoundException(
                "The specified file or directory was not found within the `Fixtures` directory tree", result);
        }

        return result;
    }

    private class FixtureNotFoundException : FileNotFoundException
    {
        public FixtureNotFoundException(string message, string value) : base(message, value)
        {
        }
    }
}
