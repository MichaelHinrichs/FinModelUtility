using schema;
using schema.attributes.ignore;


namespace Quad64.schema {
  [BinarySchema]
  public partial class PresetMacroEntry : IBiSerializable {
    [Ignore]
    public ushort PresetID { get; set; }

    public uint Behavior { get; set; }
    public byte Unknown { get; set; }
    public byte ModelID { get; set; }
    public byte BehaviorParameter1 { get; set; }
    public byte BehaviorParameter2 { get; set; }


    public PresetMacroEntry() { }

    public PresetMacroEntry(ushort presetID, byte modelID, uint behavior) {
      this.PresetID = presetID;
      this.ModelID = modelID;
      this.Behavior = behavior;
    }

    public PresetMacroEntry(ushort presetID,
                            byte modelID,
                            uint behavior,
                            byte bp1,
                            byte bp2) {
      this.PresetID = presetID;
      this.ModelID = modelID;
      this.Behavior = behavior;
      this.BehaviorParameter1 = bp1;
      this.BehaviorParameter2 = bp2;
    }
  }
}