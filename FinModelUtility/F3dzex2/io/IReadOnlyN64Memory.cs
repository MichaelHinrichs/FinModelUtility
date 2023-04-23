using System.Collections.Generic;
using System.IO;

using fin.decompression;

namespace f3dzex2.io {
  public interface IReadOnlyN64Memory {
    IEndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress);

    IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtAddress(
        uint segmentedAddress);
  }

  public interface IN64Memory : IReadOnlyN64Memory {
    void SetSegment(uint index,
                    uint offset,
                    uint length,
                    IDecompressor? decompressor = null);
  }
}