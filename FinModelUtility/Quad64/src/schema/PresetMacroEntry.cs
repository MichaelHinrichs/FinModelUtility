using schema.binary;
using schema.binary.attributes.endianness;
using schema.binary.attributes.ignore;


namespace Quad64.schema {
  [Endianness(SchemaConstants.SM64_ENDIANNESS)]
  [BinarySchema]
  public partial class PresetMacroEntry : IBiSerializable {
    [Ignore]
    public ushort PresetId { get; set; }

    public uint Behavior { get; set; }
    public byte Unknown { get; set; }
    public byte ModelId { get; set; }
    public byte BehaviorParameter1 { get; set; }
    public byte BehaviorParameter2 { get; set; }


    public PresetMacroEntry() { }

    public PresetMacroEntry(ushort presetId, byte modelId, uint behavior) {
      this.PresetId = presetId;
      this.ModelId = modelId;
      this.Behavior = behavior;
    }

    public PresetMacroEntry(ushort presetId,
                            byte modelId,
                            uint behavior,
                            byte bp1,
                            byte bp2) {
      this.PresetId = presetId;
      this.ModelId = modelId;
      this.Behavior = behavior;
      this.BehaviorParameter1 = bp1;
      this.BehaviorParameter2 = bp2;
    }
  }
}