using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class TexCoords : IDeserializable {
    public TextureMatrixMode matrixMode;
    public byte referenceCameraIndex;
    public TextureMappingType mappingMethod;
    public byte coordinateIndex;
    public readonly float[] scale = new float[2];
    public float rotation;
    public readonly float[] translation = new float[2];

    public void Read(EndianBinaryReader r) {
      this.matrixMode = (TextureMatrixMode) r.ReadByte();
      this.referenceCameraIndex = r.ReadByte();
      this.mappingMethod = (TextureMappingType) r.ReadByte();
      this.coordinateIndex = r.ReadByte();
      r.ReadSingles(this.scale, 2);
      this.rotation = r.ReadSingle();
      r.ReadSingles(this.translation, 2);
    }
  }
}