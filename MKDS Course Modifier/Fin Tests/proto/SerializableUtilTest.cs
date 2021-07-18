using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.proto {
  [TestClass]
  public class SerializableUtilTest {
    [StructLayout(LayoutKind.Sequential)]
    private class Int32Type {
      public Int32 a;
    }

    [StructLayout(LayoutKind.Sequential)]
    private class Int64Type {
      public Int64 a;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private class Int32And64Type {
      public Int32 a;
      public Int64 b;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private class PackedInt32And64Type {
      [FieldOffset(0)]
      public Int32 a;

      [FieldOffset(5)]
      public Int64 b;
    }


    [TestMethod]
    public void TestGetSizeOf() {
      Expect.AreEqual(4, SerializableUtil.GetSizeOf(typeof(Int32Type)));
      Expect.AreEqual(8, SerializableUtil.GetSizeOf(typeof(Int64Type)));
      Expect.AreEqual(12, SerializableUtil.GetSizeOf(typeof(Int32And64Type)));
      Expect.AreEqual(13, SerializableUtil.GetSizeOf(typeof(PackedInt32And64Type)));
    }
  }
}