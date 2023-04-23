using System;
using System.Collections.Generic;
using System.IO;

using f3dzex2.io;

using fin.util.enumerables;

using schema.binary.util;

namespace UoT.memory {
  public class OotMemory : IN64Memory {
    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtAddress(
        uint address)
      => this.OpenAtSegmentedAddress(address).Yield();

    public IEndianBinaryReader OpenAtSegmentedAddress(uint address) {
      IoUtils.SplitSegmentedAddress(address,
                                    out var segment,
                                    out var offset);
      throw new NotImplementedException();
    }
  }
}