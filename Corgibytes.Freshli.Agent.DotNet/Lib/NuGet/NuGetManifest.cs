using System.Collections;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetManifest : IEnumerable<PackageInfo>
{
    private readonly string _path;
    private readonly string _backupPath;
    private readonly XmlDocument _manifestXml;
    private readonly NuGetDirectoryPackagesProperties? _directoryPackagesProperties;
    public const string BackupSuffix = ".versionsBackup";

    public NuGetManifest(string path)
    {
        _path = path;
        _backupPath = _path + BackupSuffix;
        _manifestXml = new XmlDocument();
        _manifestXml.Load(path);

        var directoryPath = Path.GetDirectoryName(path);
        if (directoryPath != null)
        {
            _directoryPackagesProperties = NuGetDirectoryPackagesProperties.Build(directoryPath);
        }

        ParseManifestXml();
    }

    private readonly ILogger<NuGetManifest> _logger = Logging.Logger<NuGetManifest>();

    private readonly IDictionary<string, PackageInfo> _packages =
        new Dictionary<string, PackageInfo>();

    private bool _isDirty;

    public const string Element = "PackageReference";
    public const string NameAttribute = "Include";
    public const string VersionAttribute = "Version";

    public int Count => _packages.Count;

    private void ParseManifestXml()
    {
        if (IsCentralVersionManagementEnabled())
        {
            ParseCentralVersionManagement();
        }
        else
        {
            ParsePackageReferences();
        }
    }

    private string? GetCentralVersionManagementVersion(string packageName)
    {
        var directoryPackagesProperties = _directoryPackagesProperties;
        while (true)
        {
            if (directoryPackagesProperties == null)
            {
                return null;
            }

            var version = directoryPackagesProperties[packageName];
            if (version != null)
            {
                return version;
            }

            directoryPackagesProperties = directoryPackagesProperties.Parent;
        }
    }

    private void ParseCentralVersionManagement()
    {
        var packages = _manifestXml.GetElementsByTagName(Element);
        foreach (XmlNode package in packages)
        {
            var packageName = package.Attributes![NameAttribute]!.Value;
            var packageVersion = GetCentralVersionManagementVersion(packageName);
            _ = packageVersion ?? throw new Exception($"Could not find version for package {packageName}");

            var overrideVersion = package.Attributes["VersionOverride"]?.Value;
            if (overrideVersion != null)
            {
                packageVersion = overrideVersion;
            }
            Add(packageName, packageVersion);
        }
    }

    private bool IsCentralVersionManagementEnabled()
    {
        return DoesDocumentEnableCentralVersionManagement(_manifestXml) || DoesDirectoryPackagesPropsEnableCentralVersionManagement();
    }

    private bool DoesDirectoryPackagesPropsEnableCentralVersionManagement()
    {
        var directoryPackagesProperties = _directoryPackagesProperties;

        while (true)
        {
            if (directoryPackagesProperties == null)
            {
                return false;
            }

            if (directoryPackagesProperties.IsCentralVersionManagementEnabled())
            {
                return true;
            }

            directoryPackagesProperties = directoryPackagesProperties.Parent;
        }
    }

    private static bool DoesDocumentEnableCentralVersionManagement(XmlDocument document)
    {
        var packageManagement = document.SelectSingleNode("/Project/PropertyGroup/ManagePackageVersionsCentrally");
        return packageManagement is not null && packageManagement.InnerText == "true";
    }

    private void ParsePackageReferences()
    {
        var packages = _manifestXml.GetElementsByTagName(Element);
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
        if (IsCentralVersionManagementEnabled())
        {
            UpdateCentralVersionManagement(packageName, packageVersion);
        }
        else
        {
            UpdatePackageReference(packageName, packageVersion);
        }
    }

    private void UpdateCentralVersionManagement(string packageName, string packageVersion)
    {
        var node = _manifestXml.SelectSingleNode($"/Project/ItemGroup/{Element}[@{NameAttribute} = '{packageName}']");
        if (node is not { Attributes: not null })
        {
            // TODO: This should be logged as warning
            return;
        }

        var versionOverride = node.Attributes["VersionOverride"];
        if (versionOverride != null)
        {
            _isDirty = true;
            versionOverride.Value = packageVersion;
            return;
        }


        var directoryPackagesProperties = _directoryPackagesProperties;
        while (true)
        {
            if (directoryPackagesProperties == null)
            {
                return;
            }

            if (directoryPackagesProperties[packageName] != null)
            {
                directoryPackagesProperties[packageName] = packageVersion;
                return;
            }

            directoryPackagesProperties = directoryPackagesProperties.Parent;
        }
    }

    private void UpdatePackageReference(string packageName, string packageVersion)
    {
        var node = _manifestXml.SelectSingleNode($"/Project/ItemGroup/{Element}[@{NameAttribute} = '{packageName}']");
        if (node is not { Attributes: not null })
        {
            // TODO: This should be logged as warning
            return;
        }

        var versionAttribute = node.Attributes[VersionAttribute];
        if (versionAttribute != null)
        {
            _isDirty = true;
            versionAttribute.Value = packageVersion;
        }
        else
        {
            // TODO: this should be logged as a warning
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

    public PackageInfo this[string packageName] => _packages[packageName];

    public void Save()
    {
        if (_isDirty)
        {
            File.Copy(_path, _backupPath, true);
            _manifestXml.Save(_path);
            _isDirty = false;
        }

        SaveDirectoryPackagesProps();
    }

    public void Restore()
    {
        if (File.Exists(_backupPath))
        {
            File.Move(_backupPath, _path, true);
        }

        var directoryPackagesProperties = _directoryPackagesProperties;
        while (true)
        {
            if (directoryPackagesProperties == null)
            {
                return;
            }

            directoryPackagesProperties.Restore();

            directoryPackagesProperties = directoryPackagesProperties.Parent;
        }
    }

    private void SaveDirectoryPackagesProps()
    {
        var directoryPackagesProperties = _directoryPackagesProperties;
        while (true)
        {
            if (directoryPackagesProperties == null)
            {
                return;
            }

            directoryPackagesProperties.Save();

            directoryPackagesProperties = directoryPackagesProperties.Parent;
        }
    }
}
