using System;

using schema.binary;

namespace fin.util.binary {
  public static class EndianBinaryReaderUtil {
    public static T SubreadReturn<T>(this IEndianBinaryReader er,
                                     long offset,
                                     Func<IEndianBinaryReader, T> handler) {
      T value = default;
      er.SubreadAt(
          offset,
          ser => { value = handler(ser); });

      return value!;
    }
  }
}