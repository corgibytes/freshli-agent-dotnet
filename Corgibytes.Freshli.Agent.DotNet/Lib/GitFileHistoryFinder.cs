using LibGit2Sharp;

namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class GitFileHistoryFinder : IFileHistoryFinder
{
    private readonly Dictionary<string, string> _cloneLocations = new();

    private string NormalizeLocation(string projectRootPath)
    {
        if (Repository.IsValid(projectRootPath))
        {
            return projectRootPath;
        }

        if (_cloneLocations.TryGetValue(projectRootPath, out var location))
        {
            return location;
        }

        if (IsCloneable(projectRootPath))
        {
            var cloneLocation = GenerateTempCloneLocation();
            Repository.Clone(projectRootPath, cloneLocation);
            _cloneLocations[projectRootPath] = cloneLocation;
            return cloneLocation;
        }

        return projectRootPath;
    }

    public bool DoesPathContainHistorySource(string projectRootPath)
    {
        var result = Repository.IsValid(projectRootPath);
        if (!result)
        {
            result = IsCloneable(projectRootPath);
        }

        return result;
    }

    private static string GenerateTempCloneLocation()
    {
        return Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString()
        );
    }

    private static bool IsCloneable(string url)
    {
        var result = true;
        var options = new CloneOptions { Checkout = false };

        var tempFolder = GenerateTempCloneLocation();

        try
        {
            Repository.Clone(url, tempFolder, options);
        }
        catch (Exception exception) when (exception is NotFoundException or LibGit2SharpException)
        {
            result = false;
        }

        if (Directory.Exists(tempFolder))
        {
            new DirectoryInfo(tempFolder).Delete();
        }

        return result;
    }

    public IFileHistory FileHistoryOf(
        string projectRootPath,
        string targetFile
    )
    {
        return new GitFileHistory(NormalizeLocation(projectRootPath), targetFile);
    }

    public bool Exists(string projectRootPath, string filePath)
    {
        var clonedProjectRoot = NormalizeLocation(projectRootPath);
        return Directory.GetFiles(clonedProjectRoot, filePath).Any();
    }

    public string ReadAllText(string projectRootPath, string filePath)
    {
        var clonedProjectRoot = NormalizeLocation(projectRootPath);
        return File.ReadAllText(Path.Combine(clonedProjectRoot, filePath));
    }

    public string[] GetManifestFilenames(
        string projectRootPath,
        string pattern
    )
    {
        var clonedProjectRoot = NormalizeLocation(projectRootPath);
        return Directory.GetFiles(clonedProjectRoot,
                pattern,
                SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(clonedProjectRoot, f))
            .ToArray();
    }
}
