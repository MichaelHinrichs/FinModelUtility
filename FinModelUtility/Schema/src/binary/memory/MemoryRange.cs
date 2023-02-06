using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace schema.binary.memory {
  public interface IMemoryRange {
    IMemoryBlock? Parent { get; }

    long GetAbsoluteOffsetInBytes();
    long GetRelativeOffsetInBytes();

    long SizeInBytes { get; }
  }


  public interface IMemoryPointer : IMemoryRange {
    new IMemoryBlock Parent { get; }

    void Read(IEndianBinaryReader er);
    void Write(ISubEndianBinaryWriter ew);
  }

  public enum MemoryBlockType {
    ARCHIVE,
    FILE,
    SECTION,
    DATA,
  }

  public interface IMemoryBlock : IMemoryRange,
      IBinaryConvertible,
      IEnumerable<IMemoryRange> {
    MemoryBlockType Type { get; }

    IMemoryBlock ClaimBlockWithin(
        MemoryBlockType type,
        long offsetInBytes,
        long sizeInBytes);

    IMemoryBlock ClaimBlockAtEnd(
        MemoryBlockType type,
        long sizeInBytes);


    IMemoryPointer ClaimPointerWithin(
        long offsetInBytes,
        Action<IEndianBinaryReader> readHandler,
        Action<EndianBinaryWriter> writeHandler);

    IMemoryPointer ClaimPointerAtEnd(
        Action<IEndianBinaryReader> readHandler,
        Action<EndianBinaryWriter> writeHandler);
  }


  public class MemoryBlock : IMemoryBlock {
    private readonly INestedRanges<IMemoryRange?> impl_;

    public MemoryBlock(
        MemoryBlockType type,
        long sizeInBytes) {
      this.impl_ = new NestedRanges<IMemoryRange?>(null, this, sizeInBytes);
      this.Type = type;
    }

    private MemoryBlock(
        INestedRanges<IMemoryRange?> impl,
        MemoryBlockType type) {
      this.impl_ = impl;
      this.Type = type;
    }

    public MemoryBlockType Type { get; }

    public IMemoryBlock? Parent => this.impl_.Parent?.Data as IMemoryBlock;

    public IMemoryBlock ClaimBlockWithin(MemoryBlockType type,
                                         long offsetInBytes,
                                         long sizeInBytes) {
      var subrange =
          this.impl_.ClaimSubrangeWithin(null, offsetInBytes, sizeInBytes);
      var block = new MemoryBlock(subrange, type);
      subrange.Data = block;

      return block;
    }

    public IMemoryBlock
        ClaimBlockAtEnd(MemoryBlockType type, long sizeInBytes) {
      var subrange =
          this.impl_.ClaimSubrangeAtEnd(null, sizeInBytes);
      var block = new MemoryBlock(subrange, type);
      subrange.Data = block;

      return block;
    }

    public IMemoryPointer ClaimPointerWithin(
        long offsetInBytes,
        Action<IEndianBinaryReader> readHandler,
        Action<EndianBinaryWriter> writeHandler) {
      var pointerImpl = this.impl_.ClaimSubrangeWithin(null, offsetInBytes);
      var pointer = new MemoryPointer(
          pointerImpl,
          readHandler,
          writeHandler);
      pointerImpl.Data = pointer;

      return pointer;
    }

    public IMemoryPointer ClaimPointerAtEnd(
        Action<IEndianBinaryReader> readHandler,
        Action<EndianBinaryWriter> writeHandler) {
      var pointerImpl = this.impl_.ClaimSubrangeAtEnd(null);
      var pointer = new MemoryPointer(
          pointerImpl,
          readHandler,
          writeHandler);
      pointerImpl.Data = pointer;

      return pointer;
    }

    public long GetAbsoluteOffsetInBytes() => this.impl_.GetAbsoluteOffset();
    public long GetRelativeOffsetInBytes() => this.impl_.GetRelativeOffset();

    public long SizeInBytes => this.impl_.Length;


    public void Read(IEndianBinaryReader er) {
      /*var startingOffset = er.Position;
      foreach (var child in this) {
        var childStart =
            er.Position =
                startingOffset + child.GetRelativeOffsetInBytes();

        INestedRanges<IMemoryRange?>? impl = null;
        switch (child) {
          case MemoryBlock memoryBlock: {
            memoryBlock.Read(er);
            impl = memoryBlock.impl_;
            break;
          }
          case MemoryPointer memoryPointer: {
            memoryPointer.Data.Read(er);
            impl = memoryPointer.Impl;
            break;
          }
          default: throw new NotSupportedException();
        }

        var childLength = er.Position - childStart;
        impl!.ResizeInPlace(childLength);
      }
      this.impl_.RecalculateLengthFromChildren();
      er.Position = startingOffset + this.SizeInBytes;*/
    }

    public void Write(ISubEndianBinaryWriter ew) {
      /*var startingOffset = ew.Position;
      foreach (var child in this) {
        var childStart =
            ew.Position =
                startingOffset + child.GetRelativeOffsetInBytes();

        INestedRanges<IMemoryRange?>? impl = null;
        switch (child) {
          case MemoryBlock memoryBlock: {
            memoryBlock.Write(ew);
            break;
          }
          case MemoryPointer memoryPointer: {
            memoryPointer.Write(ew);
            break;
          }
          default: throw new NotSupportedException();
        }

        var childLength = ew.Position - childStart;
        impl!.ResizeSelfAndParents(childLength);
      }
      this.impl_.RecalculateLengthFromChildren();
      ew.Position = startingOffset + this.SizeInBytes;*/
    }


    private class MemoryPointer : IMemoryPointer {
      private Action<IEndianBinaryReader> readHandler_;
      private Action<EndianBinaryWriter> writeHandler_;

      public MemoryPointer(
          INestedRanges<IMemoryRange?> impl,
          Action<IEndianBinaryReader> readHandler,
          Action<EndianBinaryWriter> writeHandler) {
        this.Impl = impl;
        this.readHandler_ = readHandler;
        this.writeHandler_ = writeHandler;
      }

      public INestedRanges<IMemoryRange?> Impl { get; }

      public IMemoryBlock Parent => (this.Impl.Parent!.Data as IMemoryBlock)!;

      public void Read(IEndianBinaryReader er) {
        this.readHandler_(er);
      }

      public void Write(ISubEndianBinaryWriter ew) {
        /*var startPosition = ew.Position;
        this.writeHandler_(ew);
        var length = ew.Position - startPosition;

        this.Impl.ResizeSelfAndParents(length);*/
      }

      public long GetAbsoluteOffsetInBytes() => this.Impl.GetAbsoluteOffset();
      public long GetRelativeOffsetInBytes() => this.Impl.GetRelativeOffset();
      public long SizeInBytes => this.Impl.Length;
    }

    IEnumerator IEnumerable.GetEnumerator()
      => this.GetEnumerator();

    public IEnumerator<IMemoryRange> GetEnumerator()
      => this.impl_.Select(child => child.Data!).GetEnumerator();
  }
}