using System.Collections;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetManifest : IEnumerable<PackageInfo>
{
    private readonly ILogger<NuGetManifest> _logger = Logging.Logger<NuGetManifest>();

    private readonly IDictionary<string, PackageInfo> _packages =
        new Dictionary<string, PackageInfo>();

    public const string Element = "PackageReference";
    public const string NameAttribute = "Include";
    public const string VersionAttribute = "Version";
    public int Count => _packages.Count;
    private XmlDocument? InnerDocument { get; set; }

    private void ParseInnerDocument()
    {
        if (InnerDocument is null)
        {
            return;
        }

        var packages = InnerDocument.GetElementsByTagName(Element);
        foreach (XmlNode package in packages)
        {
            Add(
                package.Attributes![NameAttribute]!.Value,
                package.Attributes![VersionAttribute]!.Value
            );
        }
    }

    public void Update(string packageName, string packageVersion)
    {
        var node = InnerDocument?.SelectSingleNode($"/Project/ItemGroup/{Element}[@{NameAttribute} = '{packageName}']");
        if (node is not { Attributes: not null })
        {
            return;
        }

        var versionAttribute = node.Attributes[VersionAttribute];
        if (versionAttribute != null)
        {
            versionAttribute.Value = packageVersion;
        }
    }

    public IEnumerator<PackageInfo> GetEnumerator()
    {
        return _packages.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Add(string packageName, string packageVersion)
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

    public PackageInfo this[string packageName] => _packages[packageName];

    public void Save(string targetPath)
    {
        InnerDocument?.Save(targetPath);
    }
}
