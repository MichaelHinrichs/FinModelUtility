using System.Linq;

using fin.util.enumerables;

using schema;
using schema.attributes.child_of;
using schema.attributes.ignore;
using schema.attributes.offset;
using schema.attributes.position;


namespace bmd.schema.bmd {
  [BinarySchema]
  public partial class StringTable : IBiSerializable {
    [Position]
    public long BasePosition { get; set; }

    private ushort NrStrings;
    public ushort Padding;

    [ArrayLengthSource(nameof(NrStrings))]
    public StringTableEntry[] Entries;

    [Ignore]
    public string this[int index] => this.Entries[index].Entry;

    [Ignore]
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