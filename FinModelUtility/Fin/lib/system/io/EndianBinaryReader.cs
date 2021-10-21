// Decompiled with JetBrains decompiler
// Type: System.IO.EndianBinaryReader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Text;

using fin.util.asserts;

namespace System.IO {
  public sealed class EndianBinaryReader : IDisposable {
    private bool disposed_;
    private byte[] buffer_;

    public Stream BaseStream { get; private set; }

    public Endianness Endianness { get; set; }

    public Endianness SystemEndianness {
      get {
        return BitConverter.IsLittleEndian
                   ? Endianness.LittleEndian
                   : Endianness.BigEndian;
      }
    }

    private bool Reverse {
      get { return this.SystemEndianness != this.Endianness; }
    }

    public EndianBinaryReader(Stream baseStream)
        : this(baseStream, Endianness.BigEndian) {}

    public EndianBinaryReader(Stream baseStream, Endianness endianness) {
      if (baseStream == null)
        throw new ArgumentNullException(nameof(baseStream));
      if (!baseStream.CanRead)
        throw new ArgumentException(nameof(baseStream));
      this.BaseStream = baseStream;
      this.Endianness = endianness;
    }

    ~EndianBinaryReader() {
      this.Dispose(false);
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

    private void FillBuffer_(int bytes, int stride) {
      if (this.buffer_ == null || this.buffer_.Length < bytes)
        this.buffer_ = new byte[bytes];
      this.BaseStream.Read(this.buffer_, 0, bytes);
      if (!this.Reverse)
        return;
      for (int index = 0; index < bytes; index += stride)
        Array.Reverse((Array) this.buffer_, index, stride);
    }

    public byte ReadByte() {
      this.FillBuffer_(1, 1);
      return this.buffer_[0];
    }

    public byte[] ReadBytes(int count) => this.ReadBytes(new byte[count]);

    public byte[] ReadBytes(byte[] dst) {
      this.FillBuffer_(dst.Length, 1);
      Array.Copy((Array) this.buffer_, 0, (Array) dst, 0, dst.Length);
      return dst;
    }

    public sbyte ReadSByte() {
      this.FillBuffer_(1, 1);
      return (sbyte) this.buffer_[0];
    }

    public sbyte[] ReadSBytes(int count) {
      sbyte[] numArray = new sbyte[count];
      this.FillBuffer_(count, 1);
      for (int index = 0; index < count; ++index)
        numArray[index] = (sbyte) this.buffer_[index];
      return numArray;
    }

    public char ReadChar(Encoding encoding) {
      int encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.FillBuffer_(encodingSize, encodingSize);
      return encoding.GetChars(this.buffer_, 0, encodingSize)[0];
    }

    public char[] ReadChars(Encoding encoding, int count) {
      int encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.FillBuffer_(encodingSize * count, encodingSize);
      return encoding.GetChars(this.buffer_, 0, encodingSize * count);
    }

    private static int GetEncodingSize_(Encoding encoding) {
      return encoding == Encoding.UTF8 ||
             encoding == Encoding.ASCII ||
             encoding != Encoding.Unicode &&
             encoding != Encoding.BigEndianUnicode
                 ? 1
                 : 2;
    }

    public string ReadStringNT(Encoding encoding) {
      string str = "";
      do {
        str += this.ReadChar(encoding);
      } while (!str.EndsWith("\0", StringComparison.Ordinal));
      return str.Remove(str.Length - 1);
    }

    public string ReadStringNT() => this.ReadStringNT(Encoding.ASCII);

    public void AssertMagicText(string expectedText) {
      var actualText = this.ReadString(expectedText.Length);
      Asserts.Equal(expectedText,
                    actualText,
                    $"Expected to find magic text \"{expectedText}\", but found \"{actualText}\"");
    }

    public string ReadString(Encoding encoding, int count)
      => new(this.ReadChars(encoding, count));

    public string ReadString(int count)
      => this.ReadString(Encoding.ASCII, count);


    public double ReadDouble() {
      this.FillBuffer_(8, 8);
      return BitConverter.ToDouble(this.buffer_, 0);
    }

    public double[] ReadDoubles(int count) {
      double[] numArray = new double[count];
      this.FillBuffer_(8 * count, 8);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToDouble(this.buffer_, 8 * index);
      return numArray;
    }


    public float ReadSingle() {
      this.FillBuffer_(4, 4);
      return BitConverter.ToSingle(this.buffer_, 0);
    }

    public float[] ReadSingles(int count) => this.ReadSingles(new float[count]);

    public float[] ReadSingles(float[] dst) {
      this.FillBuffer_(4 * dst.Length, 4);
      for (int index = 0; index < dst.Length; ++index)
        dst[index] = BitConverter.ToSingle(this.buffer_, 4 * index);
      return dst;
    }


    public float ReadSn16() => this.ReadInt16() / (65535f / 2);
    public float ReadUn16() => this.ReadUInt16() / 65535f;


    public int ReadInt32() {
      this.FillBuffer_(4, 4);
      return BitConverter.ToInt32(this.buffer_, 0);
    }

    public int[] ReadInt32s(int count) {
      int[] numArray = new int[count];
      this.FillBuffer_(4 * count, 4);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToInt32(this.buffer_, 4 * index);
      return numArray;
    }

    public long ReadInt64() {
      this.FillBuffer_(8, 8);
      return BitConverter.ToInt64(this.buffer_, 0);
    }

    public long[] ReadInt64s(int count) {
      long[] numArray = new long[count];
      this.FillBuffer_(8 * count, 8);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToInt64(this.buffer_, 8 * index);
      return numArray;
    }

    public short ReadInt16() {
      this.FillBuffer_(2, 2);
      return BitConverter.ToInt16(this.buffer_, 0);
    }

    public short[] ReadInt16s(int count) {
      short[] numArray = new short[count];
      this.FillBuffer_(2 * count, 2);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToInt16(this.buffer_, 2 * index);
      return numArray;
    }


    public ushort ReadUInt16() {
      this.FillBuffer_(2, 2);
      return BitConverter.ToUInt16(this.buffer_, 0);
    }

    public ushort[] ReadUInt16s(int count) {
      ushort[] numArray = new ushort[count];
      this.FillBuffer_(2 * count, 2);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToUInt16(this.buffer_, 2 * index);
      return numArray;
    }


    public uint ReadUInt32() {
      this.FillBuffer_(4, 4);
      return BitConverter.ToUInt32(this.buffer_, 0);
    }

    public uint[] ReadUInt32s(int count) => this.ReadUInt32s(new uint[count]);

    public uint[] ReadUInt32s(uint[] dst) {
      this.FillBuffer_(4 * dst.Length, 4);
      for (int index = 0; index < dst.Length; ++index)
        dst[index] = BitConverter.ToUInt32(this.buffer_, 4 * index);
      return dst;
    }


    public ulong ReadUInt64() {
      this.FillBuffer_(8, 8);
      return BitConverter.ToUInt64(this.buffer_, 0);
    }

    public ulong[] ReadUInt64s(int count) {
      ulong[] numArray = new ulong[count];
      this.FillBuffer_(8 * count, 8);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToUInt64(this.buffer_, 8 * index);
      return numArray;
    }

    public void Close() {
      this.Dispose();
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing) {
      if (this.disposed_)
        return;
      if (disposing && this.BaseStream != null)
        this.BaseStream.Close();
      this.buffer_ = (byte[]) null;
      this.disposed_ = true;
    }
  }
}