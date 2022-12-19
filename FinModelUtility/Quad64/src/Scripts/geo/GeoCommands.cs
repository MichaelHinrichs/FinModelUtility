using fin.schema.vector;
using schema;
using schema.attributes.ignore;


namespace sm64.scripts {
  public enum GeoCommandId : byte {
    BRANCH_AND_STORE = 0x00,
    TERMINATE = 0x01,
    BRANCH = 0x02,
    RETURN_FROM_BRANCH = 0x03,
    OPEN_NODE = 0x04,
    CLOSE_NODE = 0x05,
    SET_RENDER_RANGE = 0x0D,
    TRANSLATE_AND_ROTATE = 0x10,
    TRANSLATE = 0x11,
    ROTATE = 0x12,
    DISPLAY_LIST_WITH_OFFSET = 0x13,
    BILLBOARD = 0x14,
    DISPLAY_LIST = 0x15,
    SHADOW = 0x16,
    OBJECT_LIST = 0x17,
    BACKGROUND = 0x19,
    NOOP_1A = 0x1A,
    SCALE = 0x1D,
  }

  public enum GeoDrawingLayer : byte {
    OPAQUE_NO_AA,
    OPAQUE_WITH_AA,
    DECALS,
    INTERSECTING_POLYGONS,
    TRANSPARENT_PIXELS,
    BLENDING1,
    BLENDING2,
    BLENDING3
  }

  public interface IGeoCommandList {
    IReadOnlyList<IGeoCommand> Commands { get; }
  }

  public interface IGeoCommand {
    GeoCommandId Id { get; }
  }

  [BinarySchema]
  public partial class GeoBranchAndStoreCommand : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.BRANCH_AND_STORE;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;

    public uint GeoCommandSegmentedAddress { get; set; }

    [Ignore]
    public IGeoCommandList GeoCommandList { get; set; }
  }

  [BinarySchema]
  public partial class GeoTerminateCommand : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.TERMINATE;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoBranchCommand : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.BRANCH;

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool StoreReturnAddress { get; set; }

    private readonly ushort padding_ = 0;

    public uint GeoCommandSegmentedAddress { get; set; }

    [Ignore]
    public IGeoCommandList GeoCommandList { get; set; }
  }

  [BinarySchema]
  public partial class GeoReturnFromBranchCommand
      : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.RETURN_FROM_BRANCH;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoOpenNodeCommand : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.OPEN_NODE;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoCloseNodeCommand : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.CLOSE_NODE;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoSetRenderRangeCommand : IGeoCommand, IDeserializable {
    public GeoCommandId Id => GeoCommandId.SET_RENDER_RANGE;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;

    public short MinimumDistance { get; set; }
    public short MaximumDistance { get; set; }
  }

  public class GeoTranslateAndRotateCommand : IGeoCommand {
    public GeoCommandId Id => GeoCommandId.TRANSLATE_AND_ROTATE;

    /// <summary>
    ///   Determines whether display list is enabled.
    ///
    ///   Determines how the translation/rotation params are stored.
    /// </summary>
    public byte Params { get; set; }

    public GeoDrawingLayer DrawingLayer
      => GeoUtils.GetDrawingLayerFromParams(Params);

    public Vector3s Translation => new();
    public Vector3s Rotation => new();

    public uint? DisplayListSegmentedAddress { get; set; }
  }

  public class GeoTranslationCommand : IGeoCommand {
    public GeoCommandId Id => GeoCommandId.TRANSLATE;

    /// <summary>
    ///   Determines whether display list is enabled.
    /// </summary>
    public byte Params { get; set; }

    public GeoDrawingLayer DrawingLayer
      => GeoUtils.GetDrawingLayerFromParams(Params);

    public Vector3s Translation => new();

    public uint? DisplayListSegmentedAddress { get; set; }
  }

  public class GeoRotationCommand : IGeoCommand {
    public GeoCommandId Id => GeoCommandId.ROTATE;

    /// <summary>
    ///   Determines whether display list is enabled.
    /// </summary>
    public byte Params { get; set; }

    public GeoDrawingLayer DrawingLayer
      => GeoUtils.GetDrawingLayerFromParams(Params);

    public Vector3s Rotation => new();

    public uint? DisplayListSegmentedAddress { get; set; }
  }

  [BinarySchema]
  public partial class GeoDisplayListWithOffsetCommand
      : IGeoCommand, IBiSerializable {
    public GeoCommandId Id => GeoCommandId.DISPLAY_LIST_WITH_OFFSET;

    public GeoDrawingLayer DrawingLayer { get; set; }

    public Vector3s Offset => new();

    public uint? DisplayListSegmentedAddress { get; set; }
  }

  public class GeoBillboardCommand : IGeoCommand {
    public GeoCommandId Id => GeoCommandId.BILLBOARD;

    /// <summary>
    ///   Determines whether display list is enabled.
    /// </summary>
    public byte Params { get; set; }

    public GeoDrawingLayer DrawingLayer
      => GeoUtils.GetDrawingLayerFromParams(Params);

    public Vector3s Translation => new();

    public uint? DisplayListSegmentedAddress { get; set; }
  }

  [BinarySchema]
  public partial class GeoDisplayListCommand : IGeoCommand, IBiSerializable {
    public GeoCommandId Id => GeoCommandId.DISPLAY_LIST;

    public GeoDrawingLayer DrawingLayer { get; set; }
    public uint DisplayListSegmentedAddress { get; set; }
  }

  [BinarySchema]
  public partial class GeoShadowCommand : IGeoCommand, IBiSerializable {
    public GeoCommandId Id => GeoCommandId.SHADOW;
    private readonly byte padding_ = 0;

    public short ShadowType { get; set; }
    public short ShadowSolidity { get; set; }
    public short ShadowScale { get; set; }
  }

  [BinarySchema]
  public partial class GeoObjectListCommand : IGeoCommand, IBiSerializable {
    public GeoCommandId Id => GeoCommandId.OBJECT_LIST;

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding_ = 0;
  }

  [BinarySchema]
  public partial class GeoBackgroundCommand : IGeoCommand, IBiSerializable {
    public GeoCommandId Id => GeoCommandId.BACKGROUND;

    private readonly byte padding_ = 0;

    public short BackgroundIdOrColor { get; set; }

    public uint BackgroundFunc { get; set; }
  }

  [BinarySchema]
  public partial class GeoNoopCommand : IGeoCommand, IBiSerializable {
    public GeoCommandId Id { get; set; }

    [IntegerFormat(SchemaIntegerType.UINT24)]
    private readonly uint padding1_ = 0;

    private readonly uint padding2_ = 0;
  }

  public class GeoScaleCommand : IGeoCommand {
    public GeoCommandId Id => GeoCommandId.SCALE;

    /// <summary>
    ///   Determines whether display list is enabled.
    /// </summary>
    public byte Params { get; set; }

    public GeoDrawingLayer DrawingLayer
      => GeoUtils.GetDrawingLayerFromParams(Params);

    public uint Scale { get; set; }

    public uint? DisplayListSegmentedAddress { get; set; }
  }

  public class GeoNotImplementedCommand : IGeoCommand {
    public GeoCommandId Id { get; set; }
  }

  public static class GeoUtils {
    public static bool IsDisplayListAndDrawingLayerEnabled(byte param)
      => (param & 0x80) != 0;

    public static GeoDrawingLayer GetDrawingLayerFromParams(byte param)
      => (GeoDrawingLayer)
          (IsDisplayListAndDrawingLayerEnabled(param) ? param & 0xF : 0);
  }
}