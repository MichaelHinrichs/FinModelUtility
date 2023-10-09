using fin.data;
using fin.data.dictionaries;
using fin.schema;
using fin.util.asserts;

using modl.schema.modl.bw1.node;

using schema.binary;

namespace modl.schema.modl.bw1 {
  public class Bw1Modl : IModl, IBinaryDeserializable {
    public List<IBwNode> Nodes { get; } = new();
    public ListDictionary<ushort, ushort> CnctParentToChildren { get; } = new();

    [Unknown]
    public void Read(IEndianBinaryReader er) {
      {
        er.PushMemberEndianness(Endianness.LittleEndian);
        var filenameLength = er.ReadUInt32();
        er.Position += filenameLength;
        er.PopEndianness();
      }

      SectionHeaderUtil.AssertNameAndReadSize(er, "MODL", out var size);
      var expectedEnd = er.Position + size;

      er.PushMemberEndianness(Endianness.LittleEndian);
      var nodeCount = er.ReadUInt16();
      var additionalDataCount = er.ReadByte();

      var padding = er.ReadByte();

      var someCount = er.ReadUInt32();
      var unknown0 = er.ReadSingles(4);

      var additionalData = er.ReadUInt32s(additionalDataCount);
      er.PopEndianness();

      this.SkipSection_(er, "XMEM");

      // Reads in nodes (bones)
      {
        this.Nodes.Clear();
        for (var i = 0; i < nodeCount; ++i) {
          var node = new Bw1Node(additionalDataCount);
          node.Read(er);
          this.Nodes.Add(node);
        }
      }

      // Reads in hierarchy, how nodes are "CoNneCTed" or "CoNCaTenated?"?
      {
        er.PushMemberEndianness(Endianness.LittleEndian);
        SectionHeaderUtil.AssertNameAndReadSize(er, "CNCT", out var cnctSize);
        var cnctCount = cnctSize / 4;

        this.CnctParentToChildren.Clear();
        for (var i = 0; i < cnctCount; ++i) {
          var parent = er.ReadUInt16();
          var child = er.ReadUInt16();

          this.CnctParentToChildren.Add(parent, child);
        }
        er.PopEndianness();
      }

      Asserts.Equal(expectedEnd, er.Position);
    }

    private void SkipSection_(IEndianBinaryReader er, string sectionName) {
      SectionHeaderUtil.AssertNameAndReadSize(er, sectionName, out var size);
      var data = er.ReadBytes((int) size);
    }
  }
}