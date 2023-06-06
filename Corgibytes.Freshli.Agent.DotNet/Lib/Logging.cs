using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class Logging
{
    public static ILogger<T> Logger<T>()
    {
        return LoggerFactory.Create(builder =>
                builder.AddConsole())
            .CreateLogger<T>();
    }
}
