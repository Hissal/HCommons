using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using HCommons.Disposables;
using Perfolizer.Horology;

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args: null, new AccurateConfig());

public sealed class QuickConfig : ManualConfig {
    public QuickConfig() {
        var quickJob = Job.Default
            .WithRuntime(CoreRuntime.Core90)
            .WithPlatform(Platform.X64)
            .WithJit(Jit.RyuJit)
            .WithId("Quick")
            .WithLaunchCount(1) // Single process for speed
            .WithWarmupCount(3) // Minimal warmup
            .WithIterationTime(TimeInterval.FromMilliseconds(250)) // Shorter iterations
            .WithMinIterationCount(5) // Fewer samples
            .WithMaxIterationCount(10)
            .WithStrategy(RunStrategy.Throughput)
            .WithGcServer(true)
            .WithGcForce(false); // Skip forced GC for speed

        AddJob(quickJob);
        AddLogger(ConsoleLogger.Default);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddColumnProvider(DefaultColumnProviders.Instance);
        AddExporter(MarkdownExporter.GitHub);
        Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
    }
}

public sealed class AccurateConfig : ManualConfig {
    public AccurateConfig() {
        var accurateJob = Job.Default
            .WithRuntime(CoreRuntime.Core90) // Pin runtime
            .WithPlatform(Platform.X64)
            .WithJit(Jit.RyuJit)
            .WithId("Accurate")
            .WithLaunchCount(3) // Increased: Multiple processes to average OS noise
            .WithWarmupCount(15) // Increased: Longer warmup to reach steady state
            .WithIterationTime(TimeInterval.FromMilliseconds(500)) // Increased: More time per iteration
            .WithMinIterationCount(30) // Increased: Enough samples
            .WithMaxIterationCount(50)
            .WithUnrollFactor(1) // Avoid unrolling distortion for micro-benchmarks
            .WithStrategy(RunStrategy.Throughput)
            .WithGcServer(true) // Throughput-friendly and consistent GC
            .WithGcConcurrent(false) // Reduce background GC noise
            .WithGcForce(true) // Force full GC between iterations
            .WithAffinity(new IntPtr(1)) // Pin to a single CPU core (core 0)
            .WithEnvironmentVariables( // Stabilize JIT behavior
                new EnvironmentVariable("COMPlus_ReadyToRun", "0"),
                new EnvironmentVariable("COMPlus_TieredCompilation", "0"));

        AddJob(accurateJob);

        // Add at least one logger to see progress
        AddLogger(ConsoleLogger.Default);

        AddDiagnoser(MemoryDiagnoser.Default);
        // Optional if you want to spot thread switching/contention (adds overhead):
        // AddDiagnoser(new ThreadingDiagnoser());

        AddColumnProvider(DefaultColumnProviders.Instance);
        AddExporter(MarkdownExporter.GitHub);

        WithOptions(ConfigOptions.KeepBenchmarkFiles); // Keep artifacts for inspection
        Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
    }
}

