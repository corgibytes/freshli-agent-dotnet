using Corgibytes.Freshli.Agent.DotNet.Lib;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Agent.DotNet.Test.Lib;

public class ReleaseHistoryRetrieverTest
{
    private readonly ITestOutputHelper _output;

    public ReleaseHistoryRetrieverTest(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Retrieve()
    {
        var packageReleases = new ReleaseHistoryRetriever()
            .Retrieve("pkg:nuget/Corgibytes.Freshli.Lib@0.5.0");

        var releaseDatesByVersion = new Dictionary<string, DateTimeOffset>
        {
            { "0.4.0-alpha0116", DateTimeOffset.Parse("2021-05-17T12:54:26.4800000+00:00") },
            { "0.4.0-alpha0117", DateTimeOffset.Parse("2021-05-17T12:55:16.0200000+00:00") },
            { "0.4.0-alpha0121", DateTimeOffset.Parse("2021-05-17T13:06:07.4930000+00:00") },
            { "0.4.0-alpha0124", DateTimeOffset.Parse("2021-05-17T13:12:20.7570000+00:00") },
            { "0.4.0-alpha0127", DateTimeOffset.Parse("2021-05-17T13:22:29.6970000+00:00") },
            { "0.4.0-alpha0131", DateTimeOffset.Parse("2021-05-17T13:28:47.4230000+00:00") },
            { "0.4.0-alpha0140", DateTimeOffset.Parse("2021-05-17T16:14:22.8900000+00:00") },
            { "0.4.0-alpha0141", DateTimeOffset.Parse("2021-05-17T16:14:33.1170000+00:00") },
            { "0.4.0-alpha0143", DateTimeOffset.Parse("2021-05-19T11:56:15.1200000+00:00") },
            { "0.4.0-alpha0146", DateTimeOffset.Parse("2021-05-19T12:05:10.2100000+00:00") },
            { "0.4.0-alpha0149", DateTimeOffset.Parse("2021-05-22T17:45:13.4230000+00:00") },
            { "0.4.0-alpha0169", DateTimeOffset.Parse("2021-05-24T21:53:47.4130000+00:00") },
            { "0.4.0-alpha0170", DateTimeOffset.Parse("2021-05-24T21:56:22.7870000+00:00") },
            { "0.4.0-alpha0172", DateTimeOffset.Parse("2021-05-28T14:35:23.5570000+00:00") },
            { "0.4.0-alpha0173", DateTimeOffset.Parse("2021-05-28T14:36:05.1830000+00:00") },
            { "0.4.0-alpha0177", DateTimeOffset.Parse("2021-05-31T21:44:12.9400000+00:00") },
            { "0.4.0-alpha0178", DateTimeOffset.Parse("2021-05-31T21:45:30.1100000+00:00") },
            { "0.4.0-alpha0183", DateTimeOffset.Parse("2021-05-31T22:31:53.6630000+00:00") },
            { "0.4.0-alpha0184", DateTimeOffset.Parse("2021-05-31T22:32:27.5870000+00:00") },
            { "0.4.0-alpha0186", DateTimeOffset.Parse("2021-06-07T18:55:34.7400000+00:00") },
            { "0.4.0-alpha0187", DateTimeOffset.Parse("2021-06-07T18:56:30.4230000+00:00") },
            { "0.4.0-alpha0191", DateTimeOffset.Parse("2021-06-07T22:28:08.5500000+00:00") },
            { "0.4.0-alpha0192", DateTimeOffset.Parse("2021-06-07T22:28:55.7400000+00:00") },
            { "0.4.0-alpha0196", DateTimeOffset.Parse("2021-06-07T22:42:36.0700000+00:00") },
            { "0.4.0-alpha0198", DateTimeOffset.Parse("2021-06-09T22:03:16.8430000+00:00") },
            { "0.4.0-alpha0199", DateTimeOffset.Parse("2021-06-09T22:05:00.7830000+00:00") },
            { "0.4.0-alpha0201", DateTimeOffset.Parse("2021-06-09T22:53:07.2100000+00:00") },
            { "0.4.0-alpha0202", DateTimeOffset.Parse("2021-06-09T22:55:04.8300000+00:00") },
            { "0.4.0-alpha0208", DateTimeOffset.Parse("2021-06-17T17:23:12.7400000+00:00") },
            { "0.4.0-alpha0212", DateTimeOffset.Parse("2021-08-12T22:05:59.1470000+00:00") },
            { "0.4.0-alpha0214", DateTimeOffset.Parse("2021-08-12T22:22:22.6370000+00:00") },
            { "0.4.0-alpha0215", DateTimeOffset.Parse("2021-08-12T22:24:04.0700000+00:00") },
            { "0.4.0-alpha0217", DateTimeOffset.Parse("2021-08-12T22:49:27.4770000+00:00") },
            { "0.4.0-alpha0218", DateTimeOffset.Parse("2021-08-12T22:50:04.9000000+00:00") },
            { "0.4.0-alpha0220", DateTimeOffset.Parse("2021-08-12T23:17:22.9500000+00:00") },
            { "0.4.0-alpha0221", DateTimeOffset.Parse("2021-08-12T23:18:05.8170000+00:00") },
            { "0.4.0", DateTimeOffset.Parse("2021-09-27T22:23:17.2430000+00:00") },
            { "0.5.0", DateTimeOffset.Parse("2022-09-08T19:56:14.3770000+00:00") },
        };

        foreach (var (version, expectedDate) in releaseDatesByVersion)
        {
            Assert.Equal(
                1,
                packageReleases.Count(value =>
                    value.Version == version &&
                    value.ReleasedAt == expectedDate
                )
            );
        }
    }
}
