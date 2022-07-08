using System.IO;

using schema;

namespace zar.schema.cmb {
  public class Vatr : IDeserializable {
    public uint chunkSize;
    // i.e., vertex count of model
    public uint maxIndex;

    // Basically just used to get each attibute into it's own byte[] (We won't
    // be doing that here)
    public readonly AttributeSlice position = new();
    public readonly AttributeSlice normal = new();
    public readonly AttributeSlice tangent = new();
    public readonly AttributeSlice color = new();
    public readonly AttributeSlice uv0 = new();
    public readonly AttributeSlice uv1 = new();
    public readonly AttributeSlice uv2 = new();
    public readonly AttributeSlice bIndices = new();
    public readonly AttributeSlice bWeights = new();

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("vatr");

      this.chunkSize = r.ReadUInt32();
      this.maxIndex = r.ReadUInt32();

      this.position.Read(r);
      this.normal.Read(r);
      if (CmbHeader.Version > CmbVersion.OCARINA_OF_TIME_3D) {
        this.tangent.Read(r);
      }

      this.color.Read(r);
      this.uv0.Read(r);
      this.uv1.Read(r);
      this.uv2.Read(r);
      this.bIndices.Read(r);
      this.bWeights.Read(r);
    }
  }
}