using HCommons.Reflection;

namespace HCommons.Tests;

public sealed class RuntimeTypeFilterTests {
    [Fact]
    public void DefaultFilter_MatchesEveryNonNullType() {
        default(RuntimeTypeFilter).Matches(typeof(string)).ShouldBeTrue();
        Should.Throw<ArgumentNullException>(() => default(RuntimeTypeFilter).Matches(null!));
    }

    [Fact]
    public void BuiltInFilters_ApplyTheirDocumentedConditions() {
        RuntimeTypeFilters.Concrete().Matches(typeof(string)).ShouldBeTrue();
        RuntimeTypeFilters.Concrete().Matches(typeof(IDisposable)).ShouldBeFalse();
        RuntimeTypeFilters.Concrete().Matches(typeof(AbstractType)).ShouldBeFalse();

        RuntimeTypeFilters.Public().Matches(typeof(string)).ShouldBeTrue();
        RuntimeTypeFilters.Public().Matches(typeof(PrivateType)).ShouldBeFalse();

        RuntimeTypeFilters.Closed().Matches(typeof(List<int>)).ShouldBeTrue();
        RuntimeTypeFilters.Closed().Matches(typeof(List<>)).ShouldBeFalse();

        RuntimeTypeFilters.HasPublicParameterlessConstructor()
            .Matches(typeof(PublicConstructorType))
            .ShouldBeTrue();
        RuntimeTypeFilters.HasPublicParameterlessConstructor().Matches(typeof(string)).ShouldBeFalse();

        RuntimeTypeFilters.Instantiable().Matches(typeof(PublicConstructorType)).ShouldBeTrue();
        RuntimeTypeFilters.Instantiable().Matches(typeof(AbstractType)).ShouldBeFalse();
        RuntimeTypeFilters.Instantiable().Matches(typeof(List<>)).ShouldBeFalse();
    }

    [Fact]
    public void ChainedBuiltIns_UseImplicitAnd() {
        var filter = RuntimeTypeFilters.Concrete().Public().Closed();

        filter.Matches(typeof(string)).ShouldBeTrue();
        filter.Matches(typeof(IDisposable)).ShouldBeFalse();
        filter.Matches(typeof(List<>)).ShouldBeFalse();
    }

    [Fact]
    public void AndOrAndInstanceNot_AreLeftAssociative() {
        var isString = RuntimeTypeFilters.Where(new ExactTypeRule(typeof(string)));
        var isInt = RuntimeTypeFilters.Where(new ExactTypeRule(typeof(int)));
        var filter = isString.Or(isInt).Not(isString);

        filter.Matches(typeof(string)).ShouldBeFalse();
        filter.Matches(typeof(int)).ShouldBeTrue();
        filter.Matches(typeof(decimal)).ShouldBeFalse();
    }

    [Fact]
    public void StaticNot_NegatesTheCompleteExpression() {
        var stringOrInt = RuntimeTypeFilters
            .Where(new ExactTypeRule(typeof(string)))
            .Or(RuntimeTypeFilters.Where(new ExactTypeRule(typeof(int))));
        var filter = RuntimeTypeFilters.Not(stringOrInt);

        filter.Matches(typeof(string)).ShouldBeFalse();
        filter.Matches(typeof(int)).ShouldBeFalse();
        filter.Matches(typeof(decimal)).ShouldBeTrue();
    }

    [Fact]
    public void RecordRules_GiveFiltersStructuralEquality() {
        var first = RuntimeTypeFilters.Where(new ExactTypeRule(typeof(string)));
        var second = RuntimeTypeFilters.Where(new ExactTypeRule(typeof(string))).Cached();
        var different = RuntimeTypeFilters.Where(new ExactTypeRule(typeof(int)));

        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
        first.ShouldNotBe(different);
        first.IsCacheable.ShouldBeTrue();
    }

    [Fact]
    public void DelegateWhere_IsUncacheableAndShortCircuitsNormally() {
        var calls = 0;
        var filter = RuntimeTypeFilters
            .Where(type => type == typeof(string))
            .Or(RuntimeTypeFilters.Where(type => {
                calls++;
                return type == typeof(int);
            }));
        filter = filter.Cached();

        filter.IsCacheable.ShouldBeFalse();
        filter.Matches(typeof(string)).ShouldBeTrue();
        calls.ShouldBe(0);
        filter.Matches(typeof(int)).ShouldBeTrue();
        calls.ShouldBe(1);

        var falseCalls = 0;
        var falseOrMatchAll = RuntimeTypeFilters
            .Where(_ => {
                falseCalls++;
                return false;
            })
            .Or(default);

        falseOrMatchAll.Matches(typeof(string)).ShouldBeTrue();
        falseCalls.ShouldBe(1, "left operands must retain their observable evaluation order");
    }

    [Fact]
    public void Where_NullInputsThrow() {
        Should.Throw<ArgumentNullException>(() => RuntimeTypeFilters.Where((Func<Type, bool>)null!));
        Should.Throw<ArgumentNullException>(() => RuntimeTypeFilters.Where((RuntimeTypeFilterRule)null!));
    }

    abstract class AbstractType;

    sealed class PrivateType;

    sealed class PublicConstructorType {
        public PublicConstructorType() { }
    }

    sealed record ExactTypeRule(Type Expected) : RuntimeTypeFilterRule {
        public override bool Matches(Type type) => type == Expected;
    }
}
