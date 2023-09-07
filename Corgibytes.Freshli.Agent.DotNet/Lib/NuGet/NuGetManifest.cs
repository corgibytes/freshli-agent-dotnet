using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetManifest : AbstractManifest
{
    public const string Element = "PackageReference";
    public const string NameAttribute = "Include";
    public const string VersionAttribute = "Version";

    protected override void ParseInnerDocument()
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

    public override void Update(string packageName, string packageVersion)
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
}
