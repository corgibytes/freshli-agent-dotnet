using System.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Corgibytes.Freshli.Agent.DotNet.Lib
{
    // TODO: Move in the direction of `IManifest` instances being mostly immutable, and have them be instantiated by calling an `IManifestParser.Parse` method. This is going to help with `async`/`await` implementation.
    public abstract class AbstractManifest : IManifest
    {
        private readonly ILogger<AbstractManifest> _logger = Logging.Logger<AbstractManifest>();

        private readonly IDictionary<string, PackageInfo> _packages =
            new Dictionary<string, PackageInfo>();

        public int Count => _packages.Count;
        public abstract bool UsesExactMatches { get; }

        public IEnumerator<PackageInfo> GetEnumerator()
        {
            return _packages.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string packageName, string packageVersion)
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

        public void Clear()
        {
            _packages.Clear();
        }

        public abstract void Parse(string contents);
        public PackageInfo this[string packageName] => _packages[packageName];
    }
}
