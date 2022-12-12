using schema;
using schema.attributes.endianness;
using schema.attributes.size;
using System.IO;


namespace bmd.schema.btk {
  [Endianness(Endianness.LittleEndian)]
  [BinarySchema]
  public partial class Btk : IBiSerializable {
    private readonly string magic_ = "J3D1btk1";

    [SizeOfStreamInBytes]
    private uint fileSize_ { get; set; }
  }

  [BinarySchema]
  public partial class Ttk1 : IBiSerializable {
    private readonly string magic_ = "TTK1";
  }
}
