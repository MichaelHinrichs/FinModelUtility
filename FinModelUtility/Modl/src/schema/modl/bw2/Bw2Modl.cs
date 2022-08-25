using fin.data;
using fin.util.asserts;

using modl.schema.modl.bw2.node;

using schema;


namespace modl.schema.modl.bw2 {
  public class Bw2Modl : IModl, IDeserializable {
    public List<IBwNode> Nodes { get; } = new();
    public ListDictionary<ushort, ushort> CnctParentToChildren { get; } = new();

    public void Read(EndianBinaryReader er) {
      var filenameLength = er.ReadUInt32();
      er.Position += filenameLength;

      er.AssertStringEndian("MODL");

      var size = er.ReadUInt32();
      var expectedEnd = er.Position + size;

      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var version = er.ReadUInt32s(2);

      var nodeCount = er.ReadUInt16();
      var additionalDataCount = er.ReadUInt16();

      var unkInt = er.ReadUInt32();
      var unknown0 = er.ReadSingles(4);

      var bgfNameLength = er.ReadInt32();
      var bgfName = er.ReadString(bgfNameLength);

      var additionalData = er.ReadUInt32s(additionalDataCount);

      er.Endianness = endianness;

      this.SkipSection_(er, "XMEM");

      // Reads in nodes (bones)
      {
        this.Nodes.Clear();
        for (var i = 0; i < nodeCount; ++i) {
          var node = new Bw2Node(additionalDataCount);
          node.Read(er);
          this.Nodes.Add(node);
        }
      }

      // Reads in hierarchy, how nodes are "CoNneCTed" or "CoNCaTenated?"?
      {
        uint cnctCount;
        {
          er.Endianness = Endianness.LittleEndian;
          er.AssertStringEndian("CNCT");
          var cnctSize = er.ReadUInt32();
          cnctCount = cnctSize / 4;
        }

        this.CnctParentToChildren.Clear();
        for (var i = 0; i < cnctCount; ++i) {
          var parent = er.ReadUInt16();
          var child = er.ReadUInt16();

          this.CnctParentToChildren.Add(parent, child);
        }
      }

      Asserts.Equal(expectedEnd, er.Position);
    }

    private void SkipSection_(EndianBinaryReader er, string sectionName) {
      er.AssertStringEndian(sectionName);
      var size = er.ReadUInt32();
      var data = er.ReadBytes((int) size);
    }
  }
}