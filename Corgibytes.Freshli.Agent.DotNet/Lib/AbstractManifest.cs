using System.Collections;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public abstract class AbstractManifest : IEnumerable<PackageInfo>
{
    private readonly ILogger<AbstractManifest> _logger = Logging.Logger<AbstractManifest>();

    private readonly IDictionary<string, PackageInfo> _packages =
        new Dictionary<string, PackageInfo>();

    public int Count => _packages.Count;

    protected XmlDocument? InnerDocument { get; private set; }

    public IEnumerator<PackageInfo> GetEnumerator()
    {
        return _packages.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected void Add(string packageName, string packageVersion)
    {
        _packages[packageName] = new PackageInfo(
            packageName,
            packageVersion
        );
        _logger.LogTrace(
            "AddPackage: PackageInfo({PackageName}, {PackageVersion})",
            packageName, packageVersion
        );
    }

    public void Parse(string contents)
    {
        InnerDocument = new XmlDocument();
        InnerDocument.LoadXml(contents);
        ParseInnerDocument();
    }

    protected abstract void ParseInnerDocument();

    // ReSharper disable once UnusedMemberInSuper.Global
    public abstract void Update(string packageName, string packageVersion);

    public PackageInfo this[string packageName] => _packages[packageName];

    public void Save(string targetPath)
    {
        InnerDocument?.Save(targetPath);
    }
}
