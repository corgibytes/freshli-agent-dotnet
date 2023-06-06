namespace Corgibytes.Freshli.Agent.DotNet.Exceptions
{
    public class FileHistoryFinderNotFoundException : Exception
    {

        public FileHistoryFinderNotFoundException(string path)
          : base($"Unable to find an IFileHistoryFinder instance for {path}.")
        {
        }

    }
}
