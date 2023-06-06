using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class PackagesManifest : AbstractManifest
{
    public override void Parse(string contents)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(contents);

        var packages = xmlDoc.GetElementsByTagName("package");
        foreach (XmlNode package in packages)
        {
            Add(
                package.Attributes![0].Value,
                package.Attributes[1].Value
            );
        }
    }

    public override bool UsesExactMatches => true;
}
