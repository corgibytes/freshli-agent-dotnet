using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestDetector
{
    private readonly ILogger<ManifestDetector> _logger = Logging.Logger<ManifestDetector>();

    public IEnumerable<string> FindManifests(string analysisPath)
    {
        _logger.LogDebug("FindManifests({AnalysisPath})", analysisPath);

        IList<string> manifests = new List<string>();

        var dirTree = Directory.GetDirectories(Path.GetFullPath(analysisPath));
        var hashSet = new HashSet<string>(dirTree);

        while (hashSet.Count > 0)
        {
            var currentDir = hashSet.First();
            hashSet.Remove(currentDir);

            foreach (var file in Directory.GetFiles(currentDir))
            {
                if (IsManifestFile(file))
                {
                    manifests.Add(file);
                }
            }

            foreach (var dir in Directory.GetDirectories(currentDir))
            {
                hashSet.Add(dir);
            }
        }

        return manifests.Order();
    }

    public static AbstractManifest LoadManifest(string file)
    {
        AbstractManifest manifest;
        if (Path.GetFileName(file) == "packages.config")
        {
            manifest = new PackagesManifest();
            manifest.Parse(File.ReadAllText(file));
        }
        else
        {
            manifest = new NuGetManifest();
            manifest.Parse(File.ReadAllText(file));
        }

        return manifest;
    }

    public static bool IsManifestFile(string file)
    {
        return file.EndsWith(".csproj") || Path.GetFileName(file) == "packages.config";
    }
}
