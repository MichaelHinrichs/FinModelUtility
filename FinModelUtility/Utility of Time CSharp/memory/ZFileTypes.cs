namespace UoT.memory {
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

    string FileName { get; }
    uint Offset { get; }
    uint Length { get; }
  }


  public abstract class BZFile : IZFile {
    protected BZFile(uint offset, uint length) {
      this.Offset = offset;
      this.Length = length;
    }

    public abstract ZFileType Type { get; }

    public string FileName { get; set; }

    public uint Offset { get; }
    public uint Length { get; }

    public override string ToString() => this.FileName;
  }


  public class ZObject : BZFile {
    public ZObject(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.OBJECT;
  }


  public class ZCodeFiles : BZFile {
    public ZCodeFiles(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.CODE;
  }


  public class ZScene : BZFile {
    public ZScene(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.SCENE;

    // TODO: Make nonnull via init, C#9.
    public ZMap[]? Maps;
  }

  public class ZMap : BZFile {
    public ZMap(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.MAP;

    // TODO: Make nonnull via init, C#9.
    public ZScene? Scene { get; set; }
  }

  public class ZObjectSet : BZFile {
    public ZObjectSet(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.OBJECT_SET;
  }

  public class ZOtherData : BZFile {
    public ZOtherData(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.OTHER;
  }
}