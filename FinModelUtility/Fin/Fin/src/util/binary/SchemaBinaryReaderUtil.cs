using System;

using schema.binary;

namespace fin.util.binary {
  public static class SchemaBinaryReaderUtil {
    public static T SubreadReturn<T>(this IBinaryReader br,
                                     long offset,
                                     Func<IBinaryReader, T> handler) {
      T value = default;
      br.SubreadAt(
          offset,
          ser => { value = handler(ser); });

      return value!;
    }
  }
}