// [MemoryDiagnoser]
// public class BagAddBench {
//     DisposableBag bag;
//     ConcurrentDisposableBag concurrentBag;
//
//     [Params(10, 100, 1000)]
//     public int Count;
//
//     IDisposable[] disposables;
//     
//     [IterationSetup]
//     public void IterationSetup() {
//         bag = new DisposableBag();
//         concurrentBag = new ConcurrentDisposableBag();
//         
//         disposables = new IDisposable[Count];
//         for (int i = 0; i < disposables.Length; i++) {
//             disposables[i] = Disposable.Empty;
//         }
//     }
//
//     [IterationCleanup]
//     public void IterationCleanup() {
//         bag.Dispose();
//         concurrentBag.Dispose();
//     }
//     
//     [Benchmark]
//     public void Bag_Add() {
//         foreach (var disposable in disposables) {
//             bag.Add(disposable);
//         }
//     }
//
//     [Benchmark]
//     public void ConcurrentBag_Add() {
//         foreach (var disposable in disposables) {
//             concurrentBag.Add(disposable);
//         }
//     }
// }
//
// [MemoryDiagnoser]
// public class BagClearBench {
//     DisposableBag bag;
//     ConcurrentDisposableBag concurrentBag;
//
//     [Params(10, 100, 1000)]
//     public int Count;
//
//     [Params(true, false)]
//     public bool Throwing;
//
//     IDisposable[] disposables;
//     
//     [IterationSetup]
//     public void IterationSetup() {
//         bag = new DisposableBag();
//         concurrentBag = new ConcurrentDisposableBag();
//         
//         disposables = new IDisposable[Count];
//         for (int i = 0; i < Count; i++) {
//             if (Throwing && i % 3 == 0)
//                 disposables[i] = Disposable.Action(() => throw new InvalidOperationException());
//             else
//                 disposables[i] = Disposable.Empty;
//         }
//         
//         // Populate the bags - this overhead is NOT measured
//         foreach (var disposable in disposables) {
//             bag.Add(disposable);
//             concurrentBag.Add(disposable);
//         }
//     }
//
//     [IterationCleanup]
//     public void IterationCleanup() {
//         bag.Dispose();
//         concurrentBag.Dispose();
//     }
//     
//     [Benchmark]
//     public void Bag_Clear() {
//         try {
//             bag.Clear();
//         }
//         catch {
//             // Swallow exceptions from throwing disposables
//         }
//     }
//
//     [Benchmark]
//     public void ConcurrentBag_Clear() {
//         try {
//             concurrentBag.Clear();
//         }
//         catch {
//             // Swallow exceptions from throwing disposables
//         }
//     }
// }
//
// [MemoryDiagnoser]
// public class BagDisposeBench {
//     DisposableBag bag;
//     ConcurrentDisposableBag concurrentBag;
//
//     [Params(10, 100, 1000)]
//     public int Count;
//
//     [Params(true, false)]
//     public bool Throwing;
//
//     IDisposable[] disposables;
//     
//     [IterationSetup]
//     public void IterationSetup() {
//         bag = new DisposableBag();
//         concurrentBag = new ConcurrentDisposableBag();
//         
//         disposables = new IDisposable[Count];
//         for (int i = 0; i < Count; i++) {
//             if (Throwing && i % 3 == 0)
//                 disposables[i] = Disposable.Action(() => throw new InvalidOperationException());
//             else
//                 disposables[i] = Disposable.Empty;
//         }
//         
//         // Populate the bags - this overhead is NOT measured
//         foreach (var disposable in disposables) {
//             bag.Add(disposable);
//             concurrentBag.Add(disposable);
//         }
//     }
//     
//     [Benchmark]
//     public void Bag_Dispose() {
//         try {
//             bag.Dispose();
//         }
//         catch {
//             // Swallow exceptions from throwing disposables
//         }
//     }
//
//     [Benchmark]
//     public void ConcurrentBag_Dispose() {
//         try {
//             concurrentBag.Dispose();
//         }
//         catch {
//             // Swallow exceptions from throwing disposables
//         }
//     }
// }
//
// [MemoryDiagnoser]
// public class BagLifecycleBench {
//     [Params(10, 100, 1000)]
//     public int Count;
//
//     [Params(true, false)]
//     public bool Throwing;
//
//     IDisposable[] disposables;
//     
//     [IterationSetup]
//     public void IterationSetup() {
//         disposables = new IDisposable[Count];
//         for (int i = 0; i < Count; i++) {
//             if (Throwing && i % 3 == 0)
//                 disposables[i] = Disposable.Action(() => throw new InvalidOperationException());
//             else
//                 disposables[i] = Disposable.Empty;
//         }
//     }
//     
//     [Benchmark]
//     public void Bag_Lifecycle() {
//         var bag = new DisposableBag();
//         
//         // Add phase
//         foreach (var disposable in disposables) {
//             bag.Add(disposable);
//         }
//         
//         // Clear phase
//         try {
//             bag.Clear(keepAllocatedArray: true);
//         }
//         catch {
//             // Swallow exceptions
//         }
//         
//         // Re-populate
//         foreach (var disposable in disposables) {
//             bag.Add(disposable);
//         }
//         
//         // Dispose phase
//         try {
//             bag.Dispose();
//         }
//         catch {
//             // Swallow exceptions
//         }
//     }
//
//     [Benchmark]
//     public void ConcurrentBag_Lifecycle() {
//         var concurrentBag = new ConcurrentDisposableBag();
//         
//         // Add phase
//         foreach (var disposable in disposables) {
//             concurrentBag.Add(disposable);
//         }
//         
//         // Clear phase
//         try {
//             concurrentBag.Clear(keepAllocatedArray: true);
//         }
//         catch {
//             // Swallow exceptions
//         }
//         
//         // Re-populate
//         foreach (var disposable in disposables) {
//             concurrentBag.Add(disposable);
//         }
//         
//         // Dispose phase
//         try {
//             concurrentBag.Dispose();
//         }
//         catch {
//             // Swallow exceptions
//         }
//     }
// }