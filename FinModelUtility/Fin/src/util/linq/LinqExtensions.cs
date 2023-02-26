using System;
using System.Collections.Generic;
using System.Linq;

namespace fin.util.linq {
  public static class LinqExtensions {
    public static IEnumerable<TTo> CastTo<TFrom, TTo>(
        this IEnumerable<TFrom> enumerable) where TFrom : TTo
      => enumerable.Select(value => (TTo) value);

    public static IEnumerable<TTo> WhereIs<TFrom, TTo>(
        this IEnumerable<TFrom> enumerable) where TTo : TFrom
      => enumerable.Where(value => value is TTo)
                   .Select(value => (TTo) value!);

    public static bool TryGetFirst<T>(this IEnumerable<T> enumerable,
                                      out T first) {
      try {
        first = enumerable.First();
        return true;
      } catch { }

      first = default;
      return false;
    }

    public static bool TryGetFirst<T>(this IEnumerable<T> enumerable,
                                      Func<T, bool> predicate,
                                      out T first) {
      try {
        first = enumerable.First(predicate);
        return true;
      } catch { }

      first = default;
      return false;
    }
  }
}