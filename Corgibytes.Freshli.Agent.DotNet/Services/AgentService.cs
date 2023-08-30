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
        DateTimeOffset asOfDate = request.Moment?.ToDateTimeOffset() ?? DateTimeOffset.Now;
        _logger.LogInformation("ProcessManifest() - manifestPath: {ManifestPath}, asOfDate: {AsOfDate}",
            request.Manifest.Path, asOfDate.ToString());
        try
        {
            string bomLocation =
                _manifestProcessor.ProcessManifest(request.Manifest.Path, asOfDate);
            return Task.FromResult(new BomLocation() { Path = bomLocation });
        }
        catch (ManifestProcessingException error)
        {
            string errorMessage = $"Error processing {request.Manifest.Path}@{asOfDate}: {error.Message}";
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
        string projectLocation = request.Path;
        _logger.LogInformation("DetectManifests() - {ProjectLocation}", projectLocation);
        foreach (string filename in new ManifestDetector().FindManifests(projectLocation))
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
        string packageUrl = request.Purl;
        _logger.LogInformation("RetrieveReleaseHistory() - {RequestPurl}", packageUrl);
        List<PackageReleaseData> packageReleases = new ReleaseHistoryRetriever().Retrieve(packageUrl);
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
        List<string> packageUrls = ValidatingData.PackageUrls();
        foreach (string packageUrl in packageUrls)
        {
            responseStream.WriteAsync(new Package() { Purl = packageUrl }, context.CancellationToken);
        }

        return Task.CompletedTask;
    }

    public override Task GetValidatingRepositories(Empty request,
        IServerStreamWriter<RepositoryLocation> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("GetValidatingRepositories()");
        List<string> repositoryUrls = ValidatingData.RepositoryUrls();
        foreach (string repositoryUrl in repositoryUrls)
        {
            responseStream.WriteAsync(new RepositoryLocation() { Url = repositoryUrl }, context.CancellationToken);
        }

        return Task.CompletedTask;
    }
}
