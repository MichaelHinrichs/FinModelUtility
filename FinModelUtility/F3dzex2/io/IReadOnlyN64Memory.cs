using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.data;
using fin.decompression;
using fin.util.hex;

using schema.binary.util;


namespace f3dzex2.io {
  public interface IReadOnlyN64Memory {
    Endianness Endianness { get; }
    IEndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress);

    IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtSegmentedAddress(
        uint segmentedAddress);

    bool TryToOpenPossibilitiesAtSegmentedAddress(
        uint segmentedAddress,
        out IEnumerable<IEndianBinaryReader> possibilities);

    IEndianBinaryReader OpenSegment(uint segmentIndex);
    IEndianBinaryReader OpenSegment(Segment segment, uint? offset = null);

    IEnumerable<IEndianBinaryReader> OpenPossibilitiesForSegment(
        uint segmentIndex);

    bool IsValidSegment(uint segmentIndex);
    bool IsValidSegmentedAddress(uint segmentedAddress);
    bool IsSegmentCompressed(uint segmentIndex);
  }

  public interface IN64Memory : IReadOnlyN64Memory {
    void AddSegment(uint segmentIndex,
                    uint offset,
                    uint length,
                    IDecompressor? decompressor = null);

    void AddSegment(uint segmentIndex, Segment segment);
  }

  public class N64Memory : IN64Memory {
    private readonly byte[] bytes_;
    private readonly ListDictionary<uint, Segment> segments_ = new();

    public N64Memory(byte[] data, Endianness endianness) {
      this.bytes_ = data;
      this.Endianness = endianness;
    }

    public Endianness Endianness { get; }

    public IEndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress)
      => this.OpenPossibilitiesAtSegmentedAddress(segmentedAddress).Single();

    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtSegmentedAddress(
        uint segmentedAddress) {
      Asserts.True(
          this.TryToOpenPossibilitiesAtSegmentedAddress(
              segmentedAddress,
              out var possibilities),
          $"Expected 0x{segmentedAddress.ToHex()} to be a valid segmented address.");
      return possibilities;
    }

    public bool TryToOpenPossibilitiesAtSegmentedAddress(
        uint segmentedAddress,
        out IEnumerable<IEndianBinaryReader> possibilities) {
      if (!this.TryToGetSegmentsAtSegmentedAddress_(
              segmentedAddress,
              out var offset,
              out var validSegments)) {
        possibilities = default;
        return false;
      }

      possibilities =
          validSegments.Select(segment => this.OpenSegment(segment, offset));
      return true;
    }

    public IEndianBinaryReader OpenSegment(uint segmentIndex)
      => this.OpenPossibilitiesForSegment(segmentIndex).Single();

    public IEndianBinaryReader OpenSegment(Segment segment,
                                           uint? offset = null) {
      var er = new EndianBinaryReader(
          new MemoryStream(this.bytes_,
                           (int) segment.Offset,
                           (int) segment.Length),
          this.Endianness);
      if (offset != null) {
        er.Position = offset.Value;
      }

      return er;
    }

    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesForSegment(
        uint segmentIndex)
      => this
         .segments_[segmentIndex]
         .Select(segment => this.OpenSegment(segment));


    public bool IsValidSegment(uint segmentIndex)
      => this.segments_.HasList(segmentIndex);

    public bool IsValidSegmentedAddress(uint segmentedAddress) {
      IoUtils.SplitSegmentedAddress(segmentedAddress,
                                    out var segmentIndex,
                                    out var offset);
      if (!this.segments_.TryGetList(segmentIndex, out var segments)) {
        return false;
      }

      var offsetInSegment = offset;
      return segments!.Any(segment => offsetInSegment < segment.Length);
    }

    public bool IsSegmentCompressed(uint segmentIndex)
      => this.segments_[segmentIndex].Single().Decompressor != null;

    public void AddSegment(uint segmentIndex,
                           uint offset,
                           uint length,
                           IDecompressor? decompressor = null)
      => this.AddSegment(segmentIndex,
                         new Segment {
                             Offset = offset,
                             Length = length,
                             Decompressor = decompressor,
                         });

    public void AddSegment(uint segmentIndex, Segment segment)
      => this.segments_.Add(segmentIndex, segment);

    private bool TryToGetSegmentsAtSegmentedAddress_(
        uint segmentedAddress,
        out uint offset,
        out IEnumerable<Segment> validSegments) {
      IoUtils.SplitSegmentedAddress(segmentedAddress,
                                    out var segmentIndex,
                                    out offset);
      var offsetInSegment = offset;

      if (!this.segments_.TryGetList(segmentIndex, out var segments)) {
        validSegments = default;
        return false;
      }

      validSegments =
          segments!.Where(segment => offsetInSegment < segment.Length);
      return segments!.Any();
    }
  }

  public readonly struct Segment {
    public required uint Offset { get; init; }
    public required uint Length { get; init; }
    public IDecompressor? Decompressor { get; init; }
  }
}