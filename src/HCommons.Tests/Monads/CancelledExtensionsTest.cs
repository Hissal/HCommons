using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(CancelledExtensions))]
public class CancelledExtensionsTest {

    [Fact]
    public void WithValue_AddsValueToCancelled() {
        var cancelled = new Cancelled("User requested cancellation");
        var cancelledWithValue = cancelled.WithValue(42);
        
        cancelledWithValue.Value.ShouldBe(42);
    }

    [Fact]
    public void WithValue_PreservesReason() {
        var cancelled = new Cancelled("User requested cancellation");
        var cancelledWithValue = cancelled.WithValue(42);
        
        cancelledWithValue.Reason.ShouldBe("User requested cancellation");
    }

    [Fact]
    public void WithValue_WithNullValue_AllowsNull() {
        var cancelled = new Cancelled("Cancelled");
        var cancelledWithValue = cancelled.WithValue<string?>(null);
        
        cancelledWithValue.Value.ShouldBeNull();
    }

    [Fact]
    public void WithValue_WithReferenceType_StoresReference() {
        var cancelled = new Cancelled("Operation cancelled");
        var list = new List<int> { 1, 2, 3 };
        var cancelledWithValue = cancelled.WithValue(list);
        
        cancelledWithValue.Value.ShouldBeSameAs(list);
    }

    [Fact]
    public void WithoutValue_RemovesValueFromCancelled() {
        var cancelledWithValue = new Cancelled<int>(42, "User requested cancellation");
        var cancelled = cancelledWithValue.WithoutValue();
        
        cancelled.Reason.ShouldBe("User requested cancellation");
    }
    
    [Fact]
    public void WithoutValue_PreservesReason() {
        var cancelledWithValue = new Cancelled<string>("test", "Timeout occurred");
        var cancelled = cancelledWithValue.WithoutValue();
        
        cancelled.Reason.ShouldBe("Timeout occurred");
    }
}

