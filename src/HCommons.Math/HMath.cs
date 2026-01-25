using System.Runtime.CompilerServices;

namespace HCommons.Math;

/// <summary>
/// Provides common mathematical helper methods for clamping, mapping, and other operations.
/// </summary>
public static partial class HMath {
    /// <summary>
    /// Clamps a double value to the float range and converts it to a float.
    /// </summary>
    /// <param name="value">The double value to clamp and convert.</param>
    /// <param name="min">The minimum value. Defaults to <see cref="float.MinValue"/>.</param>
    /// <param name="max">The maximum value. Defaults to <see cref="float.MaxValue"/>.</param>
    /// <returns>The clamped value as a float.</returns>
    /// <remarks>
    /// If <paramref name="value"/> is <see cref="double.NaN"/>, returns <paramref name="min"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ClampToFloat(double value, float min = float.MinValue, float max = float.MaxValue) {
        if (double.IsNaN(value)) return min;
        if (value < min) return min;
        if (value > max) return max;
        return (float)value;
    }
    
    /// <summary>
    /// Maps an integer value from one range to another range.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromSource">The lower bound of the source range.</param>
    /// <param name="toSource">The upper bound of the source range.</param>
    /// <param name="fromTarget">The lower bound of the target range.</param>
    /// <param name="toTarget">The upper bound of the target range.</param>
    /// <returns>The mapped value in the target range.</returns>
    /// <remarks>
    /// If the source range is zero (fromSource equals toSource), returns fromTarget.
    /// Values outside the source range will be extrapolated.
    /// </remarks>
    public static int Map(int value, int fromSource, int toSource, int fromTarget, int toTarget) {
        var sourceRange = toSource - fromSource;
        if (sourceRange == 0) return fromTarget;
        return (int)(((long)(value - fromSource) * (toTarget - fromTarget)) / sourceRange) + fromTarget;
    }
    
    /// <summary>
    /// Maps a float value from one range to another range.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromSource">The lower bound of the source range.</param>
    /// <param name="toSource">The upper bound of the source range.</param>
    /// <param name="fromTarget">The lower bound of the target range.</param>
    /// <param name="toTarget">The upper bound of the target range.</param>
    /// <returns>The mapped value in the target range.</returns>
    /// <remarks>
    /// If the source range is zero (fromSource equals toSource), returns fromTarget.
    /// Values outside the source range will be extrapolated.
    /// Special floating-point values (NaN, Infinity, -Infinity) are preserved through the calculation.
    /// </remarks>
    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget) {
        var sourceRange = toSource - fromSource;
        if (System.Math.Abs(sourceRange) < float.Epsilon) return fromTarget;
        return (value - fromSource) / sourceRange * (toTarget - fromTarget) + fromTarget;
    }
    
    /// <summary>
    /// Maps a double value from one range to another range.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromSource">The lower bound of the source range.</param>
    /// <param name="toSource">The upper bound of the source range.</param>
    /// <param name="fromTarget">The lower bound of the target range.</param>
    /// <param name="toTarget">The upper bound of the target range.</param>
    /// <returns>The mapped value in the target range.</returns>
    /// <remarks>
    /// If the source range is zero (fromSource equals toSource), returns fromTarget.
    /// Values outside the source range will be extrapolated.
    /// Special floating-point values (NaN, Infinity, -Infinity) are preserved through the calculation.
    /// </remarks>
    public static double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget) {
        var sourceRange = toSource - fromSource;
        if (System.Math.Abs(sourceRange) < double.Epsilon) return fromTarget;
        return (value - fromSource) / sourceRange * (toTarget - fromTarget) + fromTarget;
    }
}