using System.Runtime.CompilerServices;

namespace HCommons.Math;

public static class HMathExtensions {
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ClampToFloat(this double value, float min = float.MinValue, float max = float.MaxValue) {
        return HMath.ClampToFloat(value, min, max);
    }
    
    public static int Map(this int value, int fromSource, int toSource, int fromTarget, int toTarget) {
        return HMath.Map(value, fromSource, toSource, fromTarget, toTarget);
    }
    
    public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget) {
        return HMath.Map(value, fromSource, toSource, fromTarget, toTarget);
    }
    
    public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget) {
        return HMath.Map(value, fromSource, toSource, fromTarget, toTarget);
    }
}