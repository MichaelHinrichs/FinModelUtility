using System;
using System.Collections.Generic;

namespace uni.util.data {
  public static class ListUtil {
    public static bool RemoveFromFirstIfNotInSecond<T>(
        IList<T> first,
        IList<T> second)
      => ListUtil.RemoveWhere(first,
                              tFromFirst => !second.Contains(tFromFirst));

    public static bool RemoveWhere<T>(IList<T> impl, Func<T, bool> removeFunc) {
      var indices = new LinkedList<int>();
      for (var i = 0; i < impl.Count; ++i) {
        if (removeFunc(impl[i])) {
          indices.AddFirst(i);
        }
      }

      foreach (var i in indices) {
        impl.RemoveAt(i);
      }

      return indices.Count > 0;
    }
  }
}