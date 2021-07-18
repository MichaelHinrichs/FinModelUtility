using System;
using System.Runtime.InteropServices;

namespace fin.proto {
  public static class SerializableUtil {

    public static int GetSizeOf(Type type) => Marshal.SizeOf(type);
  }
}