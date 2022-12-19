using fin.schema.vector;
using schema;


namespace sm64.scripts {
  public interface IGeoCommand {
    byte Type { get; }
  }

  [BinarySchema]
  public partial class GeoBranchAndStoreCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x00;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly int padding_ = 0;

    public uint SegmentedAddress { get; set; }
  }

  [BinarySchema]
  public partial class GeoTerminateCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x01;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly int padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoBranchCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x02;

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool StoreReturnAddress { get; set; }

    private readonly ushort padding_ = 0;

    public uint SegmentedAddress { get; set; }
  }

  [BinarySchema]
  public partial class GeoReturnFromBranchCommand
      : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x03;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly int padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoOpenNodeCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x04;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly int padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoCloseNodeCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x05;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly int padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoSetRenderRangeCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x0D;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly int padding_ = 0;

    public short MinimumDistance { get; set; }
    public short MaximumDistance { get; set; }
  }

  public class GeoTranslateAndRotateCommand : IGeoCommand {
    public byte Type { get; } = 0x10;

    /// <summary>
    ///   Determines whether display list is enabled.
    ///
    ///   Determines how the translation/rotation params are stored.
    /// </summary>
    public byte Params { get; set; }

    public Vector3s Translation { get; } = new();
    public Vector3s Rotation { get; } = new();

    public uint? DisplayList { get; set; }
  }

  [BinarySchema]
  public partial class GeoTranslationCommand : IGeoCommand, IBiSerializable {
    public byte Type { get; } = 0x11;

    /// <summary>
    ///   Determines whether display list is enabled.
    /// </summary>
    public byte Params { get; set; }

    public Vector3s Translation { get; } = new();

    public uint? DisplayList { get; set; }
  }
}