using System.Text.RegularExpressions;

namespace System.IO {
  public interface ITextReader : IDataReader {
    bool Matches(out string text, params string[] matches);

    string ReadUpTo(params string[] matches);
    string ReadWhile(params string[] matches);

    byte[] ReadBytes(string[] separators, string[] terminators);
    void AssertHexByte(byte expectedValue);
    byte ReadHexByte();
    byte[] ReadHexBytes(string[] separators, string[] terminators);

    sbyte[] ReadSBytes(string[] separators, string[] terminators);
    void AssertHexSByte(sbyte expectedValue);
    sbyte ReadHexSByte();
    sbyte[] ReadHexSBytes(string[] separators, string[] terminators);

    short[] ReadInt16s(string[] separators, string[] terminators);
    void AssertHexInt16(short expectedValue);
    short ReadHexInt16();
    short[] ReadHexInt16s(string[] separators, string[] terminators);

    ushort[] ReadUInt16s(string[] separators, string[] terminators);
    void AssertHexUInt16(ushort expectedValue);
    ushort ReadHexUInt16();
    ushort[] ReadHexUInt16s(string[] separators, string[] terminators);

    int[] ReadInt32s(string[] separators, string[] terminators);
    void AssertHexInt32(int expectedValue);
    int ReadHexInt32();
    int[] ReadHexInt32s(string[] separators, string[] terminators);

    uint[] ReadUInt32s(string[] separators, string[] terminators);
    void AssertHexUInt32(uint expectedValue);
    uint ReadHexUInt32();
    uint[] ReadHexUInt32s(string[] separators, string[] terminators);

    long[] ReadInt64s(string[] separators, string[] terminators);
    void AssertHexInt64(long expectedValue);
    long ReadHexInt64();
    long[] ReadHexInt64s(string[] separators, string[] terminators);

    ulong[] ReadUInt64s(string[] separators, string[] terminators);
    void AssertHexUInt64(ulong expectedValue);
    ulong ReadHexUInt64();
    ulong[] ReadHexUInt64s(string[] separators, string[] terminators);

    float[] ReadSingles(string[] separators, string[] terminators);
    double[] ReadDoubles(string[] separators, string[] terminators);
  }
}