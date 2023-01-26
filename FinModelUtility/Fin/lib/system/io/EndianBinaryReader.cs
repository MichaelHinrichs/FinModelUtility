// Decompiled with JetBrains decompiler
// Type: System.IO.EndianBinaryReader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Text;
using schema;


namespace System.IO {
  public sealed partial class EndianBinaryReader : IEndianBinaryReader {
    private bool disposed_;

    // TODO: This should be private.
    // TODO: Does caching the buffer actually help, or can this logic be pulled into the extensions?
    private EndianBinaryBufferedStream BufferedStream_ { get; set; }
    private Stream BaseStream_ => this.BufferedStream_.BaseStream;

    public EndianBinaryReader(byte[] data)
      => this.Init_(new MemoryStream(data), null);

    public EndianBinaryReader(byte[] data, Endianness endianness)
      => this.Init_(new MemoryStream(data), endianness);

    public EndianBinaryReader(Stream baseStream)
      => this.Init_(baseStream, null);

    public EndianBinaryReader(Stream baseStream, Endianness endianness)
      => this.Init_(baseStream, endianness);

    private void Init_(Stream baseStream, Endianness? endianness) {
      if (baseStream == null) {
        throw new ArgumentNullException(nameof(baseStream));
      }
      if (!baseStream.CanRead) {
        throw new ArgumentException(nameof(baseStream));
      }

      this.BufferedStream_ = new EndianBinaryBufferedStream(endianness) {
        BaseStream = baseStream,
      };
    }

    ~EndianBinaryReader() {
      this.Dispose(false);
    }

    public void Close() => Dispose();

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize((object)this);
    }

    private void Dispose(bool disposing) {
      if (this.disposed_) {
        return;
      }
      if (disposing && this.BaseStream_ != null) {
        this.BaseStream_.Close();
      }
      this.disposed_ = true;
    }

    public long Position {
      get => this.BaseStream_.Position;
      set => this.BaseStream_.Position = value;
    }

    public void AssertPosition(long expectedPosition) {
      EndianBinaryReader.Assert(expectedPosition, this.Position);
    }

    public long Length => this.BaseStream_.Length;

    public bool Eof => this.Position >= this.Length;

    public void AssertNotEof() {
      if (this.Eof) {
        throw new Exception(
            $"Attempted to read past the end of the stream: position '{this.Position}' of stream length '{this.Length}'");
      }
    }

    public void Align(uint amt) {
      var offs = amt - (this.Position % amt);
      if (offs != amt) {
        this.Position += offs;
      }
    }

    public byte[] ReadBytesAtOffset(long position, int len) {
      var startingOffset = this.Position;
      this.Position = position;

      var bytes = this.ReadBytes(len);

      this.Position = startingOffset;

      return bytes;
    }

    public string ReadStringAtOffset(long position, int len) {
      var startingOffset = this.Position;
      this.Position = position;

      var str = this.ReadString(len);

      this.Position = startingOffset;

      return str;
    }

    public void Subread(long position,
                        int len,
                        Action<IEndianBinaryReader> subread) {
      var tempPos = this.Position;
      {
        this.Position = position;

        var bytes = this.ReadBytesAtOffset(position, len);
        using var ser =
            new EndianBinaryReader(new MemoryStream(bytes), this.Endianness);
        subread(ser);
      }
      this.Position = tempPos;
    }

    public void Subread(long position, Action<IEndianBinaryReader> subread) {
      var tempPos = this.Position;
      {
        this.Position = position;
        subread(this);
      }
      this.Position = tempPos;
    }


    private static void Assert<T>(T expectedValue, T actualValue) {
      if (!expectedValue.Equals(actualValue)) {
        throw new Exception(
            "Expected " + actualValue + " to be " + expectedValue);
      }
    }

    private void FillBuffer_(long count, int? optStride = null) {
      this.AssertNotEof();
      this.BufferedStream_.FillBuffer(count, optStride);
    }

    private void FillBuffer_(Span<byte> buffer) {
      this.AssertNotEof();
      this.BufferedStream_.FillBuffer(buffer);
    }


    public void AssertByte(byte expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadByte());

    public byte ReadByte() {
      this.AssertNotEof();
      return (byte) this.BufferedStream_.BaseStream.ReadByte();
    }

    public byte[] ReadBytes(long count) => this.ReadBytes(new byte[count]);

