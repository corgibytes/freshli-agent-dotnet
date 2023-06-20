using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestDetectorTest
{
    private readonly ITestOutputHelper _output;

    private readonly ManifestDetector _manifestDetector = new();

    public ManifestDetectorTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ManifestFinders()
    {
        string analysisPath = Fixtures.Path();
        IEnumerable<AbstractManifestFinder> manifestFinders = _manifestDetector.ManifestFinders(analysisPath).ToList();
        foreach (AbstractManifestFinder abstractManifestFinder in manifestFinders)
        {
            _output.WriteLine($"ManifestFinder: {abstractManifestFinder.GetType()}");
        }

        Assert.Equal(2, manifestFinders.Count());
    }

    [Fact]
    public void FindManifests()
    {
        string analysisPath = Fixtures.Path();
        IEnumerable<string> manifests = _manifestDetector.FindManifests(analysisPath);
        foreach (string manifestFile in manifests)
        {
            _output.WriteLine($"ManifestFinder: {manifestFile}");
        }

        Assert.Equal(3, manifests.Count());
    }
}
