// Decompiled with JetBrains decompiler
// Type: System.IO.EndianBinaryReader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Text;

using fin.util.asserts;

namespace System.IO {
  public sealed partial class EndianBinaryReader : IDisposable {
    private bool disposed_;
    private byte[] buffer_;

    public Stream BaseStream { get; private set; }

    public Endianness Endianness { get; set; }

    public static Endianness SystemEndianness
      => BitConverter.IsLittleEndian
             ? Endianness.LittleEndian
             : Endianness.BigEndian;

    public bool Reverse
      => EndianBinaryReader.SystemEndianness != this.Endianness;

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

    private void FillBuffer_(int count, int? optStride = null) {
      var stride = optStride ?? count;
      if (this.buffer_ == null || this.buffer_.Length < count) {
        this.buffer_ = new byte[count];
      }
      this.BaseStream.Read(this.buffer_, 0, count);

      if (!this.Reverse) {
        return;
      }
      for (int index = 0; index < count; index += stride) {
        Array.Reverse(this.buffer_, index, stride);
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

    /**
     * Methods for reading individual values and lists of values are generated
     * for each of the following types within the FinGenerated project.
     */
    private byte ConvertByte_(int i) => this.buffer_[i];

    private sbyte ConvertSByte_(int i) => (sbyte)this.buffer_[i];

    private short ConvertInt16_(int i)
      => BitConverter.ToInt16(this.buffer_, sizeof(short) * i);

    private ushort ConvertUInt16_(int i)
      => BitConverter.ToUInt16(this.buffer_, sizeof(ushort) * i);

    private int ConvertInt32_(int i)
      => BitConverter.ToInt32(this.buffer_, sizeof(int) * i);

    private uint ConvertUInt32_(int i)
      => BitConverter.ToUInt32(this.buffer_, sizeof(uint) * i);

    private long ConvertInt64_(int i)
      => BitConverter.ToInt64(this.buffer_, sizeof(long) * i);

    private ulong ConvertUInt64_(int i)
      => BitConverter.ToUInt64(this.buffer_, sizeof(ulong) * i);

    private float ConvertSingle_(int i)
      => BitConverter.ToSingle(this.buffer_, sizeof(float) * i);

    private double ConvertDouble_(int i)
      => BitConverter.ToDouble(this.buffer_, sizeof(double) * i);

    private float ConvertSn16_(int i) => this.ConvertInt16_(i) / (65535f / 2);
    private float ConvertUn16_(int i) => this.ConvertUInt16_(i) / 65535f;


    public void Close() {
      this.Dispose();
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize((object)this);
    }

    private void Dispose(bool disposing) {
      if (this.disposed_)
        return;
      if (disposing && this.BaseStream != null)
        this.BaseStream.Close();
      this.buffer_ = (byte[])null;
      this.disposed_ = true;
    }
  }
}