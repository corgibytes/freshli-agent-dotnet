// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Corgibytes.Freshli.Agent.DotNet.Commands;

namespace Corgibytes.Freshli.Agent.DotNet;

public static class Program
{
    public static int Main(params string[] args)
    {
        return new CommandLineBuilder(new MainCommand())
            .UseDefaults()
            .Build()
            .Invoke(args);
    }
}
