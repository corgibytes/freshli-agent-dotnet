// The contents of this file are based on https://github.com/DataDog/dd-trace-dotnet/blob/5c41fe49801fa6b65e4b262ef7598c04dabe51c9/tracer/test/Datadog.Trace.TestHelpers/CustomTestFramework.cs
// Credit should also go to the blog post that lead me to this approach: https://andrewlock.net/tracking-down-a-hanging-xunit-test-in-ci-building-a-custom-test-framework/
//
// The original license for this code is reproduced below:
// <copyright file="CustomTestFramework.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>
//
// The contents have been modified to use different names, different constants, and team
// linting standards.

using System.Collections.Concurrent;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("Corgibytes.Freshli.Agent.DotNet.Test.DiagnosticTestFramework", "Corgibytes.Freshli.Agent.DotNet.Test")]

namespace Corgibytes.Freshli.Agent.DotNet.Test;

// ReSharper disable once UnusedType.Global
public class DiagnosticTestFramework : XunitTestFramework
{
    public DiagnosticTestFramework(IMessageSink messageSink)
        : base(messageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
    {
        return new DiagnosticExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }

    private class DiagnosticExecutor : XunitTestFrameworkExecutor
    {
        public DiagnosticExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            using var assemblyRunner = new DiagnosticAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions);
            await assemblyRunner.RunAsync();
        }
    }

    private class DiagnosticAssemblyRunner : XunitTestAssemblyRunner
    {
        public DiagnosticAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
            : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
        {
        }

        protected override async Task<RunSummary> RunTestCollectionsAsync(IMessageBus messageBus, CancellationTokenSource cancellationTokenSource)
        {
            var collections = OrderTestCollections()
                .Select(pair =>
                    new
                    {
                        Collection = pair.Item1,
                        TestCases = pair.Item2,
                        DisableParallelization = IsParallelizationDisabled(pair.Item1)
                    }
                )
                .ToList();

            var summary = new RunSummary();

            using var runner = new ConcurrentRunner();

            var tasks = collections
                .Where(t => !t.DisableParallelization)
                .Select(test =>
                    runner.RunAsync(async () =>
                        await RunTestCollectionAsync(
                            messageBus,
                            test.Collection,
                            test.TestCases,
                            cancellationTokenSource
                        )
                    )
                )
                .ToList();

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                summary.Aggregate(task.Result);
            }

            // Single threaded collections
            foreach (var test in collections.Where(t => t.DisableParallelization))
            {
                summary.Aggregate(await RunTestCollectionAsync(messageBus, test.Collection, test.TestCases, cancellationTokenSource));
            }

            return summary;
        }

        protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
        {
            return new DiagnosticTestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource).RunAsync();
        }

        private static bool IsParallelizationDisabled(ITestCollection collection)
        {
            var attr = collection.CollectionDefinition?.GetCustomAttributes(typeof(CollectionDefinitionAttribute)).SingleOrDefault();
            return attr?.GetNamedArgument<bool>(nameof(CollectionDefinitionAttribute.DisableParallelization)) is true;
        }
    }

    private class DiagnosticTestCollectionRunner : XunitTestCollectionRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public DiagnosticTestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
        {
            return new DiagnosticTestClassRunner(testClass, @class, testCases, _diagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings)
                .RunAsync();
        }
    }

    private class DiagnosticTestClassRunner : XunitTestClassRunner
    {
        public DiagnosticTestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
        {
            return new DiagnosticTestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments)
                .RunAsync();
        }
    }

    private class DiagnosticTestMethodRunner : XunitTestMethodRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;
        private const int LongTestThresholdInMinutes = 2;

        public DiagnosticTestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments)
            : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
        {
            var parameters = string.Empty;

            if (testCase.TestMethodArguments != null)
            {
                parameters = string.Join(", ", testCase.TestMethodArguments.Select(a => a?.ToString() ?? "null"));
            }

            var test = $"{TestMethod.TestClass.Class.Name}.{TestMethod.Method.Name}({parameters})";

            _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"STARTED: {test}"));

            await using var timer = new Timer(
                _ => _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"WARNING: {test} has been running for more than {LongTestThresholdInMinutes} minutes")),
                null,
                TimeSpan.FromMinutes(LongTestThresholdInMinutes),
                Timeout.InfiniteTimeSpan);

            try
            {
                var result = await base.RunTestCaseAsync(testCase);

                var status = result.Failed > 0 ? "FAILURE" : (result.Skipped > 0 ? "SKIPPED" : "SUCCESS");

                _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"{status}: {test} ({result.Time}s)"));

                return result;
            }
            catch (Exception ex)
            {
                _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"ERROR: {test} ({ex.Message})"));
                throw;
            }
        }
    }

    private class ConcurrentRunner : IDisposable
    {
        private readonly BlockingCollection<Func<Task>> _queue;

        public ConcurrentRunner()
        {
            _queue = new BlockingCollection<Func<Task>>();

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                var thread = new Thread(DoWork) { IsBackground = true };
                thread.Start();
            }
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
        }

        public Task<T> RunAsync<T>(Func<Task<T>> action)
        {
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            _queue.Add(async () =>
            {
                try
                {
                    tcs.TrySetResult(await action());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }

        private void DoWork()
        {
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                item().GetAwaiter().GetResult();
            }
        }
    }
}
