using System.Collections.Generic;
using System.IO;

namespace f3dzex2.io {
  public interface IN64Memory {
    IEndianBinaryReader OpenAtAddress(uint address);
    IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtAddress(uint address);
  }
}
