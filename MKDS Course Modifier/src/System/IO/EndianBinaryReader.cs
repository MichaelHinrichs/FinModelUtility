// Decompiled with JetBrains decompiler
// Type: System.IO.EndianBinaryReader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Text;

namespace System.IO
{
  public sealed class EndianBinaryReader : IDisposable
  {
    private bool disposed;
    private byte[] buffer;

    public Stream BaseStream { get; private set; }

    public Endianness Endianness { get; set; }

    public Endianness SystemEndianness
    {
      get
      {
        return BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
      }
    }

    private bool Reverse
    {
      get
      {
        return this.SystemEndianness != this.Endianness;
      }
    }

    public EndianBinaryReader(Stream baseStream)
      : this(baseStream, Endianness.BigEndian)
    {
    }

    public EndianBinaryReader(Stream baseStream, Endianness endianness)
    {
      if (baseStream == null)
        throw new ArgumentNullException(nameof (baseStream));
      if (!baseStream.CanRead)
        throw new ArgumentException(nameof (baseStream));
      this.BaseStream = baseStream;
      this.Endianness = endianness;
    }

    ~EndianBinaryReader()
    {
      this.Dispose(false);
    }

    private void FillBuffer(int bytes, int stride)
    {
      if (this.buffer == null || this.buffer.Length < bytes)
        this.buffer = new byte[bytes];
      this.BaseStream.Read(this.buffer, 0, bytes);
      if (!this.Reverse)
        return;
      for (int index = 0; index < bytes; index += stride)
        Array.Reverse((Array) this.buffer, index, stride);
    }

    public byte ReadByte()
    {
      this.FillBuffer(1, 1);
      return this.buffer[0];
    }

    public byte[] ReadBytes(int count)
    {
      this.FillBuffer(count, 1);
      byte[] numArray = new byte[count];
      Array.Copy((Array) this.buffer, 0, (Array) numArray, 0, count);
      return numArray;
    }

    public sbyte ReadSByte()
    {
      this.FillBuffer(1, 1);
      return (sbyte) this.buffer[0];
    }

    public sbyte[] ReadSBytes(int count)
    {
      sbyte[] numArray = new sbyte[count];
      this.FillBuffer(count, 1);
      for (int index = 0; index < count; ++index)
        numArray[index] = (sbyte) this.buffer[index];
      return numArray;
    }

    public char ReadChar(Encoding encoding)
    {
      int encodingSize = EndianBinaryReader.GetEncodingSize(encoding);
      this.FillBuffer(encodingSize, encodingSize);
      return encoding.GetChars(this.buffer, 0, encodingSize)[0];
    }

    public char[] ReadChars(Encoding encoding, int count)
    {
      int encodingSize = EndianBinaryReader.GetEncodingSize(encoding);
      this.FillBuffer(encodingSize * count, encodingSize);
      return encoding.GetChars(this.buffer, 0, encodingSize * count);
    }

    private static int GetEncodingSize(Encoding encoding)
    {
      return encoding == Encoding.UTF8 || encoding == Encoding.ASCII || encoding != Encoding.Unicode && encoding != Encoding.BigEndianUnicode ? 1 : 2;
    }

    public string ReadStringNT(Encoding encoding)
    {
      string str = "";
      do
      {
        str += this.ReadChar(encoding);
      }
      while (!str.EndsWith("\0", StringComparison.Ordinal));
      return str.Remove(str.Length - 1);
    }

    public string ReadString(Encoding encoding, int count)
    {
      return new string(this.ReadChars(encoding, count));
    }

    public double ReadDouble()
    {
      this.FillBuffer(8, 8);
      return BitConverter.ToDouble(this.buffer, 0);
    }

    public double[] ReadDoubles(int count)
    {
      double[] numArray = new double[count];
      this.FillBuffer(8 * count, 8);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToDouble(this.buffer, 8 * index);
      return numArray;
    }

    public float ReadSingle()
    {
      this.FillBuffer(4, 4);
      return BitConverter.ToSingle(this.buffer, 0);
    }

    public float[] ReadSingles(int count)
    {
      float[] numArray = new float[count];
      this.FillBuffer(4 * count, 4);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToSingle(this.buffer, 4 * index);
      return numArray;
    }

    public int ReadInt32()
    {
      this.FillBuffer(4, 4);
      return BitConverter.ToInt32(this.buffer, 0);
    }

    public int[] ReadInt32s(int count)
    {
      int[] numArray = new int[count];
      this.FillBuffer(4 * count, 4);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToInt32(this.buffer, 4 * index);
      return numArray;
    }

    public long ReadInt64()
    {
      this.FillBuffer(8, 8);
      return BitConverter.ToInt64(this.buffer, 0);
    }

    public long[] ReadInt64s(int count)
    {
      long[] numArray = new long[count];
      this.FillBuffer(8 * count, 8);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToInt64(this.buffer, 8 * index);
      return numArray;
    }

    public short ReadInt16()
    {
      this.FillBuffer(2, 2);
      return BitConverter.ToInt16(this.buffer, 0);
    }

    public short[] ReadInt16s(int count)
    {
      short[] numArray = new short[count];
      this.FillBuffer(2 * count, 2);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToInt16(this.buffer, 2 * index);
      return numArray;
    }

    public ushort ReadUInt16()
    {
      this.FillBuffer(2, 2);
      return BitConverter.ToUInt16(this.buffer, 0);
    }

    public ushort[] ReadUInt16s(int count)
    {
      ushort[] numArray = new ushort[count];
      this.FillBuffer(2 * count, 2);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToUInt16(this.buffer, 2 * index);
      return numArray;
    }

    public uint ReadUInt32()
    {
      this.FillBuffer(4, 4);
      return BitConverter.ToUInt32(this.buffer, 0);
    }

    public uint[] ReadUInt32s(int count)
    {
      uint[] numArray = new uint[count];
      this.FillBuffer(4 * count, 4);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToUInt32(this.buffer, 4 * index);
      return numArray;
    }

    public ulong ReadUInt64()
    {
      this.FillBuffer(8, 8);
      return BitConverter.ToUInt64(this.buffer, 0);
    }

    public ulong[] ReadUInt64s(int count)
    {
      ulong[] numArray = new ulong[count];
      this.FillBuffer(8 * count, 8);
      for (int index = 0; index < count; ++index)
        numArray[index] = BitConverter.ToUInt64(this.buffer, 8 * index);
      return numArray;
    }

    public void Close()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing && this.BaseStream != null)
        this.BaseStream.Close();
      this.buffer = (byte[]) null;
      this.disposed = true;
    }
  }
}
