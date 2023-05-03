using f3dzex2.io;

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
    Segment Segment { get; }
  }


  public abstract class BZFile : IZFile {
    protected BZFile(Segment segment) {
      this.Segment = segment;
    }

    public abstract ZFileType Type { get; }

    public string FileName { get; set; }

    public Segment Segment { get; }

    public override string ToString() => this.FileName;
  }


  public class ZObject : BZFile {
    public ZObject(Segment segment) : base(segment) { }
    public override ZFileType Type => ZFileType.OBJECT;
  }


  public class ZCodeFiles : BZFile {
    public ZCodeFiles(Segment segment) : base(segment) { }
    public override ZFileType Type => ZFileType.CODE;
  }


  public class ZScene : BZFile {
    public ZScene(Segment segment) : base(segment) { }
    public override ZFileType Type => ZFileType.SCENE;

    // TODO: Make nonnull via init, C#9.
    public ZMap[]? Maps;
  }

  public class ZMap : BZFile {
    public ZMap(Segment segment) : base(segment) { }
    public override ZFileType Type => ZFileType.MAP;

    // TODO: Make nonnull via init, C#9.
    public ZScene? Scene { get; set; }
  }

  public class ZObjectSet : BZFile {
    public ZObjectSet(Segment segment) : base(segment) { }
    public override ZFileType Type => ZFileType.OBJECT_SET;
  }

  public class ZOtherData : BZFile {
    public ZOtherData(Segment segment) : base(segment) { }
    public override ZFileType Type => ZFileType.OTHER;
  }
}