using schema.binary;

namespace jsystem.schema.btk {
  /// <summary>
  ///   BRK files define color register animations.
  ///
  ///   https://wiki.cloudmodding.com/tww/BRK
  /// </summary>
  [BinarySchema]
  public partial class Brk : IBinaryConvertible { }
}