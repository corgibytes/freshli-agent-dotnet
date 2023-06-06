namespace Corgibytes.Freshli.Agent.DotNet.Lib
{
    public interface IFileHistory
    {
        IList<DateTimeOffset> Dates { get; }
        string ContentsAsOf(DateTimeOffset date);
        string ShaAsOf(DateTimeOffset date);
    }
}
