using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using CliWrap;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public enum CycloneDxExitCode
{
    Ok,
    OkFail,
    IOError
}

public partial class ManifestProcessor
{
    private readonly ILogger<ManifestProcessor> _logger = Logging.Logger<ManifestProcessor>();

    public string ProcessManifest(string manifestFilePath, DateTimeOffset? asOfDate)
    {
        _logger.LogDebug("Processing manifest at {ManifestFilePath} as of {AsOfDate}", manifestFilePath, asOfDate);
        if (asOfDate != null)
        {
            Versions.UpdateManifest(manifestFilePath, asOfDate.Value);
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
            string formattedCreationTime = existingOutFile.CreationTime.ToString("yyyyMMdd-HHmmss");
            string destFileName = $"bom-{formattedCreationTime}.json";
            _logger.LogDebug("Output file {OutFilename} exists and will be moved to {NewOutFilename}", outFile,
                destFileName);
            File.Move(existingOutFile.FullName,
                Path.Combine(outDir, destFileName));
        }

        var commandResult = Cli.Wrap("dotnet-CycloneDX")
            .WithArguments(new List<string>
            {
                manifestFilePath,
                "--disable-github-licenses",
                "--json",
                "--out",
                outDir
            })
            .WithWorkingDirectory(manifestDir.FullName)
            .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
            .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
            .WithValidation(CommandResultValidation.None)
            // TODO: Pass in a cancellation token that will get triggered if Shutdown is called
            .ExecuteAsync()
            .Task
            .Result;

        if (asOfDate != null)
        {
            Versions.RestoreManifest(manifestFilePath);
        }

        if (commandResult.ExitCode != (int)CycloneDxExitCode.Ok)
        {
            _logger.LogError("Error running CycloneDX");

            if (commandResult.ExitCode is (int)CycloneDxExitCode.IOError or (int)CycloneDxExitCode.OkFail)
            {
                throw new ManifestProcessingException(
                    $"CycloneDX execution failed with exitCode = {commandResult.ExitCode}"
                );
            }

            return "";
        }
        // The CycloneDX cli tool will exit with different error codes
        // to
        // indicate the problem. The agent needs to absorb most of them,
        // notify the caller of problems, but not cause errors that
        // can not be dealt with properly
        if (File.Exists(outFile))
        {
            return outFile;
        }

        throw new ManifestProcessingException("Failed to generate bill of materials.");
    }

}
