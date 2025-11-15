using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Cancelled<>))]
public class CancelledTest_TValue {

    [Fact]
    public void Constructor_WithValueAndReason_CreatesCancelledWithBoth() {
        var cancelled = new Cancelled<int>(42, "User requested cancellation");
        
        cancelled.Value.ShouldBe(42);
        cancelled.Reason.ShouldBe("User requested cancellation");
    }

    [Fact]
    public void ValueOnly_CreatesWithValueAndEmptyReason() {
        var cancelled = Cancelled<int>.ValueOnly(42);
        
        cancelled.Value.ShouldBe(42);
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void Because_CreatesWithValueAndReason() {
        var cancelled = Cancelled<string>.Because("test value", "Timeout occurred");
        
        cancelled.Value.ShouldBe("test value");
        cancelled.Reason.ShouldBe("Timeout occurred");
    }

    [Fact]
    public void ImplicitConversion_ToCancelled_PreservesReason() {
        var cancelledWithValue = new Cancelled<int>(42, "Operation cancelled");
        Cancelled cancelled = cancelledWithValue;
        
        cancelled.Reason.ShouldBe("Operation cancelled");
    }

    [Fact]
    public void ToString_ReturnsFormattedString() {
        var cancelled = new Cancelled<int>(42, "User cancelled");
        var result = cancelled.ToString();
        
        result.ShouldBe("[Cancelled]: User cancelled, [Value]: 42");
    }

    [Fact]
    public void ToString_WithEmptyReason_ReturnsFormattedString() {
        var cancelled = Cancelled<string>.ValueOnly("test");
        var result = cancelled.ToString();
        
        result.ShouldBe("[Cancelled]: , [Value]: test");
    }

    [Fact]
    public void Constructor_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var cancelled = new Cancelled<List<int>>(list, "Operation cancelled");
        
        cancelled.Value.ShouldBeSameAs(list);
        cancelled.Value.Count.ShouldBe(3);
    }

    [Fact]
    public void Constructor_WithNullValue_AllowsNull() {
        var cancelled = new Cancelled<string?>(null, "Cancelled");
        
        cancelled.Value.ShouldBeNull();
        cancelled.Reason.ShouldBe("Cancelled");
    }

    [Fact]
    public void Constructor_WithEmptyReason_CreatesWithEmptyReason() {
        var cancelled = new Cancelled<int>(42, string.Empty);
        
        cancelled.Value.ShouldBe(42);
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void ToString_WithNullValue_HandlesNull() {
        var cancelled = new Cancelled<string?>(null, "Operation cancelled");
        var result = cancelled.ToString();
        
        result.ShouldContain("[Cancelled]: Operation cancelled");
        result.ShouldContain("[Value]:");
    }

    [Fact]
    public void Constructor_WithLongReason_HandlesCorrectly() {
        var longReason = new string('a', 1000);
        var cancelled = new Cancelled<int>(42, longReason);
        
        cancelled.Reason.ShouldBe(longReason);
        cancelled.Reason.Length.ShouldBe(1000);
        cancelled.Value.ShouldBe(42);
    }

    [Fact]
    public void RecordEquality_WithSameValueAndReason_AreEqual() {
        var cancelled1 = new Cancelled<int>(42, "Same reason");
        var cancelled2 = new Cancelled<int>(42, "Same reason");
        
        cancelled1.ShouldBe(cancelled2);
    }

    [Fact]
    public void RecordEquality_WithDifferentValue_AreNotEqual() {
        var cancelled1 = new Cancelled<int>(42, "Same reason");
        var cancelled2 = new Cancelled<int>(100, "Same reason");
        
        cancelled1.ShouldNotBe(cancelled2);
    }

    [Fact]
    public void RecordEquality_WithDifferentReason_AreNotEqual() {
        var cancelled1 = new Cancelled<int>(42, "Reason 1");
        var cancelled2 = new Cancelled<int>(42, "Reason 2");
        
        cancelled1.ShouldNotBe(cancelled2);
    }

    [Fact]
    public void ValueOnly_WithComplexType_StoresCorrectly() {
        var complexValue = new { Id = 1, Name = "Test", Items = new List<int> { 1, 2, 3 } };
        var cancelled = Cancelled<object>.ValueOnly(complexValue);
        
        cancelled.Value.ShouldBe(complexValue);
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void Because_WithDifferentTypes_WorksCorrectly() {
        var intCancelled = Cancelled<int>.Because(42, "Cancelled");
        var stringCancelled = Cancelled<string>.Because("test", "Cancelled");
        var listCancelled = Cancelled<List<int>>.Because(new List<int> { 1, 2 }, "Cancelled");
        
        intCancelled.Value.ShouldBe(42);
        stringCancelled.Value.ShouldBe("test");
        listCancelled.Value.Count.ShouldBe(2);
    }
}

