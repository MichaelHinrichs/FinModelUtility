using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace fin.proto {
  public static class FinMarshal {
    // TODO: Optimize this so it doesn't have to allocate for each object.
    public static T Deserialize<T>(Stream stream, int offset = 0) {
      var size = Marshal.SizeOf(typeof(T));

      var bytes = new byte[size];
      stream.Read(bytes, offset, size);

      return Deserialize<T>(bytes, offset);
    }

    // TODO: Rewrite this w/ a data source interface instead.
    public static T Deserialize<T>(
        IList<byte> byteList,
        int offset = 0,
        bool flipEndian = false) {
      var size = Marshal.SizeOf(typeof(T));

      var bytes = new byte[size];
      for (var i = 0; i < size; ++i) {
        bytes[i] = byteList[offset + i];
      }

      if (flipEndian) {
        FlipStructureInPlace(typeof(T), bytes);
      }

      // TODO: Use the list directly.
      return Deserialize<T>(bytes);
    }

    public static T Deserialize<T>(
        byte[] bytes,
        int offset = 0) {
      var size = Marshal.SizeOf(typeof(T));

      unsafe {
        fixed (byte* p = bytes) {
          var ptr = (IntPtr) p + offset;
          return Marshal.PtrToStructure<T>(ptr);
        }
      }
    }


    // TODO: Optimize this so it doesn't have to allocate for each object.
    public static void Serialize(object obj, Stream stream, int offset = 0) {
      var size = Marshal.SizeOf(obj);

      var bytes = new byte[size];
      Serialize(obj, bytes, offset);

      stream.Write(bytes, offset, size);
    }

    // TODO: Optimize this so it doesn't have to allocate for each object.
    public static void Serialize(
        object obj,
        IList<byte> byteList,
        int offset = 0) {
      var size = Marshal.SizeOf(obj);

      var bytes = new byte[size];
      Serialize(obj, bytes);

      for (var i = 0; i < size; ++i) {
        byteList[offset + i] = bytes[i];
      }
    }

    // TODO: Optimize this so it doesn't have to allocate for each object.
    public static void Serialize(object obj, byte[] bytes, int offset = 0) {
      var size = Marshal.SizeOf(obj);

      var ptr = Marshal.AllocHGlobal(size);
      Marshal.StructureToPtr(obj, ptr, false);

      Marshal.Copy(ptr, bytes, offset, size);
      Marshal.FreeHGlobal(ptr);
    }

    // https://stackoverflow.com/questions/2480116/marshalling-a-big-endian-byte-collection-into-a-struct-in-order-to-pull-out-valu
    public static void FlipStructureInPlace(
        Type type,
        byte[] bytes,
        int offset = 0) {
      var size = Marshal.SizeOf(type);

      if (type.IsPrimitive) {
        Array.Reverse(bytes, offset, size);
        return;
      }

      var totalOffset = 0;
      foreach (var field in type.GetFields()) {
        FlipStructureInPlace(field.FieldType, bytes, totalOffset);
        totalOffset += Marshal.SizeOf(field.FieldType);
      }
    }
  }
}