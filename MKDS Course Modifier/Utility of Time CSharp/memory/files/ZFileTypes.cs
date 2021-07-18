using UoT.memory.map;

namespace UoT.memory.files {
  public enum ZFileType {
    OBJECT,
    CODE,
    SCENE,
    MAP,

    /// <summary>
    ///   A set of objects in a given map. These seem to be used to switch
    ///   between different versions of rooms.
    /// </summary>
    OBJECT_SET,
    OTHER,
  }

  public interface IZFile {
    ZFileType Type { get; }

    // TODO: Make these nonnull via init setters in C#9.
    string? FileName { get; set; }
    string? BetterFileName { get; set; }

    IShardedMemory Region { get; }
  }


  public abstract class BZFile : IZFile {
    protected BZFile(IShardedMemory region) {
      this.Region = region;
    }

    public abstract ZFileType Type { get; }

    public string? FileName { get; set; }
    public string? BetterFileName { get; set; }

    public IShardedMemory Region { get; }
  }


  public class ZObj : BZFile {
    public ZObj(IShardedMemory region) : base(region) {}
    public override ZFileType Type => ZFileType.OBJECT;
  }


  public class ZCodeFiles : BZFile {
    public ZCodeFiles(IShardedMemory region) : base(region) { }
    public override ZFileType Type => ZFileType.CODE;
  }


  public class ZSc : BZFile {
    public ZSc(IShardedMemory region) : base(region) { }
    public override ZFileType Type => ZFileType.SCENE;

    // TODO: Make nonnull via init, C#9.
    public ZMap[]? Maps;
  }

  public class ZMap : BZFile {
    public ZMap(IShardedMemory region) : base(region) { }
    public override ZFileType Type => ZFileType.MAP;

    // TODO: Make nonnull via init, C#9.
    public ZSc? Scene { get; set; }
  }

  public class ZObjectSet : BZFile {
    public ZObjectSet(IShardedMemory region) : base(region) { }
    public override ZFileType Type => ZFileType.OBJECT_SET;
  }


  public class ZOtherData : BZFile {
    public ZOtherData(IShardedMemory region) : base(region) { }
    public override ZFileType Type => ZFileType.OTHER;
  }
}