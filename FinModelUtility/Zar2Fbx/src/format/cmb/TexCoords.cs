using System.IO;

using schema;

namespace zar.format.cmb {
  public class TexCoords : IDeserializable {
    public TextureMatrixMode matrixMode { get; private set; }
    public byte referenceCameraIndex { get; private set; }
    public TextureMappingType mappingMethod { get; private set; }
    public byte coordinateIndex { get; private set; }
    public float[] scale { get; } = new float[2];
    public float rotation { get; private set; }
    public float[] translation { get; } = new float[2];

    public void Read(EndianBinaryReader r) {
      this.matrixMode = (TextureMatrixMode) r.ReadByte();
      this.referenceCameraIndex = r.ReadByte();
      this.mappingMethod = (TextureMappingType) r.ReadByte();
      this.coordinateIndex = r.ReadByte();
      r.ReadSingles(this.scale);
      this.rotation = r.ReadSingle();
      r.ReadSingles(this.translation);
    }
  }
}