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
    public void TypesDerivedFrom_FilterReturnsAnImmutableMatchingSnapshot() {
        RuntimeTypeCache.Clear();

        var types = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(
            type => !type.IsAbstract && !type.IsInterface);

        types.ShouldContain(typeof(ConcreteTestMarker));
        types.ShouldNotContain(typeof(AbstractTestMarker));
        types.ShouldNotContain(typeof(IDerivedTestMarker));
        var mutableView = types.ShouldBeAssignableTo<IList<Type>>();
        mutableView.IsReadOnly.ShouldBeTrue();
        Should.Throw<NotSupportedException>(() => mutableView.Add(typeof(string)));
    }

    [Fact]
    public void TypesDerivedFrom_TypeFilterOverloadAppliesThePredicate() {
        RuntimeTypeCache.Clear();

        var types = RuntimeTypeCache.TypesDerivedFrom(
            typeof(ITestMarker),
            type => type.IsAbstract && !type.IsInterface);

        types.ShouldBe(new[] { typeof(AbstractTestMarker) }, ignoreOrder: true);
    }

    [Fact]
    public void TypesDerivedFrom_DescriptorOverloadAppliesBuiltInFilters() {
        RuntimeTypeCache.Clear();

        var types = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(
            RuntimeTypeFilters.Concrete());

        types.ShouldBe(new[] { typeof(ConcreteTestMarker) }, ignoreOrder: true);
    }

    [Fact]
    public void TypesDerivedFrom_CachedDescriptorIsReusedByEquivalentUncachedCalls() {
        RuntimeTypeCache.Clear();
        var filter = RuntimeTypeFilters.Concrete();

        var firstUncached = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);
        var secondUncached = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);
        var cached = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter.Cached());
        var reused = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);

        secondUncached.ShouldNotBeSameAs(firstUncached);
        cached.ShouldNotBeSameAs(secondUncached);
        reused.ShouldBeSameAs(cached);
    }

    [Fact]
    public void TypesDerivedFrom_EqualRecordRulesShareCachedSnapshots() {
        RuntimeTypeCache.Clear();
        var cachedFilter = RuntimeTypeFilters
            .Where(new ExactTypeRule(typeof(ConcreteTestMarker)))
            .Cached();
        var equivalentFilter = RuntimeTypeFilters
            .Where(new ExactTypeRule(typeof(ConcreteTestMarker)));

        var cached = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(cachedFilter);
        var reused = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(equivalentFilter);

        reused.ShouldBeSameAs(cached);
    }

    [Fact]
    public void TypesDerivedFrom_ClearInvalidatesDescriptorSnapshots() {
        RuntimeTypeCache.Clear();
        var filter = RuntimeTypeFilters.Concrete().Cached();
        var beforeClear = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);

        RuntimeTypeCache.Clear();
        var afterClear = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);

        afterClear.ShouldNotBeSameAs(beforeClear);
        afterClear.ShouldBe(beforeClear, ignoreOrder: true);
    }

    [Fact]
    public void TypesDerivedFrom_DelegateDescriptorRemainsUncachedAndReevaluatesCapturedState() {
        RuntimeTypeCache.Clear();
        var includeTypes = true;
        var filter = RuntimeTypeFilters.Where(_ => includeTypes);
        filter = filter.Cached();

        var included = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);
        includeTypes = false;
        var excluded = RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(filter);

        included.ShouldNotBeEmpty();
        excluded.ShouldBeEmpty();
        excluded.ShouldNotBeSameAs(included);
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
    public void TypesDerivedFrom_NullFilterThrows() {
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.TypesDerivedFrom<ITestMarker>(null!));
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.TypesDerivedFrom(typeof(ITestMarker), null!));
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.TypesDerivedFrom(null!, _ => true));
    }

    [Fact]
    public void Bind_NullCallbackThrows() {
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind(typeof(ITestMarker), null!, synchronizationContext: null));
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind<ITestMarker>(_ => true, null!));
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind(typeof(ITestMarker), _ => true, null!));
    }

    [Fact]
    public void Bind_NullFilterThrows() {
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind<ITestMarker>(null!, _ => { }));
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind(typeof(ITestMarker), null!, _ => { }));
        Should.Throw<ArgumentNullException>(() =>
            RuntimeTypeCache.Bind(null!, _ => true, _ => { }));
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
    public void Bind_FilterDeliversTheInitialFilteredSnapshotSynchronously() {
        RuntimeTypeCache.Clear();
        IReadOnlyList<Type>? received = null;

        using var binding = RuntimeTypeCache.Bind<ITestMarker>(
            type => !type.IsAbstract && !type.IsInterface,
            types => received = types);

        received.ShouldNotBeNull();
        received.ShouldContain(typeof(ConcreteTestMarker));
        received.ShouldNotContain(typeof(AbstractTestMarker));
        received.ShouldNotContain(typeof(IDerivedTestMarker));
    }

    [Fact]
    public void Bind_DescriptorDeliversTheInitialFilteredSnapshotSynchronously() {
        RuntimeTypeCache.Clear();
        IReadOnlyList<Type>? received = null;

        using var binding = RuntimeTypeCache.Bind<ITestMarker>(
            RuntimeTypeFilters.Concrete(),
            types => received = types);

        received.ShouldNotBeNull();
        received.ShouldBe(new[] { typeof(ConcreteTestMarker) }, ignoreOrder: true);
    }

    [Fact]
    public void Bind_CachedDescriptorSharesTheInitialSnapshotAcrossBindings() {
        RuntimeTypeCache.Clear();
        var filter = RuntimeTypeFilters.Concrete().Cached();
        IReadOnlyList<Type>? first = null;
        IReadOnlyList<Type>? second = null;

        using var firstBinding = RuntimeTypeCache.Bind<ITestMarker>(filter, types => first = types);
        using var secondBinding = RuntimeTypeCache.Bind<ITestMarker>(filter, types => second = types);

        first.ShouldNotBeNull();
        second.ShouldBeSameAs(first);
    }

    [Fact]
    public void Bind_LoadedAssemblyPublishesOnlyAffectedQueriesOnTheCapturedContext() {
        RuntimeTypeCache.Clear();
        var context = new PumpSynchronizationContext();
        var disposableSnapshots = new List<IReadOnlyList<Type>>();
        var filteredNotificationCount = 0;
        var markerNotificationCount = 0;
        var disposedNotificationCount = 0;

        using var disposableBinding = RuntimeTypeCache.Bind(
            typeof(IDisposable),
            types => disposableSnapshots.Add(types),
            context);
        using var filteredBinding = RuntimeTypeCache.Bind(
            typeof(IDisposable),
            _ => false,
            _ => filteredNotificationCount++,
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

        context.RunUntil(
                () => disposableSnapshots.Any(snapshot => snapshot.Contains(fixtureType)),
                TimeSpan.FromSeconds(5))
            .ShouldBeTrue("The fixture update should be delivered on the captured context.");

        disposableSnapshots.Count.ShouldBeGreaterThan(1);
        disposableSnapshots[^1].ShouldContain(fixtureType);
        filteredNotificationCount.ShouldBe(1, "A binding should publish only when its filtered result changes.");
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

    sealed record ExactTypeRule(Type Expected) : RuntimeTypeFilterRule {
        public override bool Matches(Type type) => type == Expected;
    }

    sealed class PumpSynchronizationContext : SynchronizationContext {
        readonly ConcurrentQueue<Action> _callbacks = new();

        public override void Post(SendOrPostCallback callback, object? state) =>
            _callbacks.Enqueue(() => callback(state));

        public bool WaitForPendingCallback() => SpinWait.SpinUntil(() => !_callbacks.IsEmpty, TimeSpan.FromSeconds(5));

        public bool RunUntil(Func<bool> condition, TimeSpan timeout) {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (!condition()) {
                if (stopwatch.Elapsed >= timeout) {
                    return false;
                }

                _ = SpinWait.SpinUntil(() => !_callbacks.IsEmpty, TimeSpan.FromMilliseconds(25));
                RunAll();
            }

            return true;
        }

        public void RunAll() {
            while (_callbacks.TryDequeue(out var callback)) {
                callback();
            }
        }
    }
}
