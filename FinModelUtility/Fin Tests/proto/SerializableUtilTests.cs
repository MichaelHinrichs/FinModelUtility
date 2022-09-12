using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.proto {
  [TestClass]
  public class SerializableUtilTests {
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
      [FieldOffset(0)] public Int32 a;

      [FieldOffset(5)] public Int64 b;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private class ByteType {
      public Byte a;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private class ByteArrayType {
      public IEnumerable<byte> a;
    }


    [TestMethod]
    public void TestGetSizeOf() {
      Expect.AreEqual(4, SerializableUtil.GetSizeOf(typeof(Int32Type)));
      Expect.AreEqual(8, SerializableUtil.GetSizeOf(typeof(Int64Type)));
      Expect.AreEqual(12, SerializableUtil.GetSizeOf(typeof(Int32And64Type)));
      Expect.AreEqual(13,
                      SerializableUtil.GetSizeOf(typeof(PackedInt32And64Type)));
      Expect.AreEqual(1, SerializableUtil.GetSizeOf(typeof(ByteType)));
    }

    private struct ByteStruct {
      public byte a;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private struct BytesStruct {
      [FieldOffset(0)]
      public byte a;

      [FieldOffset(2)]
      public byte b;
    }

    [TestMethod]
    public void TestSerializeClass() {
      var obj = new ByteType {a = 7};
      var bytes = SerializableUtil.Serialize(obj);
      Expect.AreArraysEqual(new byte[] {7}, bytes);
    }

    [TestMethod]
    public void TestSerializeSingletonStruct() {
      var byteStruct = new ByteStruct {a = 7};
      Expect.AreArraysEqual(new byte[] {7},
                            SerializableUtil.Serialize(byteStruct));
    }

    [TestMethod]
    public void TestSerializeMultiStruct() {
      var byteStruct = new BytesStruct { a = 7, b = 3 };
      Expect.AreArraysEqual(new byte[] { 7, 0, 3},
                            SerializableUtil.Serialize(byteStruct));
    }


    [TestMethod]
    public void TestDeserializeClass() {
      var bytes = new byte[] { 7 };
      var byteObj = SerializableUtil.Deserialize<ByteType>(bytes);
      Expect.AreEqual(7, byteObj.a);
    }

    [TestMethod]
    public void TestDeserializeSingletonStruct() {
      var bytes = new byte[] { 7 };
      var byteObj = SerializableUtil.Deserialize<ByteStruct>(bytes);
      Expect.AreEqual(7, byteObj.a);
    }


    [TestMethod]
    public void TestRead() {
      var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

      var stream = new BytesViewStream();
      stream.SetImpl(bytes, 1, 9);

      var readBytes = new byte[10];
      var readCount = stream.Read(readBytes, 1, 9);

      Expect.AreEqual(7, readCount);
      Expect.AreArraysEqual(new byte[] {2, 3, 4, 5, 6, 7, 8, 0, 0, 0},
                            readBytes);
    }
  }
}