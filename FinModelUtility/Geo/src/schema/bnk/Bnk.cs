﻿using schema.binary;

namespace geo.schema.bnk {
  public class Bnk : IBinaryDeserializable {
    public void Read(IEndianBinaryReader er) {
      er.Position = 0x24;
      var animationCount = er.ReadUInt32();
    }
  }
}
