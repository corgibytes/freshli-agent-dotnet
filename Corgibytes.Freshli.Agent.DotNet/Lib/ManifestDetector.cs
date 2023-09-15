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

        var hashSet = new HashSet<string> { analysisPath };

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

    public static bool IsManifestFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return fileName.EndsWith(".csproj") || fileName == "packages.config";
    }
}
