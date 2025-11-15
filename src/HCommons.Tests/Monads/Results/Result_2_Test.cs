using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Result<,>))]
public class Result_2_Test {

    [Fact]
    public void Success_CreatesSuccessResult() {
        var result = Result<int, string>.Success(42);
        
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Success_StoresValue() {
        var result = Result<int, string>.Success(42);
        
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Failure_CreatesFailureResult() {
        var result = Result<int, string>.Failure("error");
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Failure_StoresFailureValue() {
        var result = Result<int, string>.Failure("error");
        
        result.FailureValue.ShouldBe("error");
    }

    [Fact]
    public void ImplicitConversion_FromSuccessValue_CreatesSuccess() {
        Result<int, string> result = 42;
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void ImplicitConversion_FromFailureValue_CreatesFailure() {
        Result<int, string> result = "error";
        
        result.IsFailure.ShouldBeTrue();
        result.FailureValue.ShouldBe("error");
    }

    [Fact]
    public void TryGetSuccess_WhenSuccess_ReturnsTrue() {
        var result = Result<int, string>.Success(42);
        
        var success = result.TryGetSuccess(out var value);
        
        success.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetSuccess_WhenFailure_ReturnsFalse() {
        var result = Result<int, string>.Failure("error");
        
        var success = result.TryGetSuccess(out _);
        
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryGetFailure_WhenFailure_ReturnsTrue() {
        var result = Result<int, string>.Failure("error");
        
        var hasFailure = result.TryGetFailure(out var failureValue);
        
        hasFailure.ShouldBeTrue();
        failureValue.ShouldBe("error");
    }

    [Fact]
    public void TryGetFailure_WhenSuccess_ReturnsFalse() {
        var result = Result<int, string>.Success(42);
        
        var hasFailure = result.TryGetFailure(out _);
        
        hasFailure.ShouldBeFalse();
    }

    [Fact]
    public void GetSuccessOrDefault_WhenSuccess_ReturnsValue() {
        var result = Result<int, string>.Success(42);
        
        result.GetSuccessOrDefault().ShouldBe(42);
    }

    [Fact]
    public void GetSuccessOrDefault_WhenFailure_ReturnsDefault() {
        var result = Result<int, string>.Failure("error");
        
        result.GetSuccessOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetSuccessOrDefault_WithDefaultValue_WhenSuccess_ReturnsValue() {
        var result = Result<int, string>.Success(42);
        
        result.GetSuccessOrDefault(100).ShouldBe(42);
    }

    [Fact]
    public void GetSuccessOrDefault_WithDefaultValue_WhenFailure_ReturnsDefaultValue() {
        var result = Result<int, string>.Failure("error");
        
        result.GetSuccessOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int, string>.Success(42);
        
        var output = result.Match(
            value => value * 2,
            error => error.Length
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = Result<int, string>.Failure("error");
        
        var output = result.Match(
            value => value * 2,
            error => error.Length
        );
        
        output.ShouldBe(5);
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int, string>.Success(42);
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state
        );
        
        output.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result<int, string>.Failure("error");
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, error) => state + error.Length
        );
        
        output.ShouldBe(15);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int, string>.Success(42);
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeTrue();
        failureCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = Result<int, string>.Failure("error");
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int, string>.Success(42);
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result<int, string>.Failure("error");
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, error) => counter = state + error.Length
        );
        
        counter.ShouldBe(105);
    }

    [Fact]
    public void ToString_WhenSuccess_ReturnsFormattedString() {
        var result = Result<int, string>.Success(42);
        
        result.ToString().ShouldBe("Success: 42");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFormattedString() {
        var result = Result<int, string>.Failure("error");
        
        result.ToString().ShouldBe("Failure: error");
    }

    [Fact]
    public void Success_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var result = Result<List<int>, string>.Success(list);
        
        result.Value.ShouldBeSameAs(list);
    }

    [Fact]
    public void Failure_WithReferenceType_StoresReference() {
        var list = new List<string> { "a", "b", "c" };
        var result = Result<int, List<string>>.Failure(list);
        
        result.FailureValue.ShouldBeSameAs(list);
    }
}

