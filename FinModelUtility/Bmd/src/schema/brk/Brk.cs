using schema;


namespace bmd.schema.btk {
  /// <summary>
  ///   BRK files define color register animations.
  ///
  ///   https://wiki.cloudmodding.com/tww/BRK
  /// </summary>
  [BinarySchema]
  public partial class Brk : IBiSerializable { }
}