using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetManifest : AbstractManifest
{
    public const string Element = "PackageReference";
    public const string NameAttribute = "Include";
    public const string VersionAttribute = "Version";

    public override void Parse(string contents)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(contents);
        Parse(xmlDoc);
    }

    public override void Parse(XmlDocument xmlDoc)
    {
        var packages = xmlDoc.GetElementsByTagName(Element);
        foreach (XmlNode package in packages)
        {
            Add(
                package.Attributes![NameAttribute]!.Value,
                package.Attributes![VersionAttribute]!.Value
            );
        }
    }

    public override void Update(XmlDocument xmlDoc, string packageName, string packageVersion)
    {
        var node = xmlDoc.SelectSingleNode($"/Project/ItemGroup/{Element}[@{NameAttribute} = '{packageName}']");
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

    public override bool UsesExactMatches => true;
}
