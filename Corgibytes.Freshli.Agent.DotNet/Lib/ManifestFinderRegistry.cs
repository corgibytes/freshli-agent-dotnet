﻿namespace Corgibytes.Freshli.Agent.DotNet.Lib;

public class ManifestFinderRegistry
{
    public IList<AbstractManifestFinder> Finders { get; } = new List<AbstractManifestFinder>();

    public void Register<TFinder>() where TFinder : AbstractManifestFinder, new()
    {
        Finders.Add(new TFinder());
    }
}