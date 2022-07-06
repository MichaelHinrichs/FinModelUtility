using fin.data;
using fin.util.asserts;

using modl.protos.bw1.node;

using schema;


namespace modl.protos.bw1 {
  public class Bw1Model : IDeserializable {
    public List<NodeBw1> Nodes { get; } = new();
    public ListDictionary<ushort, ushort> CnctParentToChildren { get; } = new();

    public void Read(EndianBinaryReader er) {
      var filenameLength = er.ReadUInt32();
      er.Position += filenameLength;

      er.AssertMagicTextEndian("MODL");

      var size = er.ReadUInt32();

      var nodeCount = er.ReadUInt16();
      var additionalDataCount = er.ReadByte();

      var padding = er.ReadByte();

      var unk0 = er.ReadUInt32();
      var floatTuple = er.ReadSingles(4);

      var additionalData = new uint[additionalDataCount];
      for (var i = 0; i < additionalDataCount; ++i) {
        additionalData[i] = er.ReadUInt32();
      }

      this.SkipSection_(er, "XMEM");

      // Reads in nodes (bones)
      {
        this.Nodes.Clear();
        for (var i = 0; i < nodeCount; ++i) {
          var node = new NodeBw1(additionalDataCount);
          node.Read(er);
          this.Nodes.Add(node);
        }
      }

      // Reads in hierarchy, how nodes are "CoNneCTed" or "CoNCaTenated?"?
      {
        er.AssertMagicTextEndian("CNCT");

        var cnctSize = er.ReadUInt32();
        var cnctCount = cnctSize / 4;

        this.CnctParentToChildren.Clear();
        for (var i = 0; i < cnctCount; ++i) {
          var parent = er.ReadUInt16();
          var child = er.ReadUInt16();

          this.CnctParentToChildren.Add(parent, child);
        }
      }
    }

    private void SkipSection_(EndianBinaryReader er, string sectionName) {
      er.AssertMagicTextEndian(sectionName);
      var size = er.ReadUInt32();
      er.Position += size;
    }
  }

  public class NodeBw1 : IDeserializable {
    private int additionalDataCount_;

    public BwTransform Transform { get; } = new();

    public NodeBw1(int additionalDataCount) {
      this.additionalDataCount_ = additionalDataCount;
    }

    public void Read(EndianBinaryReader er) {
      er.AssertMagicTextEndian("NODE");

      var nodeSize = er.ReadUInt32();
      var nodeStart = er.Position;
      var expectedNodeEnd = nodeStart + nodeSize;

      var headerStart = er.Position;
      var expectedHeaderEnd = headerStart + 0x38;
      {
        // TODO: unknown
        er.Position += 12;

        this.Transform.Read(er);

        // TODO: unknown, also transform??
        er.Position += 4 * 4;
      }
      Asserts.Equal(er.Position, expectedHeaderEnd);

      // TODO: additional data
      er.Position += 4 * this.additionalDataCount_;

      er.AssertMagicTextEndian("BBOX");
      er.AssertUInt32(4 * 6);

      // TODO: Handle bounding box

      er.Position += 4 * 6;

      var sectionName = this.ReadMagic_(er);
      var sectionSize = er.ReadInt32();

      while (sectionName != "MATL") {
        if (sectionName == "VSCL") {
          Asserts.Equal(4, sectionSize);
        }

        er.Position += sectionSize;

        sectionName = this.ReadMagic_(er);
        sectionSize = er.ReadInt32();
      }

      Asserts.Equal("MATL", sectionName);
      Asserts.Equal(0, sectionSize % 0x48);

      er.Position += sectionSize;

      while (er.Position < expectedNodeEnd) {
        sectionName = ReadMagic_(er);
        sectionSize = er.ReadInt32();

        er.Position += sectionSize;
      }

      Asserts.Equal(er.Position, expectedNodeEnd);
    }

    private string ReadMagic_(EndianBinaryReader er)
      => new(er.ReadChars(4).Reverse().ToArray());
  }
}