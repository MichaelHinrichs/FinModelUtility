using System.Linq;

using fin.util.enumerables;

using schema.binary;
using schema.binary.attributes.child_of;
using schema.binary.attributes.ignore;
using schema.binary.attributes.offset;
using schema.binary.attributes.position;


namespace j3d.schema.bmd {
  [BinarySchema]
  public partial class StringTable : IBiSerializable {
    [PositionRelativeToStream]
    public long BasePosition { get; set; }

    private ushort NrStrings;
    private readonly ushort padding_ = ushort.MaxValue;

    [ArrayLengthSource(nameof(NrStrings))]
    public StringTableEntry[] Entries;

    public string this[int index] => this.Entries[index].Entry;

    public int this[string value] =>
        this.Entries.Select(entry => entry.Entry).IndexOfOrNegativeOne(value);
  }

  [BinarySchema]
  public partial class
      StringTableEntry : IBiSerializable, IChildOf<StringTable> {
    public StringTable Parent { get; set; }

    [Ignore]
    public long BasePosition => Parent.BasePosition;

    public ushort Unknown;
    private ushort Offset;

    [Offset(nameof(BasePosition), nameof(Offset))]
    [NullTerminatedString]
    public string Entry;

    public override string ToString() => this.Entry;
  }
}