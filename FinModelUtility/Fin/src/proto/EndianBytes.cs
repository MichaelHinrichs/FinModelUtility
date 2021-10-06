using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace fin.proto {
  public enum Endianness {
    LITTLE,
    BIG,
  }

  public class EndianBytes {
    private readonly Endianness endianness_;
    private readonly byte[] impl_;

    /// <summary>
    ///   Used for flipping endianness in-place without extra allocations.
    /// </summary>
    private readonly byte[] otherEndianImpl_;

    public EndianBytes(byte[] impl, Endianness endianness = Endianness.LITTLE) {
      this.endianness_ = endianness;
      this.impl_ = impl;
      this.otherEndianImpl_ = new byte[this.impl_.Length];
    }

    public byte this[int index] {
      get => this.impl_[index];
      set => this.impl_[index] = value;
    }

    public T Deserialize<T>(int offset) {
      var size = Marshal.SizeOf<T>();

      var deserializationBytes = this.impl_;
      if (this.endianness_ != Endianness.LITTLE) {
        deserializationBytes = this.otherEndianImpl_;
        Buffer.BlockCopy(this.impl_,
                         offset,
                         this.otherEndianImpl_,
                         offset,
                         size);
        FlipStructureInPlace_<T>(deserializationBytes, offset);
      }

      return FinMarshal.Deserialize<T>(deserializationBytes, offset);
    }


     static void FlipStructureInPlace_<T>(byte[] bytes, int offset)
      => FlipStructureInPlace_(typeof(T), bytes, offset);

    // https://stackoverflow.com/questions/2480116/marshalling-a-big-endian-byte-collection-into-a-struct-in-order-to-pull-out-valu
    public static void FlipStructureInPlace_(
        Type type,
        byte[] bytes,
        int offset) {
      var size = Marshal.SizeOf(type);

      if (type.IsPrimitive) {
        Array.Reverse(bytes, offset, size);
        return;
      }

      var totalOffset = 0;
      foreach (var field in type.GetFields()) {
        FlipStructureInPlace_(field.FieldType, bytes, totalOffset);
        totalOffset += Marshal.SizeOf(field.FieldType);
      }
    }
  }
}