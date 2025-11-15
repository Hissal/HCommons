using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Cancelled))]
public class CancelledTest {

    [Fact]
    public void Constructor_WithReason_CreatesCancelledWithReason() {
        var cancelled = new Cancelled("User requested cancellation");
        
        cancelled.Reason.ShouldBe("User requested cancellation");
    }

    [Fact]
    public void Empty_ReturnsCancelledWithEmptyReason() {
        var cancelled = Cancelled.Empty;
        
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void Because_CreatesCancelledWithReason() {
        var cancelled = Cancelled.Because("Timeout occurred");
        
        cancelled.Reason.ShouldBe("Timeout occurred");
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesCancelled() {
        Cancelled cancelled = "Operation timed out";
        
        cancelled.Reason.ShouldBe("Operation timed out");
    }

    [Fact]
    public void ToString_ReturnsFormattedString() {
        var cancelled = new Cancelled("User cancelled");
        
        cancelled.ToString().ShouldBe("[Cancelled]: User cancelled");
    }

    [Fact]
    public void ToString_WithEmptyReason_ReturnsFormattedString() {
        var cancelled = Cancelled.Empty;
        
        cancelled.ToString().ShouldBe("[Cancelled]: ");
    }

    [Fact]
    public void Constructor_WithEmptyString_CreatesWithEmptyReason() {
        var cancelled = new Cancelled(string.Empty);
        
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void Because_WithEmptyString_CreatesWithEmptyReason() {
        var cancelled = Cancelled.Because(string.Empty);
        
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void ImplicitConversion_WithEmptyString_CreatesWithEmptyReason() {
        Cancelled cancelled = string.Empty;
        
        cancelled.Reason.ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_WithLongReason_HandlesCorrectly() {
        var longReason = new string('a', 1000);
        var cancelled = new Cancelled(longReason);
        
        cancelled.Reason.ShouldBe(longReason);
        cancelled.Reason.Length.ShouldBe(1000);
    }

    [Fact]
    public void RecordEquality_WithSameReason_AreEqual() {
        var cancelled1 = new Cancelled("Same reason");
        var cancelled2 = new Cancelled("Same reason");
        
        cancelled1.ShouldBe(cancelled2);
    }

    [Fact]
    public void RecordEquality_WithDifferentReason_AreNotEqual() {
        var cancelled1 = new Cancelled("Reason 1");
        var cancelled2 = new Cancelled("Reason 2");
        
        cancelled1.ShouldNotBe(cancelled2);
    }
}