    public byte[] ReadBytes(byte[] dst) {
      this.BufferedStream_.BaseStream.Read(dst, 0, dst.Length);
      return dst;
    }


    public void AssertSByte(sbyte expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadSByte());

    public sbyte ReadSByte() => (sbyte) this.ReadByte();

    public sbyte[] ReadSBytes(long count) => this.ReadSBytes(new sbyte[count]);

    public sbyte[] ReadSBytes(sbyte[] dst) {
      this.BufferedStream_.FillBuffer(dst, dst.Length);
      return dst;
    }


    public void AssertInt16(short expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt16());

    public short ReadInt16() => this.BufferedStream_.Read<short>();

    public short[] ReadInt16s(long count) => this.ReadInt16s(new short[count]);

    public short[] ReadInt16s(short[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertUInt16(ushort expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt16());

    public ushort ReadUInt16() => this.BufferedStream_.Read<ushort>();

    public ushort[] ReadUInt16s(long count)
      => this.ReadUInt16s(new ushort[count]);

    public ushort[] ReadUInt16s(ushort[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertInt24(int expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt24());

    public int ReadInt24() {
      this.FillBuffer_(3);
      return EndianBinaryReader.ConvertInt24_(this.BufferedStream_.Buffer, 0);
    }

    public int[] ReadInt24s(long count) => this.ReadInt24s(new int[count]);

    public int[] ReadInt24s(int[] dst) {
      const int size = 3;
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertInt24_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUInt24(uint expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt24());

    public uint ReadUInt24() {
      this.FillBuffer_(3);
      return EndianBinaryReader.ConvertUInt24_(this.BufferedStream_.Buffer, 0);
    }

    public uint[] ReadUInt24s(long count) => this.ReadUInt24s(new uint[count]);

    public uint[] ReadUInt24s(uint[] dst) {
      const int size = 3;
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertUInt24_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertInt32(int expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt32());

    public int ReadInt32() => this.BufferedStream_.Read<int>();

    public int[] ReadInt32s(long count) => this.ReadInt32s(new int[count]);

    public int[] ReadInt32s(int[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertUInt32(uint expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt32());

    public uint ReadUInt32() => this.BufferedStream_.Read<uint>();

    public uint[] ReadUInt32s(long count) => this.ReadUInt32s(new uint[count]);

    public uint[] ReadUInt32s(uint[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertInt64(long expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt64());

    public long ReadInt64() => this.BufferedStream_.Read<long>();

    public long[] ReadInt64s(long count) => this.ReadInt64s(new long[count]);

    public long[] ReadInt64s(long[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertUInt64(ulong expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt64());

    public ulong ReadUInt64() => this.BufferedStream_.Read<ulong>();

    public ulong[] ReadUInt64s(long count) =>
        this.ReadUInt64s(new ulong[count]);

    public ulong[] ReadUInt64s(ulong[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertHalf(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadHalf());

    public float ReadHalf() {
      this.FillBuffer_(2);
      return EndianBinaryReader.ConvertHalf_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadHalfs(long count) => this.ReadHalfs(new float[count]);

    public float[] ReadHalfs(float[] dst) {
      const int size = 2;
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertHalf_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertSingle(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadSingle());

    public float ReadSingle() => this.BufferedStream_.Read<float>();

    public float[] ReadSingles(long count) =>
        this.ReadSingles(new float[count]);

    public float[] ReadSingles(float[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertDouble(double expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadDouble());

    public double ReadDouble() => this.BufferedStream_.Read<double>();

    public double[] ReadDoubles(long count)
      => this.ReadDoubles(new double[count]);

    public double[] ReadDoubles(double[] dst) {
      this.BufferedStream_.FillBufferAndReverse(dst, dst.Length);
      return dst;
    }


    public void AssertSn8(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadSn8());

    public float ReadSn8() {
      this.FillBuffer_(sizeof(byte));
      return EndianBinaryReader.ConvertSn8_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadSn8s(long count) => this.ReadSn8s(new float[count]);

    public float[] ReadSn8s(float[] dst) {
      const int size = sizeof(byte);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertSn8_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUn8(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUn8());

    public float ReadUn8() {
      this.FillBuffer_(sizeof(byte));
      return EndianBinaryReader.ConvertUn8_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadUn8s(long count) => this.ReadUn8s(new float[count]);

    public float[] ReadUn8s(float[] dst) {
      const int size = sizeof(byte);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertUn8_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertSn16(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadSn16());

    public float ReadSn16() {
      this.FillBuffer_(sizeof(short));
      return EndianBinaryReader.ConvertSn16_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadSn16s(long count) => this.ReadSn16s(new float[count]);

    public float[] ReadSn16s(float[] dst) {
      const int size = sizeof(short);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertSn16_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUn16(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUn16());

    public float ReadUn16() {
      this.FillBuffer_(sizeof(ushort));
      return EndianBinaryReader.ConvertUn16_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadUn16s(long count) => this.ReadUn16s(new float[count]);

    public float[] ReadUn16s(float[] dst) {
      const int size = sizeof(ushort);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertUn16_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }

    public void AssertChar(char expectedValue)
      => this.AssertChar(Encoding.ASCII, expectedValue);

    public char ReadChar() => this.ReadChar(Encoding.ASCII);

    public char[] ReadChars(long count) =>
        this.ReadChars(Encoding.ASCII, count);

    public char[] ReadChars(char[] dst) => this.ReadChars(Encoding.ASCII, dst);


    public void AssertChar(Encoding encoding, char expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadChar(encoding));

    public char ReadChar(Encoding encoding) {
      this.AssertNotEof();
      var encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.BufferedStream_.FillBuffer(encodingSize, encodingSize);
      return encoding.GetChars(this.BufferedStream_.Buffer, 0, encodingSize)[0];
    }

    public char[] ReadChars(Encoding encoding, long count)
      => this.ReadChars(encoding, new char[count]);

    public char[] ReadChars(Encoding encoding, char[] dst) {
      var encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.BufferedStream_.FillBuffer(encodingSize * dst.Length, encodingSize);
      encoding.GetChars(this.BufferedStream_.Buffer,
                        0,
                        encodingSize * dst.Length,
                        dst,
                        0);
      return dst;
    }

    private static int GetEncodingSize_(Encoding encoding) {
      return encoding == Encoding.UTF8 ||
             encoding == Encoding.ASCII ||
             encoding != Encoding.Unicode &&
             encoding != Encoding.BigEndianUnicode
                 ? 1
                 : 2;
    }


    public void AssertString(string expectedValue)
      => this.AssertString(Encoding.ASCII, expectedValue);

    public string ReadString(long count)
      => this.ReadString(Encoding.ASCII, count);


    public void AssertString(Encoding encoding, string expectedValue)
      => EndianBinaryReader.Assert(
          expectedValue.TrimEnd('\0'),
          this.ReadString(encoding, expectedValue.Length));

    public string ReadString(Encoding encoding, long count) {
      this.AssertNotEof();
      return new string(this.ReadChars(encoding, count)).TrimEnd('\0');
    }


    public void AssertStringNT(string expectedValue)
      => this.AssertStringNT(Encoding.ASCII, expectedValue);

    public string ReadStringNT() => this.ReadStringNT(Encoding.ASCII);


    public void AssertStringNT(Encoding encoding, string expectedValue)
      => EndianBinaryReader.Assert(
          expectedValue,
          this.ReadStringNT(encoding));

    public string ReadStringNT(Encoding encoding) {
      var strBuilder = new StringBuilder();
      while (true) {
        var c = this.ReadChar(encoding);
        if (c == '\0') {
          break;
        }

        strBuilder.Append(c);
      }
      return strBuilder.ToString();
    }

    public void AssertMagicText(string expectedText) {
      var actualText = this.ReadString(expectedText.Length);

      if (expectedText != actualText) {
        throw new Exception(
            $"Expected to find magic text \"{expectedText}\", but found \"{actualText}\"");
      }
    }

    public T ReadNew<T>() where T : IDeserializable, new() {
      this.AssertNotEof();
      var value = new T();
      value.Read(this);
      return value;
    }

    public bool TryReadNew<T>(out T? value) where T : IDeserializable, new() {
      var originalPosition = this.Position;
      try {
        value = this.ReadNew<T>();
        return true;
      } catch {
        this.Position = originalPosition;
        value = default;
        return false;
      }
    }

    public void ReadNewArray<T>(out T[] array, int length)
        where T : IDeserializable, new() {
      array = new T[length];
      for (var i = 0; i < length; ++i) {
        this.AssertNotEof();
        array[i] = this.ReadNew<T>();
      }
    }
  }
}