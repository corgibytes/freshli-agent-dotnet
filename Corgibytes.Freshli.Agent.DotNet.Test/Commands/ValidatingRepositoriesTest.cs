using Corgibytes.Freshli.Agent.DotNet.Commands;
using Xunit;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Commands;

public class ValidatingRepositoriesTest
{
    [Fact]
    public void Run()
    {
        // Ensure command executes without error
        new ValidatingRepositories().Run();
    }
}
