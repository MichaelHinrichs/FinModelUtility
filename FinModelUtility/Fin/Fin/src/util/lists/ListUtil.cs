using System;
using System.Collections.Generic;

namespace fin.util.lists {
  public static class ListUtil {
    public static bool TryFindFirst<T>(IReadOnlyList<T> data, T value, out int index) {
      for (var i = 0; i < data.Count; i++) {
        var dataValue = data[i];
        if (dataValue?.Equals(value) ?? (dataValue == null && value == null)) {
          index = i;
          return true;
        }
      }
      index = -1;
      return false;
    }

    public static int AssertFindFirst<T>(IReadOnlyList<T> data, T value) {
      if (ListUtil.TryFindFirst(data, value, out var index)) {
        return index;
      }
      throw new Exception($"Failed to find value \"{value}\" in array.");
    }
  }
}
