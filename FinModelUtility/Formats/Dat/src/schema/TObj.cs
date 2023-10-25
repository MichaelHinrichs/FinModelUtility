using schema.binary;

namespace dat.schema {
  /// <summary>
  ///   Texture object.
  ///
  ///   Shamelessly copied from:
  ///   https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs#L1281
  ///   https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/LibWii/TLP.cs#L166
  /// </summary>
  [BinarySchema]
  public partial class TObj : IBinaryDeserializable {
    private readonly int[] unk_ = new int[13];

    public void Read(IBinaryReader br) { }
  }
}