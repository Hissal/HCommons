using System.Collections;
using JetBrains.Annotations;

// Minimal definition to trigger IDE support without the NuGet package
namespace JetBrains.Annotations {
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NoEnumerationAttribute : Attribute;
}

namespace HCommons.Buffers {
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
}