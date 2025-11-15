using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult<,,>))]
public class OperationResult_3_Test {

    [Fact]
    public void Success_CreatesSuccessResult() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Success_SetsIsFailureAndIsCancelledToFalse() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        result.IsFailure.ShouldBeFalse();
        result.IsCancelled.ShouldBeFalse();
    }

    [Fact]
    public void Success_StoresValue() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        result.SuccessValue.ShouldBe(42);
    }

    [Fact]
    public void Failure_CreatesFailureResult() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Failure_SetsIsSuccessAndIsCancelledToFalse() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        result.IsSuccess.ShouldBeFalse();
        result.IsCancelled.ShouldBeFalse();
    }

    [Fact]
    public void Failure_StoresFailureValue() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        result.FailureValue.ShouldBe("error");
    }

    [Fact]
    public void Cancelled_CreatesCancelledResult() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        result.IsCancelled.ShouldBeTrue();
    }

    [Fact]
    public void Cancelled_SetsIsSuccessAndIsFailureToFalse() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Cancelled_StoresCancelledValue() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        result.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void ImplicitConversion_FromSuccessValue_CreatesSuccess() {
        OperationResult<int, string, bool> result = 42;
        
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromSuccessValue_StoresValue() {
        OperationResult<int, string, bool> result = 42;
        
        result.SuccessValue.ShouldBe(42);
    }

    [Fact]
    public void ImplicitConversion_FromFailureValue_CreatesFailure() {
        OperationResult<int, string, bool> result = "error";
        
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromFailureValue_StoresValue() {
        OperationResult<int, string, bool> result = "error";
        
        result.FailureValue.ShouldBe("error");
    }

    [Fact]
    public void ImplicitConversion_FromCancelledValue_CreatesCancelled() {
        OperationResult<int, string, bool> result = true;
        
        result.IsCancelled.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromCancelledValue_StoresValue() {
        OperationResult<int, string, bool> result = true;
        
        result.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void TryGetSuccess_WhenSuccess_ReturnsTrue() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        var success = result.TryGetSuccess(out var value);
        
        success.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetSuccess_WhenFailure_ReturnsFalse() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        var success = result.TryGetSuccess(out _);
        
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryGetSuccess_WhenCancelled_ReturnsFalse() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        var success = result.TryGetSuccess(out _);
        
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryGetFailure_WhenFailure_ReturnsTrue() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        var hasFailure = result.TryGetFailure(out var value);
        
        hasFailure.ShouldBeTrue();
        value.ShouldBe("error");
    }

    [Fact]
    public void TryGetFailure_WhenSuccess_ReturnsFalse() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        var hasFailure = result.TryGetFailure(out _);
        
        hasFailure.ShouldBeFalse();
    }

    [Fact]
    public void TryGetFailure_WhenCancelled_ReturnsFalse() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        var hasFailure = result.TryGetFailure(out _);
        
        hasFailure.ShouldBeFalse();
    }

    [Fact]
    public void TryGetCancelled_WhenCancelled_ReturnsTrue() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        var isCancelled = result.TryGetCancelled(out var value);
        
        isCancelled.ShouldBeTrue();
        value.ShouldBe(true);
    }

    [Fact]
    public void TryGetCancelled_WhenSuccess_ReturnsFalse() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        var isCancelled = result.TryGetCancelled(out _);
        
        isCancelled.ShouldBeFalse();
    }

    [Fact]
    public void TryGetCancelled_WhenFailure_ReturnsFalse() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        var isCancelled = result.TryGetCancelled(out _);
        
        isCancelled.ShouldBeFalse();
    }

    [Fact]
    public void GetSuccessOrDefault_WhenSuccess_ReturnsValue() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        result.GetSuccessOrDefault().ShouldBe(42);
    }

    [Fact]
    public void GetSuccessOrDefault_WhenFailure_ReturnsDefault() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        result.GetSuccessOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetSuccessOrDefault_WhenCancelled_ReturnsDefault() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        result.GetSuccessOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetSuccessOrDefault_WithDefaultValue_WhenSuccess_ReturnsValue() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        result.GetSuccessOrDefault(100).ShouldBe(42);
    }

    [Fact]
    public void GetSuccessOrDefault_WithDefaultValue_WhenFailure_ReturnsDefaultValue() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        result.GetSuccessOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void GetSuccessOrDefault_WithDefaultValue_WhenCancelled_ReturnsDefaultValue() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        result.GetSuccessOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        var output = result.Match(
            value => value * 2,
            _ => 0,
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        var output = result.Match(
            value => value * 2,
            error => error.Length,
            _ => 0
        );
        
        output.ShouldBe(5);
    }

    [Fact]
    public void Match_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        var output = result.Match(
            value => value * 2,
            _ => 0,
            cancelled => cancelled ? 1 : 0
        );
        
        output.ShouldBe(1);
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state,
            (state, _) => state
        );
        
        output.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, error) => state + error.Length,
            (state, _) => state
        );
        
        output.ShouldBe(15);
    }

    [Fact]
    public void Match_WithState_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state,
            (state, cancelled) => state + (cancelled ? 1 : 0)
        );
        
        output.ShouldBe(11);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int, string, bool>.Success(42);
        var successCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => { },
            _ => { }
        );
        
        successCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int, string, bool>.Failure("error");
        var failureCalled = false;
        
        result.Switch(
            _ => { },
            _ => failureCalled = true,
            _ => { }
        );
        
        failureCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var cancelledCalled = false;
        
        result.Switch(
            _ => { },
            _ => { },
            _ => cancelledCalled = true
        );
        
        cancelledCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int, string, bool>.Success(42);
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int, string, bool>.Failure("error");
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, error) => counter = state + error.Length,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(105);
    }

    [Fact]
    public void Switch_WithState_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state,
            (state, cancelled) => counter = state + (cancelled ? 1 : 0)
        );
        
        counter.ShouldBe(101);
    }

    [Fact]
    public void ToString_WhenSuccess_ReturnsFormattedString() {
        var result = OperationResult<int, string, bool>.Success(42);
        
        result.ToString().ShouldBe("Success: 42");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFormattedString() {
        var result = OperationResult<int, string, bool>.Failure("error");
        
        result.ToString().ShouldBe("Failure: error");
    }

    [Fact]
    public void ToString_WhenCancelled_ReturnsFormattedString() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        
        result.ToString().ShouldBe("Cancelled: True");
    }

    [Fact]
    public void Success_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var result = OperationResult<List<int>, string, bool>.Success(list);
        
        result.SuccessValue.ShouldBeSameAs(list);
    }

    [Fact]
    public void Failure_WithReferenceType_StoresReference() {
        var list = new List<string> { "a", "b", "c" };
        var result = OperationResult<int, List<string>, bool>.Failure(list);
        
        result.FailureValue.ShouldBeSameAs(list);
    }

    [Fact]
    public void Cancelled_WithReferenceType_StoresReference() {
        var list = new List<bool> { true, false };
        var result = OperationResult<int, string, List<bool>>.Cancelled(list);
        
        result.CancelledValue.ShouldBeSameAs(list);
    }
}

