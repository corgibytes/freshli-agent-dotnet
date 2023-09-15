using System.Xml;

namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class NuGetDirectoryPackagesProperties
{
    public NuGetDirectoryPackagesProperties? Parent { get; private set; }
    private readonly string _directoryPackagesPropsFilePath;
    private readonly XmlDocument _propsXml;
    private bool _isDirty;
    private string _backupDirectoryPackagesPropsFilePath;
    private const string BackupSuffix = ".versionsBackup";

    private NuGetDirectoryPackagesProperties(string directoryPackagesPropsFilePath)
    {
        _directoryPackagesPropsFilePath = directoryPackagesPropsFilePath;

        _propsXml = new XmlDocument();
        _propsXml.Load(directoryPackagesPropsFilePath);

        LoadParent();
        _backupDirectoryPackagesPropsFilePath = _directoryPackagesPropsFilePath + BackupSuffix;
    }

    private void LoadParent()
    {
        var parentPropsFilePath = GetParentPropsFilePath();
        if (parentPropsFilePath is null)
        {
            return;
        }

        Parent = new NuGetDirectoryPackagesProperties(parentPropsFilePath);
    }

    private string? GetParentPropsFilePath()
    {
        var importNode = _propsXml.SelectSingleNode("/Project/Import[@Project]");

        var importProject = importNode?.Attributes?["Project"]?.Value;
        if (importProject is null)
        {
            return null;
        }

        const string nearestParentPropsDirective = "$([MSBuild]::GetPathOfFileAbove(Directory.Packages.props, $(MSBuildThisFileDirectory)..))";
        if (importProject != nearestParentPropsDirective)
        {
            // TODO: Log that we didn't attempt to parse the expression/path
            return null;
        }

        var directoryPackagesPropsDirPath = Path.GetDirectoryName(_directoryPackagesPropsFilePath);
        if (directoryPackagesPropsDirPath == null)
        {
            return null;
        }

        var parentPackagesPropsDirPath = Path.Combine(directoryPackagesPropsDirPath, "..");
        return FindNearestDirectoryPackagesPropsFile(parentPackagesPropsDirPath);
    }

    public void Save()
    {
        if (!_isDirty)
        {
            return;
        }

        File.Copy(_directoryPackagesPropsFilePath, _backupDirectoryPackagesPropsFilePath, true);
        _propsXml.Save(_directoryPackagesPropsFilePath);
        _isDirty = false;
    }

    public void Restore()
    {
        if (File.Exists(_backupDirectoryPackagesPropsFilePath))
        {
            File.Move(_backupDirectoryPackagesPropsFilePath, _directoryPackagesPropsFilePath, true);
        }
    }

    public static NuGetDirectoryPackagesProperties? Build(string projectDirPath)
    {
        var nearestDirectoryPropsFile = FindNearestDirectoryPackagesPropsFile(projectDirPath);
        return nearestDirectoryPropsFile == null ? null : new NuGetDirectoryPackagesProperties(nearestDirectoryPropsFile);
    }

    private static string? FindNearestDirectoryPackagesPropsFile(string projectDirPath)
    {
        while (true)
        {
            var directoryPropsFilePath = Path.Combine(projectDirPath, "Directory.Packages.props");
            if (File.Exists(directoryPropsFilePath))
            {
                return directoryPropsFilePath;
            }

            if (IsProjectRoot(projectDirPath))
            {
                return null;
            }

            var parentDirPath = Path.GetDirectoryName(projectDirPath);
            if (parentDirPath is null)
            {
                return null;
            }

            projectDirPath = parentDirPath;
        }
    }

    private static bool IsProjectRoot(string projectDirPath)
    {
        return Directory.Exists(Path.Combine(projectDirPath, ".git"));
    }

    public bool IsCentralVersionManagementEnabled()
    {
        var packageManagementNode = _propsXml.SelectSingleNode("/Project/PropertyGroup/ManagePackageVersionsCentrally");
        if (packageManagementNode is null)
        {
            return false;
        }

        var packageManagementValue = packageManagementNode.InnerText;
        return packageManagementValue == "true";
    }

    public string? this[string packageName]
    {
        get
        {
            var packageVersionNode = _propsXml.SelectSingleNode($"/Project/ItemGroup/PackageVersion[@Include='{packageName}']/@Version");
            return packageVersionNode?.InnerText;
        }

        set
        {
            var packageVersionNode = _propsXml.SelectSingleNode($"/Project/ItemGroup/PackageVersion[@Include='{packageName}']/@Version");
            if (packageVersionNode == null)
            {
                return;
            }

            _isDirty = true;
            packageVersionNode.InnerText = value!;
        }
    }
}
