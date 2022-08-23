﻿using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class DepthFunction : IDeserializable {
    public byte Enable;
    public byte Func;
    public byte UpdateEnable;
    private readonly byte padding_ = 0xff;
  }
}
