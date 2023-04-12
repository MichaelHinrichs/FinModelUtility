using f3dzex2.io;

using fin.util.asserts;
using fin.util.enumerables;


namespace Quad64.memory {
  public class Sm64Memory : IN64Memory {
    public byte? AreaId { get; set; }

    public IEnumerable<IEndianBinaryReader> OpenPossibilitiesAtAddress(
        uint address)
      => OpenAtAddress(address).Yield();

    public IEndianBinaryReader OpenAtAddress(uint address) {
      var segment = (byte) (address >> 24);
      var offset = address & 0xFFFFFF;
      var er = new EndianBinaryReader(
          Asserts.CastNonnull(ROM.Instance.getSegment(segment, this.AreaId)),
          Endianness.BigEndian);
      er.Position = offset;
      return er;
    }
  }
}