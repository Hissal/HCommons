using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(Disposable))]
public class DisposableTest {
    [Fact]
    public void DisposableEmpty_ReturnsSharedInstance() {
        var d1 = Disposable.Empty;
        var d2 = Disposable.Empty;
        d1.ShouldBe(d2);
    }
    
    [Fact]
    public void ActionDisposable_Dispose_InvokesAction() {
        var invoked = false;
        var sut = Disposable.Action(() => invoked = true);
        sut.Dispose();
        invoked.ShouldBeTrue();
    }

    [Fact]
    public void ActionDisposableWithState_Dispose_InvokesAction() {
        var invoked = false;
        var sut = Disposable.Action(this, t => invoked = true);
        sut.Dispose();
        invoked.ShouldBeTrue();
    }
    
    [Fact]
    public void BooleanDisposable_IsDisposedReflectsState() {
        var sut = Disposable.Boolean();
        sut.IsDisposed.ShouldBeFalse();
        sut.Dispose();
        sut.IsDisposed.ShouldBeTrue();
    }
}