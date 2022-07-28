using System.Collections.Generic;


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
  }
}