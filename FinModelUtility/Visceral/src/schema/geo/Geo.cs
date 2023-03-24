using schema.binary;

namespace visceral.schema.geo {
  public class Geo : IBinaryDeserializable {
    public string ModelName { get; set; }

    public IReadOnlyList<Mesh> Meshes { get; set; }

    public void Read(IEndianBinaryReader er) {
      er.AssertString("MGAE");

      er.Position = 0x20;
      this.ModelName = er.ReadStringNTAtOffset(er.ReadUInt32());

      er.Position = 0x34;
      var meshCount = er.ReadUInt32();
      var unk4 = er.ReadUInt32();
      er.Position += 0xc;

      var unkTableCount = er.ReadUInt32();
      var unkTableOffset = er.ReadUInt32();

      var dataTable1 = er.ReadUInt32();
      var unkTable2 = er.ReadUInt32();
      er.Position += 0x8;

      var unkBuffer = er.ReadUInt32();
      var dataTable2 = er.ReadUInt32();
      var unk3 = er.ReadUInt32();

      er.Position = 0x9c;
      var uvOffsetLocal = er.ReadUInt32();

      var meshes = new List<Mesh>();

      for (var i = 0; i < meshCount; i++) {
        er.Position = dataTable1 + 0xC0 * i;

        var meshName = er.ReadStringNTAtOffset(er.ReadUInt32());

        er.Position += 0x4;
        er.Position += 0x4;
        er.Position += 0x14;

        var tableOffset = er.ReadUInt32();
        var unk2 = er.ReadUInt16();

        var b = er.ReadByte();

        var uvSize = er.ReadByte();

        er.Position += 0x8;

        var faceCount = er.ReadUInt32();
        er.Position += 0x4;

        var vertCount = er.ReadUInt16();
        var unkCount = er.ReadUInt16();

        var lodType = er.ReadUInt16();
        er.Position += 0x4;
        er.Position += 0x2;
        er.Position += 0x4;
        er.Position += 0x1c;

        var vertSize = er.ReadUInt32();
        var unk6 = er.ReadUInt32();
        var vertId = er.ReadUInt32();

        er.Position += 0xc;
        er.Position += 0x8;

        var vertOffset = er.ReadUInt32();
        var faceOffset = er.ReadUInt32();

        er.Position += 0x34;
        
        ;

        meshes.Add(new Mesh() {Name = meshName});
      }

      this.Meshes = meshes;

      ;
    }

    public class Mesh {
      public string Name { get; set; }
    }
  }
}
