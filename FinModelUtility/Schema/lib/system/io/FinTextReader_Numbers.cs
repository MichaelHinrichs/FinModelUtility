using System.Linq;

using schema.binary.util;

namespace System.IO {
  public sealed partial class FinTextReader {
    public void AssertByte(byte expectedValue)
      => Asserts.Equal(expectedValue, this.ReadByte());

    public byte ReadByte() => byte.Parse(this.ReadIntegerChars_());


    public void AssertSByte(sbyte expectedValue)
      => Asserts.Equal(expectedValue, this.ReadSByte());

    public sbyte ReadSByte() => sbyte.Parse(this.ReadIntegerChars_());


    public void AssertInt16(short expectedValue)
      => Asserts.Equal(expectedValue, this.ReadInt16());

    public short ReadInt16() => short.Parse(this.ReadIntegerChars_());

    public void AssertUInt16(ushort expectedValue)
      => Asserts.Equal(expectedValue, this.ReadUInt16());


    public ushort ReadUInt16() => ushort.Parse(this.ReadIntegerChars_());


    public void AssertInt32(int expectedValue)
      => Asserts.Equal(expectedValue, this.ReadInt32());

    public int ReadInt32() => int.Parse(this.ReadIntegerChars_());


    public void AssertUInt32(uint expectedValue)
      => Asserts.Equal(expectedValue, this.ReadUInt32());

    public uint ReadUInt32() => uint.Parse(this.ReadIntegerChars_());


    public void AssertInt64(long expectedValue)
      => Asserts.Equal(expectedValue, this.ReadInt64());

    public long ReadInt64() => long.Parse(this.ReadIntegerChars_());


    public void AssertUInt64(ulong expectedValue)
      => Asserts.Equal(expectedValue, this.ReadUInt64());

    public ulong ReadUInt64() => ulong.Parse(this.ReadIntegerChars_());


    public void AssertSingle(float expectedValue)
      => Asserts.Equal(expectedValue, this.ReadSingle());

    public float ReadSingle() => float.Parse(this.ReadFloatChars_());


    public void AssertDouble(double expectedValue)
      => Asserts.Equal(expectedValue, this.ReadDouble());

    public double ReadDouble() => double.Parse(this.ReadFloatChars_());


    private static readonly string[] integerMatches_ = {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
    };

    private static readonly string[] floatMatches_ =
        integerMatches_.Concat(new[] { "." }).ToArray();


    private string ReadIntegerChars_()
      => ReadWhile(FinTextReader.integerMatches_);

    private string ReadFloatChars_()
      => ReadWhile(FinTextReader.floatMatches_);
  }
}