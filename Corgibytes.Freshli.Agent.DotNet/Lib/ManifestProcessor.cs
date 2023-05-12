using System.Diagnostics;
using System.Text.RegularExpressions;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestProcessor
{
    public string ProcessManifest(string manifestFilePath, DateTimeOffset? asOfDate)
    {
        string outDir = new DirectoryInfo(manifestFilePath).Parent.FullName + "/obj";
        ProcessStartInfo startInfo = new()
        {
            FileName = "dotnet",
            Arguments = $"dotnet-CycloneDX {manifestFilePath} -j -o {outDir}",
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var proc = Process.Start(startInfo);

        ArgumentNullException.ThrowIfNull(proc);

        string output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        return ExtractFile(output);
    }

    public static string ExtractFile(string content)
    {
        Match match = Regex.Match(content, @".*Writing to:(.*)$", RegexOptions.IgnoreCase);
        if (match is { Success: true, Groups.Count: > 0 })
        {
            return match.Groups[1].ToString().Trim();
        }
        throw new ManifestProcessingException(
            $"Failed to generate bill of materials. See command output for more information:{Environment.NewLine}{content}");
    }
}
