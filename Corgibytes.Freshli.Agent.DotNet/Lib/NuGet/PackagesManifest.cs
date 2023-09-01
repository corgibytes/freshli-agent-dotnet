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
        var packages = xmlDoc.GetElementsByTagName(Element);
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
        var node = xmlDoc.SelectSingleNode($"*/{Element}[@{NameAttribute} = '{packageName}']");
        if (node != null)
        {
            var attributes = node.Attributes;
            if (attributes != null)
            {
                var versionAttribute = attributes[VersionAttribute];
                if (versionAttribute != null)
                {
                    versionAttribute.Value = packageVersion;
                }
            }
        }
    }

    public override bool UsesExactMatches => true;
}
