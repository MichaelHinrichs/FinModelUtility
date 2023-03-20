using System.Collections.Generic;
using System.Linq;


namespace fin.util.enumerables {
  public static class EnumerableExtensions {
    public static int IndexOfOrNegativeOne<T>(
        this IEnumerable<T> enumerable,
        T value) {
      var index = 0;
      foreach (var item in enumerable) {
        if (value?.Equals(item) ?? (value == null && item == null)) {
          return index;
        }

        index++;
      }

      return -1;
    }

    public static IEnumerable<T> ConcatIfNonnull<T>(
        this IEnumerable<T> enumerable,
        IEnumerable<T>? other)
      => other == null ? enumerable : enumerable.Concat(other);

    public static IEnumerable<T> Yield<T>(this T item) {
      yield return item;
    }
  }
}