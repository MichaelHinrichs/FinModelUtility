// Decompiled with JetBrains decompiler
// Type: System.IO.EndianBinaryReader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;
using System.Text;

using schema;


namespace System.IO {
  public sealed class EndianBinaryReader : IDisposable {
    private bool disposed_;

    // TODO: This should be private.
    // TODO: Does caching the buffer actually help, or can this logic be pulled into the extensions?
    private EndianBinaryBufferedStream BufferedStream_ { get; }
    public Stream BaseStream => this.BufferedStream_.BaseStream;

    public Endianness Endianness {
      get => this.BufferedStream_.Endianness;
      set => this.BufferedStream_.Endianness = value;
    }

    public EndianBinaryReader(Stream baseStream)
        : this(baseStream, Endianness.BigEndian) { }

    public EndianBinaryReader(Stream baseStream, Endianness endianness) {
      if (baseStream == null) {
        throw new ArgumentNullException(nameof(baseStream));
      }
      if (!baseStream.CanRead) {
        throw new ArgumentException(nameof(baseStream));
      }

      this.BufferedStream_ = new EndianBinaryBufferedStream {
          BaseStream = baseStream,
          Endianness = endianness
      };
    }

    ~EndianBinaryReader() {
      this.Dispose(false);
    }

    public void Close() => this.Dispose();

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing) {
      if (this.disposed_) {
        return;
      }
      if (disposing && this.BaseStream != null) {
        this.BaseStream.Close();
      }
      this.disposed_ = true;
    }

    public long Position {
      get => this.BaseStream.Position;
      set => this.BaseStream.Position = value;
    }

    public bool Eof => this.BaseStream.Position == this.BaseStream.Length;

    public void Align(uint amt) {
      var offs = amt - (this.BaseStream.Position % amt);
      if (offs != amt) {
        this.BaseStream.Position += offs;
      }
    }

    public void Subread(long position, Action<EndianBinaryReader> subread) {
      var tempPos = this.Position;
      {
        this.Position = position;
        subread(this);
      }
      this.Position = tempPos;
    }


    /**
     * Methods for reading individual values and lists of values are generated
     * for each of the following types within the FinGenerated project.
     */
    private static byte ConvertByte_(byte[] buffer, int i) => buffer[i];

    private static sbyte ConvertSByte_(byte[] buffer, int i)
      => (sbyte) buffer[i];

    private static short ConvertInt16_(byte[] buffer, int i)
      => BitConverter.ToInt16(buffer, sizeof(short) * i);

    private static ushort ConvertUInt16_(byte[] buffer, int i)
      => BitConverter.ToUInt16(buffer, sizeof(ushort) * i);

    private static int ConvertInt32_(byte[] buffer, int i)
      => BitConverter.ToInt32(buffer, sizeof(int) * i);

    private static uint ConvertUInt32_(byte[] buffer, int i)
      => BitConverter.ToUInt32(buffer, sizeof(uint) * i);

    private static long ConvertInt64_(byte[] buffer, int i)
      => BitConverter.ToInt64(buffer, sizeof(long) * i);

    private static ulong ConvertUInt64_(byte[] buffer, int i)
      => BitConverter.ToUInt64(buffer, sizeof(ulong) * i);

    private static float ConvertSingle_(byte[] buffer, int i)
      => BitConverter.ToSingle(buffer, sizeof(float) * i);

    private static double ConvertDouble_(byte[] buffer, int i)
      => BitConverter.ToDouble(buffer, sizeof(double) * i);

    private static float ConvertUn8_(byte[] buffer, int i)
      => EndianBinaryReader.ConvertByte_(buffer, i) / 255f;

    private static float ConvertSn16_(byte[] buffer, int i)
      => EndianBinaryReader.ConvertInt16_(buffer, i) / (65535f / 2);

    private static float ConvertUn16_(byte[] buffer, int i)
      => EndianBinaryReader.ConvertUInt16_(buffer, i) / 65535f;

    private static void Assert<T>(T expectedValue, T actualValue) {
      if (!expectedValue.Equals(actualValue)) {
        throw new Exception(
            "Expected " + actualValue + " to be " + expectedValue);
      }
    }

    private void FillBuffer_(int count, int? optStride = null)
      => this.BufferedStream_.FillBuffer(count, optStride);


    public void AssertByte(byte expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadByte());

    public byte ReadByte() {
      this.FillBuffer_(sizeof(byte));
      return EndianBinaryReader.ConvertByte_(this.BufferedStream_.Buffer, 0);
    }

    public byte[] ReadBytes(int count) => this.ReadBytes(new byte[count]);

