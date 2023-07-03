using System;
using System.IO;

namespace fin.util.binary {
  public static class EndianBinaryReaderUtil {
    public static T SubreadReturn<T>(this IEndianBinaryReader er,
                                     long offset,
                                     Func<IEndianBinaryReader, T> handler) {
      T value = default;
      er.Subread(
          offset,
          ser => { value = handler(ser); });

      return value!;
    }
  }
}