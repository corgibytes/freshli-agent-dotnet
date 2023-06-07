namespace Corgibytes.Freshli.Agent.DotNet.Lib.NuGet;

public class PackagesManifestFinder : AbstractManifestFinder
{
    protected override string ManifestPattern => "packages.config";
    public override IPackageRepository RepositoryFor(string projectRootPath)
    {
        return new NuGetRepository();
    }

    public override IManifest ManifestFor(string projectRootPath)
    {
        return new PackagesManifest();
    }

}
