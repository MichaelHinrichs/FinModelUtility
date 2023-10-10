using schema.text;
using schema.text.reader;

namespace pmdc.schema.omd {
  public class Omd : ITextDeserializable {
    public OmdMaterial[] Materials { get; set; }
    public OmdMesh[] Meshes { get; set; }

    public void Read(ITextReader tr) {
      var something = tr.ReadInt32();

      var materialCount = tr.ReadInt32();
      tr.ReadLine();
      this.Materials = tr.ReadNewArray<OmdMaterial>(materialCount);

      var meshCount = tr.ReadInt32();
      tr.ReadLine();
      this.Meshes = tr.ReadNewArray<OmdMesh>(meshCount);
    }
  }
}