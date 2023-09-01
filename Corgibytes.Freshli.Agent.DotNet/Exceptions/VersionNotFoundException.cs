namespace Corgibytes.Freshli.Agent.DotNet.Exceptions;

public class VersionNotFoundException : Exception
{
    public VersionNotFoundException(string dependency, string version, Exception e) :
        base($"Unable to find {version} version of {dependency}.", e)
    {
    }
}
