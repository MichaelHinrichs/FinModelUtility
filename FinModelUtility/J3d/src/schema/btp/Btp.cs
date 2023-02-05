using schema.binary;
using schema.binary.attributes.endianness;
using System.IO;


namespace j3d.schema.btp {
  /// <summary>
  ///   BTP files define texture-swap animations.
  ///
  ///   https://wiki.cloudmodding.com/tww/BTP
  /// </summary>
  [Endianness(Endianness.BigEndian)]
  [BinarySchema]
  public partial class Btp : IBiSerializable { }
}