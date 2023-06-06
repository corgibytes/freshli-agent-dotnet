using System.Diagnostics;
using System.Text.RegularExpressions;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public partial class ManifestProcessor
{
    private readonly ILogger<ManifestProcessor> _logger = Logging.Logger<ManifestProcessor>();

    public string ProcessManifest(string manifestFilePath, DateTimeOffset? asOfDate)
    {
        _logger.LogDebug("Processing manifest at {ManifestFilePath} as of {AsOfDate}", manifestFilePath, asOfDate);
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
        if (!Directory.Exists(outDir))
        {
            _logger.LogDebug("Output directory {Dir} needs to be created", outDir);
            try
            {
                Directory.CreateDirectory(outDir);
            }
            catch (SystemException error)
            {
                throw new ManifestProcessingException($"Failed creating {outDir}: " + error.Message);
            }
        }

        string outFile = Path.Combine(outDir, "bom.json");
        if (File.Exists(outFile))
        {
            var existingOutFile = new FileInfo(outFile);
            string destFileName = $"bom-{existingOutFile.CreationTime.ToString("yyyyMMdd-HHmmss")}.json";
            _logger.LogDebug("Output file {OutFilename} exists and will be moved to {NewOutFilename}", outFile,
                destFileName);
            File.Move(existingOutFile.FullName,
                Path.Combine(outDir, destFileName));
        }

        // use -dgl for now to avoid hitting Github rate limit
        ProcessStartInfo startInfo = new()
        {
            FileName = "/usr/local/bin/cyclonedx",
            Arguments = $"{manifestFilePath} -dgl -j -o {outDir}",
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = manifestDir.FullName
        };
        var proc = Process.Start(startInfo);

        ArgumentNullException.ThrowIfNull(proc);

        string output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            throw new ManifestProcessingException(
                $"CycloneDX execution failed with exitCode = {proc.ExitCode}");
        }

        if (File.Exists(outFile))
        {
            return outFile;
        }
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
            "Failed to generate bill of materials.", content);
    }
}
