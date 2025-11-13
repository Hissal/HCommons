using System.Collections;
using JetBrains.Annotations;

namespace HCommons.Internal;

internal static class EnumerableExtensions {
#if !NET6_0_OR_GREATER
    public static bool TryGetNonEnumeratedCount<TSource>([NoEnumeration] this IEnumerable<TSource> source, out int count) {
        switch (source) {
            case ICollection<TSource> genericCollection:
                count = genericCollection.Count;
                return true;
            case IReadOnlyCollection<TSource> readonlyCollection:
                count = readonlyCollection.Count;
                return true;
            case ICollection collection:
                count = collection.Count;
                return true;
            default:
                count = 0;
                return false;
        }
    }
#endif
}