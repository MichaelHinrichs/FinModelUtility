using System.Globalization;
using System.Linq;

using schema.binary.util;

namespace System.IO {
  public sealed partial class FinTextReader {
    public void AssertHexByte(byte expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexByte());

    public byte ReadHexByte()
      => byte.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    public void AssertHexSByte(sbyte expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexSByte());

    public sbyte ReadHexSByte()
      => sbyte.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    public void AssertHexInt16(short expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexInt16());

    public short ReadHexInt16()
      => short.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);

    public void AssertHexUInt16(ushort expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexUInt16());

    public ushort ReadHexUInt16()
      => ushort.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    public void AssertHexInt32(int expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexInt32());

    public int ReadHexInt32()
      => int.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    public void AssertHexUInt32(uint expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexUInt32());

    public uint ReadHexUInt32()
      => uint.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    public void AssertHexInt64(long expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexInt64());

    public long ReadHexInt64()
      => long.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    public void AssertHexUInt64(ulong expectedValue)
      => Asserts.Equal(expectedValue, this.ReadHexUInt64());

    public ulong ReadHexUInt64()
      => ulong.Parse(this.ReadHexChars_(), NumberStyles.HexNumber);


    private static readonly string[] hexMatches =
        digitMatches_
            .Concat(
                new[] { "a", "b", "c", "d", "e", "f" }.SelectMany(
                    c => new[] { c.ToLower(), c.ToUpper() }))
            .ToArray();

    private string ReadHexChars_() => this.ReadWhile(FinTextReader.hexMatches);
  }
}