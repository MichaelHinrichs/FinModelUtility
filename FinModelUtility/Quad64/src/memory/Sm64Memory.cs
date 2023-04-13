using f3dzex2.io;

using fin.util.asserts;
using fin.util.enumerables;

using sm64.scripts.geo;

namespace Quad64.memory {
  public interface IReadOnlySm64Memory : IN64Memory {
    byte? AreaId { get; }
  }

  public interface ISm64Memory : IReadOnlySm64Memory {
    new byte? AreaId { get; set; }
  }

  public class Sm64Memory : ISm64Memory {
    public byte? AreaId { get; set; }

    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtAddress(
        uint address)
      => this.OpenAtSegmentedAddress(address).Yield();

    public IEndianBinaryReader OpenAtSegmentedAddress(uint address) {
      GeoUtils.SplitAddress(address,
                            out var segment,
                            out var offset);
      var er = new EndianBinaryReader(
          Asserts.CastNonnull(ROM.Instance.getSegment(segment, this.AreaId)),
          Endianness.BigEndian);
      er.Position = offset;
      return er;
    }
  }
}