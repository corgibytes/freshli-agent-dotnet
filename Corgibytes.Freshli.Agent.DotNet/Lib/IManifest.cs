using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public interface IManifest : IEnumerable<PackageInfo>
{
    int Count { get; }
    void Add(string packageName, string packageVersion);

    void Update(XmlDocument xmlDoc, string packageName, string packageVersion);
    void Parse(string contents);
    void Parse(XmlDocument xmlDoc);
    PackageInfo this[string packageName] { get; }
    bool UsesExactMatches { get; }
}
