using Corgibytes.Freshli.Agent.DotNet.Lib;
using Corgibytes.Freshli.Lib;
using Xunit;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ManifestDetectorTest
{
    [Fact]
    public void ManifestFinders()
    {
        var manifestDetector = new ManifestDetector();
        string analysisPath = new DirectoryInfo("./../../../../").FullName;

        IEnumerable<AbstractManifestFinder> manifestFinders = manifestDetector.ManifestFinders(analysisPath).ToList();
        Assert.Single(manifestFinders);
        AbstractManifestFinder finder = manifestFinders.First();
        string[] manifestFilenames = finder.GetManifestFilenames(analysisPath);
        Assert.Equal(2, manifestFilenames.Length);
        Assert.Equal("Corgibytes.Freshli.Agent.DotNet/Corgibytes.Freshli.Agent.DotNet.csproj",
            manifestFilenames[0]);
        Assert.Equal("Corgibytes.Freshli.Agent.DotNet.Test/Corgibytes.Freshli.Agent.DotNet.Test.csproj",
            manifestFilenames[1]);
    }
}
