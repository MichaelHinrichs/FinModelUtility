using f3dzex2.io;

using fin.decompression;
using fin.util.asserts;
using fin.util.enumerables;

using schema.binary;

using sm64.schema;


namespace sm64.memory {
  public interface IReadOnlySm64Memory : IReadOnlyN64Memory {
    byte? AreaId { get; }
  }

  public interface ISm64Memory : IN64Memory, IReadOnlySm64Memory {
    new byte? AreaId { get; set; }
  }

  public class Sm64Memory : ISm64Memory {
    public byte? AreaId { get; set; }

    public Endianness Endianness => Endianness.BigEndian;

    public IEnumerable<EndianBinaryReader> OpenPossibilitiesAtSegmentedAddress(
        uint address)
      => this.OpenAtSegmentedAddress(address).Yield();

    public bool TryToOpenPossibilitiesAtSegmentedAddress(uint segmentedAddress,
      out IEnumerable<EndianBinaryReader> possibilities) {
      possibilities = OpenPossibilitiesAtSegmentedAddress(segmentedAddress);
      return true;
    }

    public EndianBinaryReader OpenSegment(Segment segment, uint? offset = null) {
      throw new NotImplementedException();
    }

    public IEnumerable<EndianBinaryReader> OpenPossibilitiesForSegment(
        uint segmentIndex) {
      throw new NotImplementedException();
    }

    public bool IsValidSegment(uint segmentIndex) {
      throw new NotImplementedException();
    }

    public bool IsValidSegmentedAddress(uint segmentedAddress) {
      throw new NotImplementedException();
    }

    public EndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress) {
      IoUtils.SplitSegmentedAddress(segmentedAddress,
                                    out var segment,
                                    out var offset);
      var er = new EndianBinaryReader(
          Asserts.CastNonnull(ROM.Instance.getSegment(segment, this.AreaId)),
          SchemaConstants.SM64_ENDIANNESS);
      er.Position = offset;
      return er;
    }

    public EndianBinaryReader OpenSegment(uint segmentIndex) {
      throw new NotImplementedException();
    }

    public bool IsSegmentCompressed(uint segmentIndex) {
      throw new NotImplementedException();
    }

    public void AddSegment(uint segmentIndex,
                           uint offset,
                           uint length,
                           IDecompressor? decompressor = null) {
      throw new NotImplementedException();
    }

    public void AddSegment(uint segmentIndex, Segment segment) {
      throw new NotImplementedException();
    }
  }
}