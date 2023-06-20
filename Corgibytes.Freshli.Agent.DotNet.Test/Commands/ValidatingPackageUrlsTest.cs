using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class ValidatingPackageUrlsTest
{
    [Fact]
    public void Run()
    {
        // Ensure command executes without error
        new ValidatingPackageUrls().Run();
    }
}