    public byte[] ReadBytes(byte[] dst) {
      const int size = sizeof(byte);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertByte_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertSByte(sbyte expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadSByte());

    public sbyte ReadSByte() {
      this.FillBuffer_(sizeof(sbyte));
      return EndianBinaryReader.ConvertSByte_(this.BufferedStream_.Buffer, 0);
    }

    public sbyte[] ReadSBytes(int count) => this.ReadSBytes(new sbyte[count]);

    public sbyte[] ReadSBytes(sbyte[] dst) {
      const int size = sizeof(sbyte);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertSByte_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertInt16(short expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt16());

    public short ReadInt16() {
      this.FillBuffer_(sizeof(short));
      return EndianBinaryReader.ConvertInt16_(this.BufferedStream_.Buffer, 0);
    }

    public short[] ReadInt16s(int count) => this.ReadInt16s(new short[count]);

    public short[] ReadInt16s(short[] dst) {
      const int size = sizeof(short);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertInt16_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUInt16(ushort expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt16());

    public ushort ReadUInt16() {
      this.FillBuffer_(sizeof(ushort));
      return EndianBinaryReader.ConvertUInt16_(this.BufferedStream_.Buffer, 0);
    }

    public ushort[] ReadUInt16s(int count)
      => this.ReadUInt16s(new ushort[count]);

    public ushort[] ReadUInt16s(ushort[] dst) {
      const int size = sizeof(ushort);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertUInt16_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertInt32(int expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt32());

    public int ReadInt32() {
      this.FillBuffer_(sizeof(int));
      return EndianBinaryReader.ConvertInt32_(this.BufferedStream_.Buffer, 0);
    }

    public int[] ReadInt32s(int count) => this.ReadInt32s(new int[count]);

    public int[] ReadInt32s(int[] dst) {
      const int size = sizeof(int);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertInt32_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUInt32(uint expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt32());

    public uint ReadUInt32() {
      this.FillBuffer_(sizeof(uint));
      return EndianBinaryReader.ConvertUInt32_(this.BufferedStream_.Buffer, 0);
    }

    public uint[] ReadUInt32s(int count) => this.ReadUInt32s(new uint[count]);

    public uint[] ReadUInt32s(uint[] dst) {
      const int size = sizeof(uint);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertUInt32_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertInt64(long expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadInt64());

    public long ReadInt64() {
      this.FillBuffer_(sizeof(long));
      return EndianBinaryReader.ConvertInt64_(this.BufferedStream_.Buffer, 0);
    }

    public long[] ReadInt64s(int count) => this.ReadInt64s(new long[count]);

    public long[] ReadInt64s(long[] dst) {
      const int size = sizeof(long);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertInt64_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUInt64(ulong expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUInt64());

    public ulong ReadUInt64() {
      this.FillBuffer_(sizeof(ulong));
      return EndianBinaryReader.ConvertUInt64_(this.BufferedStream_.Buffer, 0);
    }

    public ulong[] ReadUInt64s(int count) => this.ReadUInt64s(new ulong[count]);

    public ulong[] ReadUInt64s(ulong[] dst) {
      const int size = sizeof(ulong);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertUInt64_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertSingle(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadSingle());

    public float ReadSingle() {
      this.FillBuffer_(sizeof(float));
      return EndianBinaryReader.ConvertSingle_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadSingles(int count) => this.ReadSingles(new float[count]);

    public float[] ReadSingles(float[] dst) {
      const int size = sizeof(float);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertSingle_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertDouble(double expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadDouble());

    public double ReadDouble() {
      this.FillBuffer_(sizeof(double));
      return EndianBinaryReader.ConvertDouble_(this.BufferedStream_.Buffer, 0);
    }

    public double[] ReadDoubles(int count)
      => this.ReadDoubles(new double[count]);

    public double[] ReadDoubles(double[] dst) {
      const int size = sizeof(double);
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {
        dst[i] =
            EndianBinaryReader.ConvertDouble_(this.BufferedStream_.Buffer, i);
      }
      return dst;
    }


    public void AssertUn8(float expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadUn8());

    public float ReadUn8() {
      this.FillBuffer_(sizeof(byte));
      return EndianBinaryReader.ConvertUn8_(this.BufferedStream_.Buffer, 0);
    }

    public float[] ReadUn8s(int count) => this.ReadUn8s(new float[count]);

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

    public float[] ReadSn16s(int count) => this.ReadSn16s(new float[count]);

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

    public float[] ReadUn16s(int count) => this.ReadUn16s(new float[count]);

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
    public char[] ReadChars(int count) => this.ReadChars(Encoding.ASCII, count);
    public char[] ReadChars(char[] dst) => this.ReadChars(Encoding.ASCII, dst);


    public void AssertChar(Encoding encoding, char expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadChar(encoding));

    public char ReadChar(Encoding encoding) {
      var encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.BufferedStream_.FillBuffer(encodingSize, encodingSize);
      return encoding.GetChars(this.BufferedStream_.Buffer, 0, encodingSize)[0];
    }

    public char[] ReadChars(Encoding encoding, int count)
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

    public string ReadString(int count)
      => this.ReadString(Encoding.ASCII, count);


    public void AssertString(Encoding encoding, string expectedValue)
      => EndianBinaryReader.Assert(
          expectedValue,
          this.ReadString(encoding, expectedValue.Length));

    public string ReadString(Encoding encoding, int count)
      => new(this.ReadChars(encoding, count));


    public void AssertStringNT(string expectedValue)
      => this.AssertStringNT(Encoding.ASCII, expectedValue);

    public string ReadStringNT() => this.ReadStringNT(Encoding.ASCII);


    public void AssertStringNT(Encoding encoding, string expectedValue)
      => EndianBinaryReader.Assert(
          expectedValue,
          this.ReadStringNT(encoding));

    public string ReadStringNT(Encoding encoding) {
      string str = "";
      do {
        str += this.ReadChar(encoding);
      } while (!str.EndsWith("\0", StringComparison.Ordinal));
      return str.Remove(str.Length - 1);
    }

    public void AssertMagicText(string expectedText) {
      var actualText = this.ReadString(expectedText.Length);

      if (expectedText != actualText) {
        throw new Exception(
            $"Expected to find magic text \"{expectedText}\", but found \"{actualText}\"");
      }
    }

    public T ReadNew<T>() where T : IDeserializable, new() {
      var value = new T();
      value.Read(this);
      return value;
    }

    public void ReadNewArray<T>(out T[] array, int length)
        where T : IDeserializable, new() {
      array = new T[length];
      for (var i = 0; i < length; ++i) {
        array[i] = this.ReadNew<T>();
      }
    }
  }
}