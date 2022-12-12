using schema;
using schema.attributes.endianness;
using schema.attributes.size;
using System.IO;


namespace bmd.schema.btk {
  /// <summary>
  ///   BTK files define SRT animations for texture, i.e.
  ///   scale/rotation/translation.
  ///
  ///   https://wiki.cloudmodding.com/tww/BTK
  /// </summary>
  [Endianness(Endianness.BigEndian)]
  [BinarySchema]
  public partial class Btk : IBiSerializable {
    private readonly string magic1_ = "J3D1btk1";

    [SizeOfStreamInBytes]
    private uint fileSize_ { get; set; }

    private readonly uint padding1_ = 1;

    private readonly string magic2_ = "SVR1";

    private readonly uint padding2_ = uint.MaxValue;
    private readonly uint padding3_ = uint.MaxValue;
    private readonly uint padding4_ = uint.MaxValue;

    public Ttk1 Ttk1 { get; } = new();
  }

  [BinarySchema]
  public partial class Ttk1 : IBiSerializable {
    private readonly string magic_ = "TTK1";
  }
}
