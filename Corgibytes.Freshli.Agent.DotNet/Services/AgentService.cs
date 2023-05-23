using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Lib;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Services;

public class AgentService : Agent.AgentBase
{
    private readonly ILogger<AgentService> _logger;
    private readonly ManifestProcessor _manifestProcessor;

    public AgentService(ILogger<AgentService> logger)
    {
        _logger = logger;
        _manifestProcessor = new ManifestProcessor();
    }

    public override Task<BomLocation> ProcessManifest(ProcessingRequest request, ServerCallContext context)
    {
        DateTimeOffset asOfDate = request.Moment?.ToDateTimeOffset() ?? DateTimeOffset.Now;
        _logger.LogInformation("ProcessManifest() - manifestPath: {ManifestPath}, asOfDate: {AsOfDate}",
            request.Manifest.Path, asOfDate.ToString());
        string bomLocation =
            _manifestProcessor.ProcessManifest(request.Manifest.Path, asOfDate);
        return Task.FromResult(new BomLocation() { Path = bomLocation });
    }

    public override Task DetectManifests(ProjectLocation request, IServerStreamWriter<ManifestLocation> responseStream,
        ServerCallContext context)
    {
        string projectLocation = request.Path;
        _logger.LogInformation("DetectManifests() - {ProjectLocation}", projectLocation);
        IEnumerable<AbstractManifestFinder> manifestFinders = new ManifestDetector().ManifestFinders(projectLocation);
        foreach (AbstractManifestFinder manifestFinder in manifestFinders)
        {
            string[] filenames = manifestFinder.GetManifestFilenames(projectLocation);
            foreach (string filename in filenames)
            {
                responseStream.WriteAsync(new ManifestLocation() { Path = Path.Combine(projectLocation, filename) });
            }
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
                });
            });
        return Task.CompletedTask;
    }

    public override Task<Empty> Shutdown(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("Shutdown()");
        AgentServer.Instance()!.Stop();
        return Task.FromResult(new Empty());
    }

    public override Task GetValidatingPackages(Empty request, IServerStreamWriter<Package> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("GetValidatingPackages()");
        List<string> packageUrls = ValidatingData.PackageUrls();
        foreach (string packageUrl in packageUrls)
        {
            responseStream.WriteAsync(new Package() { Purl = packageUrl });
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
            responseStream.WriteAsync(new RepositoryLocation() { Url = repositoryUrl });
        }

        return Task.CompletedTask;
    }
}
