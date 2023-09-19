using System.Text.Json;
using System.Text.Json.Nodes;
using CliWrap;
using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestProcessor
{
    private readonly ILogger<ManifestProcessor> _logger = Logging.Logger<ManifestProcessor>();

    public async Task<string> ProcessManifest(string manifestFilePath, DateTimeOffset? asOfDate)
    {
        if (!File.Exists(manifestFilePath))
        {
            throw new FileNotFoundException("Manifest file was not found", manifestFilePath);
        }

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

        var outDir = manifestDir.FullName + "/obj";
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

        var outFile = Path.Combine(outDir, "bom.json");
        if (File.Exists(outFile))
        {
            var existingOutFile = new FileInfo(outFile);
            var formattedCreationTime = existingOutFile.CreationTime.ToString("yyyyMMdd-HHmmss");
            var destFileName = $"bom-{formattedCreationTime}.json";
            _logger.LogDebug("Output file {OutFilename} exists and will be moved to {NewOutFilename}", outFile,
                destFileName);
            File.Move(existingOutFile.FullName,
                Path.Combine(outDir, destFileName));
        }

        var commandResult = await Cli.Wrap("dotnet-CycloneDX")
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
            .ExecuteAsync();

        if (asOfDate != null)
        {
            Versions.RestoreManifest(manifestFilePath);
        }

        if (commandResult.ExitCode != 0)
        {
            _logger.LogError("Error running CycloneDX");

            throw new ManifestProcessingException(
                $"CycloneDX execution failed with exitCode = {commandResult.ExitCode}"
            );
        }

        if (!File.Exists(outFile))
        {
            throw new ManifestProcessingException("Failed to generate bill of materials.");
        }

        FixBomLicenseNodes(outFile);

        return outFile;
    }

    private void FixBomLicenseNodes(string bomPath)
    {
        using var bomStream = new FileStream(bomPath, FileMode.Open, FileAccess.ReadWrite);
        var bomRoot = JsonNode.Parse(bomStream)!;

        FixBomLicenseNodes(bomRoot);
        var bomContents = bomRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        bomStream.Seek(0, SeekOrigin.Begin);
        using var bomWriter = new StreamWriter(bomStream);
        bomWriter.Write(bomContents);
        bomWriter.Close();
    }

    private void FixBomLicenseNodes(JsonNode node)
    {
        switch (node)
        {
            case JsonObject objectNode:
                HandleJsonObject(objectNode);
                break;

            case JsonArray arrayNode:
                HandleJsonArray(arrayNode);
                break;
        }
    }

    private void HandleJsonArray(JsonArray arrayNode)
    {
        foreach (var childNode in arrayNode)
        {
            if (childNode != null)
            {
                FixBomLicenseNodes(childNode);
            }
        }
    }

    private void HandleJsonObject(JsonObject objectNode)
    {
        if (HandleLicenseArray(objectNode))
        {
            return;
        }

        foreach (var childEntry in objectNode)
        {
            if (childEntry.Value != null)
            {
                FixBomLicenseNodes(childEntry.Value);
            }
        }
    }

    private static bool HandleLicenseArray(JsonObject objectNode)
    {
        if (!objectNode.ContainsKey("licenses") || objectNode["licenses"] is not JsonArray licensesArray)
        {
            return false;
        }

        foreach (var licenseChoiceNode in licensesArray)
        {
            if (licenseChoiceNode is not JsonObject objectLicenseChoiceNode ||
                !((IDictionary<string, JsonNode?>)objectLicenseChoiceNode).TryGetValue("license", out var licenseNode))
            {
                continue;
            }

            if (licenseNode is JsonObject objectLicenseNode &&
                objectLicenseNode.ContainsKey("url") &&
                !objectLicenseNode.ContainsKey("name"))
            {
                objectLicenseNode["name"] = "Unknown - See URL";
            }
        }

        return true;
    }
}
