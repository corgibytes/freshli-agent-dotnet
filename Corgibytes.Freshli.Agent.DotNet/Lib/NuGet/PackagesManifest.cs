
using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class PackagesManifest : AbstractManifest
{
    public override void Parse(string contents)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(contents);
        Parse(xmlDoc);
    }

    public override void Parse(XmlDocument xmlDoc)
    {
        XmlNodeList packages = xmlDoc.GetElementsByTagName("package");
        foreach (XmlNode package in packages)
        {
            Add(
                package.Attributes![0].Value,
                package.Attributes[1].Value
            );
        }
    }

    public override void Update(XmlDocument xmlDoc, string packageName, string packageVersion)
    {
        XmlNode? node = xmlDoc.SelectSingleNode($"*/package[@id = '{packageName}']");
        node.Attributes["version"].Value = packageVersion;
    }

    public override bool UsesExactMatches => true;
}
