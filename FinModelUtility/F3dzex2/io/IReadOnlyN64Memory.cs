using System.Collections.Generic;
using System.IO;

using fin.decompression;

namespace f3dzex2.io {
  public interface IReadOnlyN64Memory {
    IEndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress);

    IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtAddress(
        uint segmentedAddress);

    bool IsValidSegment(uint segmentIndex);
    bool IsValidSegmentedAddress(uint segmentedAddress);
    IEndianBinaryReader OpenSegment(uint segmentIndex);
    bool IsSegmentCompressed(uint segmentIndex);
    uint GetSegmentLength(uint segmentIndex);
  }

  public interface IN64Memory : IReadOnlyN64Memory {
    void SetSegment(uint segmentIndex,
                    uint offset,
                    uint length,
                    IDecompressor? decompressor = null);
  }
}