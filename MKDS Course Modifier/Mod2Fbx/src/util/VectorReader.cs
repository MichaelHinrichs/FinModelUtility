using System.Collections.Generic;

namespace mod.util {
  public enum Endianness {
    Little = 0,
    Big
  }

  public class VectorReader {
    private readonly List<byte> buffer_ = new();
    private int position_;
    private Endianness endianness_;

    public VectorReader(Endianness endianness = Endianness.Little) {
      this.endianness_ = endianness;
    }

    public VectorReader(
        List<byte> bytes,
        int position = 0,
        Endianness endianness = Endianness.Little) {
      this.buffer_ = bytes;
      this.position_ = position;
      this.endianness_ = endianness;
    }

    public List<byte> GetBuffer() => this.buffer_;

    public int GetRemaining() => this.buffer_.Count - this.position_;

    public Endianness Endianness => this.endianness_;


    public void SetPosition(int position) {
      if (position <= this.buffer_.Count) {
        this.position_ = position;
      }
    }

    public int GetPosition() => this.position_;

    public void ReadBuffer(byte[] buffer, int size) {
      for (var i = 0; i < size; i++) {
        buffer[i] = this.buffer_[this.position_ + i];
      }
    }

    public byte ReadU8() {
      return this.buffer_[this.position_++];
    }

    public ushort ReadU16() {
      var byte0 = this.ReadU8();
      var byte1 = this.ReadU8();

      return (ushort) ((this.endianness_ == Endianness.Little)
                 ? (byte0 | (byte1 << 8))
                 : (byte1 | (byte0 << 8)));
    }

    public uint ReadU32() {
      var byte0 = this.ReadU8();
      var byte1 = this.ReadU8();
      var byte2 = this.ReadU8();
      var byte3 = this.ReadU8();

      return (uint) ((this.endianness_ == Endianness.Little)
                 ? (byte0 | (byte1 << 8) | (byte2 << 16) | (byte3 << 24))
                 : (byte3 | (byte2 << 8) | (byte1 << 16) | (byte0 << 24)));
    }

    public ulong ReadU64() {
      var word0 = this.ReadU32();
      var word1 = this.ReadU32();

      return (this.endianness_ == Endianness.Little)
                 ? (((ulong) word0) | ((ulong) word1 << 32))
                 : (((ulong) word1) | ((ulong) word0 << 32));
    }

    public sbyte ReadS8() {
      return (sbyte) this.buffer_[this.position_++];
    }

    public short ReadS16() {
      var byte0 = this.ReadS8();
      var byte1 = this.ReadS8();

      return (short) ((this.endianness_ == Endianness.Little)
                 ? (byte0 | (byte1 << 8))
                 : (byte1 | (byte0 << 8)));
    }

    public int ReadS32() {
      var byte0 = this.ReadS8();
      var byte1 = this.ReadS8();
      var byte2 = this.ReadS8();
      var byte3 = this.ReadS8();

      return (this.endianness_ == Endianness.Little)
                 ? (byte0 | (byte1 << 8) | (byte2 << 16) | (byte3 << 24))
                 : (byte3 | (byte2 << 8) | (byte1 << 16) | (byte0 << 24));
    }

    public long ReadS64() {
      var word0 = this.ReadS32();
      var word1 = this.ReadS32();

      return (this.endianness_ == Endianness.Little)
                 ? (((long) word0 << 32) | (word1))
                 : (((long) word1 << 32) | (word0));
    }
  }
}