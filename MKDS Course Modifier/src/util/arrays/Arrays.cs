using System.Linq;

namespace mkds.util.array {
  public static class Arrays {
    public static T[] Concat<T>(params T[][] arrays) {
      var totalSize = arrays.Select(array => array.Length)
                            .Aggregate((lhs, rhs) => lhs + rhs);

      var totalArray = new T[totalSize];
      var i = 0;
      foreach (var array in arrays) {
        foreach (var t in array) {
          totalArray[i++] = t;
        }
      }

      return totalArray;
    }
  }
}