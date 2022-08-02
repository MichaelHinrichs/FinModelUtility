using schema.memory;


namespace schema.indirect {
  public interface IMemoryRange {
    IMemoryBlock? Parent { get; }

    long GetAbsoluteOffsetInBytes();
    long GetRelativeOffsetInBytes();
    long SizeInBytes { get; }
  }


  public interface IMemoryPointer : IMemoryRange {
    new IMemoryBlock Parent { get; }
  }

  public enum MemoryBlockType {
    ARCHIVE,
    FILE,
    SECTION,
    DATA,
  }

  public interface IMemoryBlock : IMemoryRange {
    MemoryBlockType Type { get; }


    IMemoryBlock ClaimBlockWithin(
        MemoryBlockType type,
        long offsetInBytes,
        long sizeInBytes);

    IMemoryBlock ClaimBlockAtEnd(
        MemoryBlockType type,
        long sizeInBytes);


    IMemoryPointer ClaimPointerWithin(long offsetInBytes, long sizeInBytes);
    IMemoryPointer ClaimPointerAtEnd(long sizeInBytes);
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

    public IMemoryBlock ClaimBlockAtEnd(MemoryBlockType type, long sizeInBytes) {
      var subrange =
          this.impl_.ClaimSubrangeAtEnd(null, sizeInBytes);
      var block = new MemoryBlock(subrange, type);
      subrange.Data = block;

      return block;
    }

    public IMemoryPointer
        ClaimPointerWithin(long offsetInBytes, long sizeInBytes) {
      throw new System.NotImplementedException();
    }

    public IMemoryPointer ClaimPointerAtEnd(long sizeInBytes) {
      throw new System.NotImplementedException();
    }

    public long GetAbsoluteOffsetInBytes() => this.impl_.GetAbsoluteOffset();
    public long GetRelativeOffsetInBytes() => this.impl_.GetRelativeOffset();
    public long SizeInBytes => this.impl_.Length;

    private class MemoryPointer : IMemoryPointer {
      private readonly INestedRanges<IMemoryRange?> impl_;

      public MemoryPointer(INestedRanges<IMemoryRange?> impl) {
        this.impl_ = impl;
      }

      public IMemoryBlock Parent => (this.impl_.Parent!.Data as IMemoryBlock)!;

      public long GetAbsoluteOffsetInBytes() => this.impl_.GetAbsoluteOffset();
      public long GetRelativeOffsetInBytes() => this.impl_.GetRelativeOffset();
      public long SizeInBytes => this.impl_.Length;
    }
  }
}