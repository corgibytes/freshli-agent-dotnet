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

    public override Task<BomLocation> ProcessManifest(ProcessingRequest request, ServerCallContext context)
    {
        var asOfDate = request.Moment?.ToDateTimeOffset() ?? DateTimeOffset.Now;
        _logger.LogInformation("ProcessManifest() - manifestPath: {ManifestPath}, asOfDate: {AsOfDate}",
            request.Manifest.Path, asOfDate.ToString());
        try
        {
            var bomLocation =
                _manifestProcessor.ProcessManifest(request.Manifest.Path, asOfDate);
            return Task.FromResult(new BomLocation() { Path = bomLocation });
        }
        catch (ManifestProcessingException error)
        {
            var errorMessage = $"Error processing {request.Manifest.Path}@{asOfDate}: {error.Message}";
            _logger.LogError("{ErrorMessage}", errorMessage);
            _logger.LogDebug("Exception.details: {Detail}", error.Details);
            throw new RpcException(
                new Status(StatusCode.Internal, "Processing Error"),
                errorMessage);
        }
    }

    public override Task DetectManifests(ProjectLocation request, IServerStreamWriter<ManifestLocation> responseStream,
        ServerCallContext context)
    {
        var projectLocation = request.Path;
        _logger.LogInformation("DetectManifests() - {ProjectLocation}", projectLocation);
        foreach (var filename in new ManifestDetector().FindManifests(projectLocation))
        {
            responseStream.WriteAsync(
                new ManifestLocation() { Path = Path.Combine(projectLocation, filename) },
                context.CancellationToken
            );
        }

        return Task.CompletedTask;
    }

    public override Task RetrieveReleaseHistory(Package request, IServerStreamWriter<PackageRelease> responseStream,
        ServerCallContext context)
    {
        var packageUrl = request.Purl;
        _logger.LogInformation("RetrieveReleaseHistory() - {RequestPurl}", packageUrl);
        var packageReleases = new ReleaseHistoryRetriever().Retrieve(packageUrl);
        packageReleases
            .ForEach(release =>
            {
                responseStream.WriteAsync(new PackageRelease()
                {
                    ReleasedAt = release.ReleasedAt.ToTimestamp(),
                    Version = release.Version
                }, context.CancellationToken);
            });
        return Task.CompletedTask;
    }

    public override Task<Empty> Shutdown(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("Shutdown()");
        _server.Stop();
        return Task.FromResult(new Empty());
    }

    public override Task GetValidatingPackages(Empty request, IServerStreamWriter<Package> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("GetValidatingPackages()");
        var packageUrls = ValidatingData.PackageUrls();
        foreach (var packageUrl in packageUrls)
        {
            responseStream.WriteAsync(new Package() { Purl = packageUrl }, context.CancellationToken);
        }

        return Task.CompletedTask;
    }

    public override Task GetValidatingRepositories(Empty request,
        IServerStreamWriter<RepositoryLocation> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("GetValidatingRepositories()");
        var repositoryUrls = ValidatingData.RepositoryUrls();
        foreach (var repositoryUrl in repositoryUrls)
        {
            responseStream.WriteAsync(new RepositoryLocation() { Url = repositoryUrl }, context.CancellationToken);
        }

        return Task.CompletedTask;
    }
}
