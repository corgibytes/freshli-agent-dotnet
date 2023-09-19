using Corgibytes.Freshli.Agent.DotNet.Exceptions;
using Corgibytes.Freshli.Agent.DotNet.Lib;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Services;

public class AgentService : Agent.AgentBase
{
    private readonly AgentServer _server;
    private readonly ILogger<AgentService> _logger;
    private readonly ManifestProcessor _manifestProcessor;

    public AgentService(AgentServer server, ILogger<AgentService> logger)
    {
        _server = server;
        _logger = logger;
        _manifestProcessor = new ManifestProcessor();
    }

    public override async Task<BomLocation> ProcessManifest(ProcessingRequest request, ServerCallContext context)
    {
        var asOfDate = request.Moment?.ToDateTimeOffset() ?? DateTimeOffset.Now;
        _logger.LogInformation("ProcessManifest() - manifestPath: {ManifestPath}, asOfDate: {AsOfDate}",
            request.Manifest.Path, asOfDate.ToString());
        try
        {
            var bomLocation =
                await _manifestProcessor.ProcessManifest(request.Manifest.Path, asOfDate);
            return new BomLocation() { Path = bomLocation };
        }
        catch (ManifestProcessingException error)
        {
            var errorMessage = $"Error processing {request.Manifest.Path}@{asOfDate}: {error.Message}";
            _logger.LogWarning("{ErrorMessage}", errorMessage);
            _logger.LogDebug("Exception.details: {Detail}", error.Details);

            return new BomLocation() { Path = "" };
        }
    }

    public override async Task DetectManifests(ProjectLocation request, IServerStreamWriter<ManifestLocation> responseStream,
        ServerCallContext context)
    {
        var projectLocation = request.Path;
        _logger.LogInformation("DetectManifests() - {ProjectLocation}", projectLocation);
        foreach (var filename in new ManifestDetector().FindManifests(projectLocation))
        {
            await responseStream.WriteAsync(
                new ManifestLocation() { Path = Path.Combine(projectLocation, filename) },
                context.CancellationToken
            );
        }
    }

    public override async Task RetrieveReleaseHistory(Package request, IServerStreamWriter<PackageRelease> responseStream,
        ServerCallContext context)
    {
        var packageUrl = request.Purl;
        _logger.LogInformation("RetrieveReleaseHistory() - {RequestPurl}", packageUrl);
        var releaseRetriever = new ReleaseHistoryRetriever();
        var packageReleases = await releaseRetriever.Retrieve(packageUrl);
        foreach (var release in packageReleases)
        {
            await responseStream.WriteAsync(new PackageRelease()
            {
                ReleasedAt = release.ReleasedAt.ToTimestamp(),
                Version = release.Version
            }, context.CancellationToken);
        }
    }

    public override Task<Empty> Shutdown(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("Shutdown()");
        _server.Stop();
        return Task.FromResult(new Empty());
    }

    public override async Task GetValidatingPackages(Empty request, IServerStreamWriter<Package> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("GetValidatingPackages()");
        var packageUrls = ValidatingData.PackageUrls();
        foreach (var packageUrl in packageUrls)
        {
            await responseStream.WriteAsync(new Package() { Purl = packageUrl }, context.CancellationToken);
        }
    }

    public override async Task GetValidatingRepositories(Empty request,
        IServerStreamWriter<RepositoryLocation> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("GetValidatingRepositories()");
        var repositoryUrls = ValidatingData.RepositoryUrls();
        foreach (var repositoryUrl in repositoryUrls)
        {
            await responseStream.WriteAsync(new RepositoryLocation() { Url = repositoryUrl }, context.CancellationToken);
        }
    }
}
