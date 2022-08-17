using schema;


namespace dat.schema {
  [Flags]
  public enum PObjFlags : ushort {
    OBJTYPE_SKIN = 0 << 12,
    OBJTYPE_SHAPEANIM = 1 << 12,
    OBJTYPE_ENVELOPE = 1 << 13,
    OBJTYPE_MASK = 0x3000,

    CULLFRONT = 1 << 14,
    CULLBACK = 1 << 15,
  }

  [Schema]
  public partial class PObjData : IBiSerializable {
    public uint StringOffset { get; set; }
    public uint NextPObjOffset { get; set; }
    public uint VertexDescriptorListOffset { get; set; }
    public PObjFlags Flags { get; set; }
    public ushort nDisp { get; set; }
    public ushort DisplayOffset { get; set; }
    public ushort ContentsOffset { get; set; }
  }

  public class PObj {
    public PObjData Data { get; set; } = new();

    public List<VertexDescriptor> VertexDescriptors { get; } = new();

    public List<(ushort, ushort, ushort)> PositionIndices { get; } = new();
  }
}
