using System;
using System.IO;

using schema;

namespace cmb.schema.cmb {
  [Schema]
  public partial class Texture : IBiSerializable {
    public uint dataLength { get; private set; }
    public ushort mimapCount { get; private set; }

    [Format(SchemaNumberType.BYTE)]
    public bool isEtc1 { get; private set; }

    [Format(SchemaNumberType.BYTE)]
    public bool isCubemap { get; private set; }

    public ushort width { get; private set; }
    public ushort height { get; private set; }

    public GlTextureFormat imageFormat { get; private set; }

    public uint dataOffset { get; private set; }

    [StringLengthSource(0x10)]
    public string name { get; private set; }
  }
}