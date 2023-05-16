using System.Diagnostics;
using System.Text.RegularExpressions;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public partial class ManifestProcessor
{
    public string ProcessManifest(string manifestFilePath, DateTimeOffset? asOfDate)
    {
        if (asOfDate != null)
        {
            // do nothing at the moment.
        }
        var manifestDir = new DirectoryInfo(manifestFilePath);
        if (manifestDir.Parent != null)
        {
            manifestDir = manifestDir.Parent;
        }
        string outDir = manifestDir.FullName + "/obj";
        ProcessStartInfo startInfo = new()
        {
            FileName = "/usr/local/bin/cyclonedx",
            Arguments = $"{manifestFilePath} -j -o {outDir}",
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = manifestDir.FullName
        };
        var proc = Process.Start(startInfo);

        ArgumentNullException.ThrowIfNull(proc);

        string output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        return ExtractFile(output);
    }

    [GeneratedRegex(".*Writing to:(.*)$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex FilenameFinderRegex();

    public static string ExtractFile(string content)
    {
        Match match = FilenameFinderRegex().Match(content);
        if (match is { Success: true, Groups.Count: > 0 })
        {
            return match.Groups[1].ToString().Trim();
        }

        throw new ManifestProcessingException(
            $"Failed to generate bill of materials. See command output for more information:{Environment.NewLine}{content}");
    }
}
