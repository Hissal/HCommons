using HCommons.Math;

namespace HCommons.Tests.Math;

[TestSubject(typeof(HMathExtensions))]
public class HMathExtensionsTest {

    [Theory]
    [InlineData(42.5, 42.5f)]
    [InlineData(123.456, 123.456f)]
    [InlineData(0.0, 0f)]
    [InlineData(-50.25, -50.25f)]
    public void ClampToFloat_WhenValueWithinRange_ReturnsValue(double value, float expected) {
        var result = value.ClampToFloat();
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(-1000.0, -100f, 100f, -100f)]
    [InlineData(-150.5, -100f, 100f, -100f)]
    public void ClampToFloat_WhenValueBelowMin_ReturnsMin(double value, float min, float max, float expected) {
        var result = value.ClampToFloat(min, max);
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(1000.0, -100f, 100f, 100f)]
    [InlineData(150.5, -100f, 100f, 100f)]
    public void ClampToFloat_WhenValueAboveMax_ReturnsMax(double value, float min, float max, float expected) {
        var result = value.ClampToFloat(min, max);
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(-100.0, -100f, 100f, -100f)]
    [InlineData(0.0, 0f, 100f, 0f)]
    public void ClampToFloat_WhenValueEqualsMin_ReturnsMin(double value, float min, float max, float expected) {
        var result = value.ClampToFloat(min, max);
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(100.0, -100f, 100f, 100f)]
    [InlineData(50.0, 0f, 50f, 50f)]
    public void ClampToFloat_WhenValueEqualsMax_ReturnsMax(double value, float min, float max, float expected) {
        var result = value.ClampToFloat(min, max);
        result.ShouldBe(expected);
    }

    [Fact]
    public void ClampToFloat_WhenValueIsNaN_ReturnsMin() {
        var result = double.NaN.ClampToFloat(-100f, 100f);
        result.ShouldBe(-100f);
    }

    [Fact]
    public void ClampToFloat_WhenValueIsPositiveInfinity_ReturnsMax() {
        var result = double.PositiveInfinity.ClampToFloat(-100f, 100f);
        result.ShouldBe(100f);
    }

    [Fact]
    public void ClampToFloat_WhenValueIsNegativeInfinity_ReturnsMin() {
        var result = double.NegativeInfinity.ClampToFloat(-100f, 100f);
        result.ShouldBe(-100f);
    }

    [Fact]
    public void ClampToFloat_WhenValueIsDoubleMaxValue_ReturnsFloatMaxValue() {
        var result = double.MaxValue.ClampToFloat();
        result.ShouldBe(float.MaxValue);
    }

    [Fact]
    public void ClampToFloat_WhenValueIsDoubleMinValue_ReturnsFloatMinValue() {
        var result = double.MinValue.ClampToFloat();
        result.ShouldBe(float.MinValue);
    }

    [Theory]
    [InlineData(50, 0, 100, 0, 10, 5)]
    [InlineData(0, 0, 100, 0, 10, 0)]
    [InlineData(100, 0, 100, 0, 10, 10)]
    [InlineData(50, 100, 0, 0, 10, 5)]
    [InlineData(50, 0, 100, 10, 0, 5)]
    [InlineData(-50, -100, 0, 0, 10, 5)]
    [InlineData(150, 0, 100, 0, 10, 15)]
    [InlineData(-50, 0, 100, 0, 10, -5)]
    [InlineData(75, 0, 100, 200, 300, 275)]
    [InlineData(25, 0, 100, -50, 50, -25)]
    public void Map_Int_MapsCorrectly(int value, int fromSource, int toSource, int fromTarget, int toTarget, int expected) {
        var result = value.Map(fromSource, toSource, fromTarget, toTarget);
        result.ShouldBe(expected);
    }

    [Fact]
    public void Map_Int_WhenSourceRangeIsZero_ReturnsTargetMin() {
        var result = 50.Map(100, 100, 0, 10);
        result.ShouldBe(0);
    }

    [Theory]
    [InlineData(50f, 0f, 100f, 0f, 10f, 5f)]
    [InlineData(0f, 0f, 100f, 0f, 10f, 0f)]
    [InlineData(100f, 0f, 100f, 0f, 10f, 10f)]
    [InlineData(50f, 100f, 0f, 0f, 10f, 5f)]
    [InlineData(50f, 0f, 100f, 10f, 0f, 5f)]
    [InlineData(-50f, -100f, 0f, 0f, 10f, 5f)]
    [InlineData(150f, 0f, 100f, 0f, 10f, 15f)]
    [InlineData(-50f, 0f, 100f, 0f, 10f, -5f)]
    [InlineData(75f, 0f, 100f, 200f, 300f, 275f)]
    [InlineData(0.5f, 0f, 1f, 0f, 100f, 50f)]
    [InlineData(0.25f, 0f, 1f, 0f, 100f, 25f)]
    public void Map_Float_MapsCorrectly(float value, float fromSource, float toSource, float fromTarget, float toTarget, float expected) {
        var result = value.Map(fromSource, toSource, fromTarget, toTarget);
        result.ShouldBe(expected);
    }

    [Fact]
    public void Map_Float_WhenSourceRangeIsZero_ReturnsTargetMin() {
        var result = 50f.Map(100f, 100f, 0f, 10f);
        result.ShouldBe(0f);
    }

    [Theory]
    [InlineData(50.0, 0.0, 100.0, 0.0, 10.0, 5.0)]
    [InlineData(0.0, 0.0, 100.0, 0.0, 10.0, 0.0)]
    [InlineData(100.0, 0.0, 100.0, 0.0, 10.0, 10.0)]
    [InlineData(50.0, 100.0, 0.0, 0.0, 10.0, 5.0)]
    [InlineData(50.0, 0.0, 100.0, 10.0, 0.0, 5.0)]
    [InlineData(-50.0, -100.0, 0.0, 0.0, 10.0, 5.0)]
    [InlineData(150.0, 0.0, 100.0, 0.0, 10.0, 15.0)]
    [InlineData(-50.0, 0.0, 100.0, 0.0, 10.0, -5.0)]
    [InlineData(75.0, 0.0, 100.0, 200.0, 300.0, 275.0)]
    [InlineData(0.5, 0.0, 1.0, 0.0, 100.0, 50.0)]
    [InlineData(0.25, 0.0, 1.0, 0.0, 100.0, 25.0)]
    public void Map_Double_MapsCorrectly(double value, double fromSource, double toSource, double fromTarget, double toTarget, double expected) {
        var result = value.Map(fromSource, toSource, fromTarget, toTarget);
        result.ShouldBe(expected);
    }

    [Fact]
    public void Map_Double_WhenSourceRangeIsZero_ReturnsTargetMin() {
        var result = 50.0.Map(100.0, 100.0, 0.0, 10.0);
        result.ShouldBe(0.0);
    }
}

