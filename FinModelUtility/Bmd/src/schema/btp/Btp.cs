using schema;
using schema.attributes.endianness;
using System.IO;


namespace bmd.schema.btp {
  /// <summary>
  ///   BTP files define texture-swap animations.
  ///
  ///   https://wiki.cloudmodding.com/tww/BTP
  /// </summary>
  [Endianness(Endianness.BigEndian)]
  [BinarySchema]
  public partial class Btp : IBiSerializable { }
}