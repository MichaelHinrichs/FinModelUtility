using UoT.memory.mapper;
using UoT.util.array;
using UoT.util.data;

namespace UoT.memory.map {
  public enum ShardedMemoryType {
    UNKNOWN,

    ROOT,

    // Files
    OBJECT,
    CODE,
    SCENE,
    MAP,
    OTHER_DATA,

    // Graphics
    LIMB_HIERARCHY,
    DISPLAY_LIST,
    TEXTURE,
    PALETTE,
  }

  /// <summary>
  ///   Helper interface for splitting up regions of ROM by type into
  ///   hierarchies. Especially useful for identifying regions of interest,
  ///   e.g. regions that have not yet been identified.
  /// </summary>
  public interface IShardedMemory : IMemorySource {
    ShardedMemoryType ShardType { get; set; }

    IShardedListAddress GlobalOffset { get; }

    IShardedMemory Shard(int localOffset, int length);

    void Resize(int newLength);
  }

  public sealed class ShardedMemory : BIndexable, IShardedMemory {
    private readonly IShardedList<byte> impl_;

    private ShardedMemory(IShardedList<byte> impl) {
      this.impl_ = impl;
    }

    public static ShardedMemory From(params byte[] bytes)
      => new ShardedMemory(ShardedList<byte>.From(bytes));

    public ShardedMemoryType ShardType { get; set; }

    public override int Count => this.impl_.Length;

    public override byte this[int localOffset] {
      get => this.impl_[localOffset];
      set => this.impl_[localOffset] = value;
    }

    public IShardedListAddress GlobalOffset => this.impl_.GlobalOffset;

    public IShardedMemory Shard(int localOffset, int length)
      => new ShardedMemory(this.impl_.Shard(localOffset, length));

    public void Resize(int newLength) => this.impl_.Resize(newLength);
  }
}