using System.Collections.Concurrent;
using System.Reflection;
using HCommons.Reflection;

namespace HCommons.Tests;

[Collection(nameof(NonParallelTests))]
public sealed class RuntimeTypeCacheTests {
    [Fact]
    public void TypesDerivedFrom_ReturnsEveryAssignableTypeExceptTheQueriedType() {
        RuntimeTypeCache.Clear();

        var types = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>();

        types.ShouldContain(typeof(IDerivedTestMarker));
        types.ShouldContain(typeof(AbstractTestMarker));
        types.ShouldContain(typeof(ConcreteTestMarker));
        types.ShouldNotContain(typeof(ITestMarker));
    }

    [Fact]
    public void TypesDerivedFrom_TypeOverloadReturnsTheCachedImmutableSnapshot() {
        RuntimeTypeCache.Clear();

        var first = RuntimeTypeCache.TypesDerivedFrom(typeof(ITestMarker));
        var second = RuntimeTypeCache.TypesDerivedFrom(typeof(ITestMarker));

        second.ShouldBeSameAs(first);
        var mutableView = first.ShouldBeAssignableTo<IList<Type>>();
        mutableView.IsReadOnly.ShouldBeTrue();
        Should.Throw<NotSupportedException>(() => mutableView.Add(typeof(string)));
    }

    [Fact]
    public void Clear_RebuildsTheSnapshotWithoutChangingItsContents() {
        RuntimeTypeCache.Clear();
        var beforeClear = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>();

        RuntimeTypeCache.Clear();
        var afterClear = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>();

        afterClear.ShouldNotBeSameAs(beforeClear);
        afterClear.ToHashSet().SetEquals(beforeClear).ShouldBeTrue();
    }

    [Fact]
    public void TypesDerivedFrom_NullTypeThrows() {
        Should.Throw<ArgumentNullException>(() => RuntimeTypeCache.TypesDerivedFrom(null!));
    }

    [Fact]
    public void Bind_NullCallbackThrows() {
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind(typeof(ITestMarker), null!, synchronizationContext: null));
    }

    [Fact]
    public void Bind_DeliversTheInitialSnapshotSynchronously() {
        RuntimeTypeCache.Clear();
        IReadOnlyList<Type>? received = null;

        using var binding = RuntimeTypeCache.Bind<ITestMarker>(types => received = types);

        received.ShouldNotBeNull();
        received.ShouldContain(typeof(ConcreteTestMarker));
    }

    [Fact]
    public void Bind_LoadedAssemblyPublishesOnlyAffectedQueriesOnTheCapturedContext() {
        RuntimeTypeCache.Clear();
        var context = new PumpSynchronizationContext();
        var disposableSnapshots = new List<IReadOnlyList<Type>>();
        var markerNotificationCount = 0;
        var disposedNotificationCount = 0;

        using var disposableBinding = RuntimeTypeCache.Bind(
            typeof(IDisposable),
            types => disposableSnapshots.Add(types),
            context);
        using var markerBinding = RuntimeTypeCache.Bind<ITestMarker>(
            _ => markerNotificationCount++,
            context);
        var disposedBinding = RuntimeTypeCache.Bind(
            typeof(IDisposable),
            _ => disposedNotificationCount++,
            context);
        disposedBinding.Dispose();
        disposedBinding.Dispose();

        var fixturePath = Path.Combine(AppContext.BaseDirectory, "HCommons.Reflection.Tests.Fixture.dll");
        File.Exists(fixturePath).ShouldBeTrue($"Fixture assembly was not copied to '{fixturePath}'.");

        var fixtureAssembly = Assembly.Load(File.ReadAllBytes(fixturePath));
        var fixtureType = fixtureAssembly.GetType(
            "HCommons.Reflection.Tests.Fixture.FixtureDisposable",
            throwOnError: true)!;

        context.WaitForPendingCallback().ShouldBeTrue("A bound query should be updated after an assembly load.");
        disposableSnapshots.Count.ShouldBe(1, "Later notifications must be posted to the captured context.");

        context.RunAll();

        disposableSnapshots.Count.ShouldBe(2);
        disposableSnapshots[^1].ShouldContain(fixtureType);
        markerNotificationCount.ShouldBe(1, "An assembly with no marker implementations must not publish that query.");
        disposedNotificationCount.ShouldBe(1, "A disposed binding must suppress pending and future notifications.");
    }

    [Fact]
    public void TypesDerivedFrom_ConcurrentFirstReadsReturnTheSameSnapshot() {
        RuntimeTypeCache.Clear();
        var snapshots = new ConcurrentBag<IReadOnlyList<Type>>();

        Parallel.For(0, 64, _ => snapshots.Add(RuntimeTypeCache.TypesDerivedFrom<ITestMarker>()));

        var expected = snapshots.First();
        snapshots.ShouldAllBe(snapshot => ReferenceEquals(snapshot, expected));
    }

    [Fact]
    public void LateLoadedFixture_ContainsACompleteGeneratedDisposableCatalog() {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "HCommons.Reflection.Tests.Fixture.dll");
        var fixtureAssembly = Assembly.Load(File.ReadAllBytes(fixturePath));

        var catalog = fixtureAssembly
            .GetCustomAttributes<RuntimeTypeCacheGeneratedTypesAttribute>()
            .Single(attribute => attribute.BaseType == typeof(IDisposable));

        catalog.IsComplete.ShouldBeTrue();
    }

    interface ITestMarker;

    interface IDerivedTestMarker : ITestMarker;

    abstract class AbstractTestMarker : ITestMarker;

    sealed class ConcreteTestMarker : AbstractTestMarker;

    sealed class PumpSynchronizationContext : SynchronizationContext {
        readonly ConcurrentQueue<Action> _callbacks = new();

        public override void Post(SendOrPostCallback callback, object? state) =>
            _callbacks.Enqueue(() => callback(state));

        public bool WaitForPendingCallback() => SpinWait.SpinUntil(() => !_callbacks.IsEmpty, TimeSpan.FromSeconds(5));

        public void RunAll() {
            while (_callbacks.TryDequeue(out var callback)) {
                callback();
            }
        }
    }
}
