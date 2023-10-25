using schema.binary;
using schema.binary.attributes;

namespace dat.schema {
  /// <summary>
  ///   Material object.
  ///
  ///   Shamelessly copied from:
  ///   https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs#L1256
  /// </summary>
  [BinarySchema]
  public partial class MObj : IBinaryDeserializable {
    public uint Unk1 { get; set; }
    public uint Unk2 { get; set; }

    public uint TObjOffset { get; set; }

    [Ignore]
    private bool hasTObj_ => this.TObjOffset != 0;

    [RIfBoolean(nameof(hasTObj_))]
    [RAtPosition(nameof(TObjOffset))]
    public TObj? TObj { get; set; }
  
    // TODO: What is this used for, is this another MObj?
    public uint MaterialOffset { get; private set; }

    public uint Unk3 { get; set; }
    public uint Unk4 { get; set; }
  }
}