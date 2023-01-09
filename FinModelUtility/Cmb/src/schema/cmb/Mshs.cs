﻿using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Mshs : IBiSerializable {
    public readonly string magic = "mshs";
    public uint chunkSize;

    private uint meshCount;
    // The remainder are translucent meshes and always packed at the end
    public ushort opaqueMeshCount; 
    public ushort idCount;
    
    // Note: Mesh order = draw order
    [ArrayLengthSource(nameof(meshCount))]
    public Mesh[] meshes;
  }
}
