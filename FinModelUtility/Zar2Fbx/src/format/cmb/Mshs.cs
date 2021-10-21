using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Mshs : IDeserializable {
    public uint chunkSize;
    // The remainder are translucent meshes and always packed at the end
    public ushort opaqueMeshCount; 
    public ushort idCount;
    // Note: Mesh order = draw order
    public Mesh[] meshes;

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("mshs");

      this.chunkSize = r.ReadUInt32();
      this.meshes = new Mesh[r.ReadUInt32()];
      this.opaqueMeshCount = r.ReadUInt16();
      this.idCount = r.ReadUInt16();

      for (var i = 0; i < this.meshes.Length; ++i) {
        var mesh = new Mesh();
        mesh.Read(r);
        this.meshes[i] = mesh;
      }
    }
  }
}
