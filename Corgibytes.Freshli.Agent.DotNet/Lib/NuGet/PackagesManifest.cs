using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class PackagesManifest : AbstractManifest
{
    public const string Element = "package";
    public const string NameAttribute = "id";
    public const string VersionAttribute = "version";

    public override void Parse(string contents)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(contents);
        Parse(xmlDoc);
    }

    public override void Parse(XmlDocument xmlDoc)
    {
        XmlNodeList packages = xmlDoc.GetElementsByTagName(Element);
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

    public override void Update(XmlDocument xmlDoc, string packageName, string packageVersion)
    {
        XmlNode? node = xmlDoc.SelectSingleNode($"*/{Element}[@{NameAttribute} = '{packageName}']");
        node.Attributes[VersionAttribute].Value = packageVersion;
    }

    public override bool UsesExactMatches => true;
}
