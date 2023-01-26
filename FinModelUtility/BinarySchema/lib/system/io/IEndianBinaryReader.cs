using schema;

using System.Text;

namespace System.IO {
  public interface IEndianBinaryReader : IEndiannessStack {
    long Position { get; set; }

    void AssertPosition(long expectedPosition);

    long Length { get; }
    bool Eof { get; }

    void AssertNotEof();

    void Align(uint amt);

    byte[] ReadBytesAtOffset(long position, int len);
    string ReadStringAtOffset(long position, int len);

    void Subread(long position,
                 int len,
                 Action<IEndianBinaryReader> subread);

    void Subread(long position, Action<IEndianBinaryReader> subread);

    void AssertByte(byte expectedValue);
    byte ReadByte();
    byte[] ReadBytes(long count);
    byte[] ReadBytes(byte[] dst);


    void AssertSByte(sbyte expectedValue);
    sbyte ReadSByte();
    sbyte[] ReadSBytes(long count);
    sbyte[] ReadSBytes(sbyte[] dst);

    void AssertInt16(short expectedValue);
    short ReadInt16();
    short[] ReadInt16s(long count);
    short[] ReadInt16s(short[] dst);


    void AssertUInt16(ushort expectedValue);
    ushort ReadUInt16();
    ushort[] ReadUInt16s(long count);
    ushort[] ReadUInt16s(ushort[] dst);

    void AssertInt24(int expectedValue);
    int ReadInt24();
    int[] ReadInt24s(long count);
    int[] ReadInt24s(int[] dst);

    void AssertUInt24(uint expectedValue);
    uint ReadUInt24();
    uint[] ReadUInt24s(long count);
    uint[] ReadUInt24s(uint[] dst);

    void AssertInt32(int expectedValue);
    int ReadInt32();
    int[] ReadInt32s(long count);
    int[] ReadInt32s(int[] dst);


    void AssertUInt32(uint expectedValue);
    uint ReadUInt32();
    uint[] ReadUInt32s(long count);
    uint[] ReadUInt32s(uint[] dst);
    
    void AssertInt64(long expectedValue);
    long ReadInt64();
    long[] ReadInt64s(long count);
    long[] ReadInt64s(long[] dst);

    void AssertUInt64(ulong expectedValue);
    ulong ReadUInt64();
    ulong[] ReadUInt64s(long count);
    ulong[] ReadUInt64s(ulong[] dst);


    void AssertHalf(float expectedValue);
    float ReadHalf();
    float[] ReadHalfs(long count);
    float[] ReadHalfs(float[] dst);

    void AssertSingle(float expectedValue);
    float ReadSingle();
    float[] ReadSingles(long count);
    float[] ReadSingles(float[] dst);

    void AssertDouble(double expectedValue);
    double ReadDouble();
    double[] ReadDoubles(long count);
    double[] ReadDoubles(double[] dst);

    void AssertSn8(float expectedValue);
    float ReadSn8();
    float[] ReadSn8s(long count);
    float[] ReadSn8s(float[] dst);

    void AssertUn8(float expectedValue);
    float ReadUn8();
    float[] ReadUn8s(long count);
    float[] ReadUn8s(float[] dst);

    void AssertSn16(float expectedValue);
    float ReadSn16();
    float[] ReadSn16s(long count);
    float[] ReadSn16s(float[] dst);

    void AssertUn16(float expectedValue);
    float ReadUn16();
    float[] ReadUn16s(long count);
    float[] ReadUn16s(float[] dst);

    void AssertChar(char expectedValue);
    char ReadChar();
    char[] ReadChars(long count);
    char[] ReadChars(char[] dst);

    void AssertChar(Encoding encoding, char expectedValue);
    char ReadChar(Encoding encoding);
    char[] ReadChars(Encoding encoding, long count);
    char[] ReadChars(Encoding encoding, char[] dst);

    void AssertString(string expectedValue);
    string ReadString(long count);

    void AssertString(Encoding encoding, string expectedValue);
    string ReadString(Encoding encoding, long count);

    void AssertStringNT(string expectedValue);
    string ReadStringNT();

    void AssertStringNT(Encoding encoding, string expectedValue);
    string ReadStringNT(Encoding encoding);

    void AssertMagicText(string expectedText);

    T ReadNew<T>() where T : IDeserializable, new();

    bool TryReadNew<T>(out T? value) where T : IDeserializable, new();

    void ReadNewArray<T>(out T[] array, int length)
        where T : IDeserializable, new();
  }
}