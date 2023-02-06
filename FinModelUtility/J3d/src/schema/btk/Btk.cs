using schema.binary;
using schema.binary.attributes.endianness;
using schema.binary.attributes.size;
using System.IO;


namespace j3d.schema.btk {
  /// <summary>
  ///   BTK files define SRT animations for texture, i.e.
  ///   scale/rotation/translation.
  ///
  ///   https://wiki.cloudmodding.com/tww/BTK
  /// </summary>
  [Endianness(Endianness.BigEndian)]
  [BinarySchema]
  public partial class Btk : IBinaryConvertible {
    private readonly string magic1_ = "J3D1btk1";

    [SizeOfStreamInBytes]
    private uint fileSize_;

    private readonly uint padding1_ = 1;

    private readonly string magic2_ = "SVR1";

    private readonly uint padding2_ = uint.MaxValue;
    private readonly uint padding3_ = uint.MaxValue;
    private readonly uint padding4_ = uint.MaxValue;

    public Ttk1 Ttk1 { get; } = new();
  }

  [BinarySchema]
  public partial class Ttk1 : IBinaryConvertible {
    private readonly string magic_ = "TTK1";
  }
}
