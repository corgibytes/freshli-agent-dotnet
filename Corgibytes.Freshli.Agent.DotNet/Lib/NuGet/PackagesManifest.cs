using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class PackagesManifest : AbstractManifest
{
    public const string Element = "package";
    public const string NameAttribute = "id";
    public const string VersionAttribute = "version";

    protected override void ParseInnerDocument()
    {
        if (InnerDocument == null)
        {
            return;
        }

        var packages = InnerDocument.GetElementsByTagName(Element);
        foreach (XmlNode package in packages)
        {
            if (package?.Attributes?.Count >= 2)
            {
                Add(
                    package.Attributes[NameAttribute]!.Value,
                    package.Attributes[VersionAttribute]!.Value
                );
            }
        }
    }

    public override void Update(string packageName, string packageVersion)
    {
        var node = InnerDocument?.SelectSingleNode($"*/{Element}[@{NameAttribute} = '{packageName}']");
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
}
