using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;

using fin.data;
using fin.decompression;
using fin.util.enumerables;


namespace f3dzex2.io {
  public interface IReadOnlyN64Memory {
    IEndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress);

    IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtSegmentedAddress(
        uint segmentedAddress);

    IEndianBinaryReader OpenSegment(uint segmentIndex);

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
  }

  public class N64Memory : IN64Memory {
    private readonly byte[] data_;
    private readonly Endianness endianness_;
    private readonly ListDictionary<uint, Segment> segments_ = new();

    public N64Memory(byte[] data, Endianness endianness) {
      this.data_ = data;
      this.endianness_ = endianness;
    }

    public IEndianBinaryReader OpenAtSegmentedAddress(uint segmentedAddress)
      => this.OpenPossibilitiesAtSegmentedAddress(segmentedAddress).Single();

    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtSegmentedAddress(
        uint segmentedAddress)
      => this
         .GetSegmentsAtSegmentedAddress_(segmentedAddress, out var offset)
         .Select(segment => {
           var memoryStream =
               new MemoryStream(this.data_,
                                (int) segment.Offset,
                                (int) segment.Length);
           var er = new EndianBinaryReader(memoryStream, this.endianness_);
           er.Position = offset;
           return er;
         });

    public IEndianBinaryReader OpenSegment(uint segmentIndex)
      => this.OpenPossibilitiesForSegment(segmentIndex).Single();

    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesForSegment(
        uint segmentIndex)
      => this
         .segments_[segmentIndex]
         .Select(segment => {
           var memoryStream =
               new MemoryStream(this.data_,
                                (int) segment.Offset,
                                (int) segment.Length);
           var er = new EndianBinaryReader(memoryStream, this.endianness_);
           return er;
         });


    public bool IsValidSegment(uint segmentIndex)
      => this.segments_.HasList(segmentIndex);

    public bool IsValidSegmentedAddress(uint segmentedAddress)
      => this.GetSegmentsAtSegmentedAddress_(segmentedAddress, out _)
             .Any();

    public bool IsSegmentCompressed(uint segmentIndex)
      => this.segments_[segmentIndex].Single().Decompressor != null;

    public void AddSegment(uint segmentIndex,
                           uint offset,
                           uint length,
                           IDecompressor? decompressor = null)
      => this.segments_.Add(segmentIndex,
                            new Segment {
                                Offset = offset,
                                Length = length,
                                Decompressor = decompressor,
                            });

    private IEnumerable<Segment> GetSegmentsAtSegmentedAddress_(
        uint segmentedAddress,
        out uint offset) {
      IoUtils.SplitSegmentedAddress(segmentedAddress,
                                    out var segmentIndex,
                                    out offset);
      var offsetInSegment = offset;
      var segments = this.segments_[segmentIndex];
      return segments.Where(segment => segment.Length < offsetInSegment);
    }

    private class Segment {
      public uint Offset { get; set; }
      public uint Length { get; set; }
      public IDecompressor? Decompressor { get; set; }
    }
  }
}