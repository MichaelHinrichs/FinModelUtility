using System.IO;

using schema;

namespace cmb.schema.cmb {
  public class Sepd : IDeserializable {
    public uint chunkSize;
    public ushort primSetCount;

    /**
      Bit Flags: (HasTangents was added in versions > OoT:3D (aka 6))
         HasPosition : 00000001
         HasNormals  : 00000010
         HasTangents : 00000100 (MM3D/LM3D/EO only)
         HasColors   : 00000100
         HasUV0      : 00001000
         HasUV1      : 00010000
         HasUV2      : 00100000
         HasIndices  : 01000000
         HasWeights  : 10000000
     */
    public ushort vertFlags;

    public float[] meshCenter = new float[3];
    public float[] positionOffset = new float[3];

    // Min coordinate of the shape
    public float[] min = new float[3];

    // Max coordinate of the shape
    public float[] max = new float[3];

    public readonly VertexAttribute position = new();
    public readonly VertexAttribute normal = new();
    public readonly VertexAttribute tangents = new();
    public readonly VertexAttribute color = new();
    public readonly VertexAttribute uv0 = new();
    public readonly VertexAttribute uv1 = new();
    public readonly VertexAttribute uv2 = new();
    public readonly VertexAttribute bIndices = new();
    public readonly VertexAttribute bWeights = new();

    // How many weights each vertex has for this shape
    public ushort boneDimensions;

    /**
      Note: Constant values are set in "VertexAttribute" (Use constants instead of an array to save space, assuming all values are the same)
        #Bit Flags:
        # PositionUseConstant : 00000001
        # NormalsUseConstant  : 00000010
        # TangentsUseConstant : 00000100 (MM3D/LM3D/EO only)
        # ColorsUseConstant   : 00000100
        # UV0UseConstant      : 00001000
        # UV1UseConstant      : 00010000
        # UV2UseConstant      : 00100000
        # IndicesUseConstant  : 01000000
        # WeightsUseConstant  : 10000000
    */
    public ushort constantFlags;

    public PrimitiveSet[] primitiveSets;

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("sepd");

      this.chunkSize = r.ReadUInt32();
      this.primSetCount = r.ReadUInt16();

      this.vertFlags = r.ReadUInt16();

      r.ReadSingles(this.meshCenter);
      r.ReadSingles(this.positionOffset);

      if (CmbHeader.Version > CmbVersion.EVER_OASIS) {
        r.ReadSingles(this.min);
        r.ReadSingles(this.max);
      }

      this.position.Read(r);
      this.normal.Read(r);

      if (CmbHeader.Version > CmbVersion.OCARINA_OF_TIME_3D) {
        this.tangents.Read(r);
      }

      this.color.Read(r);
      this.uv0.Read(r);
      this.uv1.Read(r);
      this.uv2.Read(r);
      this.bIndices.Read(r);
      this.bWeights.Read(r);

      this.boneDimensions = r.ReadUInt16();
      this.constantFlags = r.ReadUInt16();

      for (var i = 0; i < this.primSetCount; ++i) {
        r.ReadInt16(); // PrimitiveSetOffset(s)
      }

      r.Align(4);
      this.primitiveSets = new PrimitiveSet[this.primSetCount];
      for (var i = 0; i < this.primSetCount; ++i) {
        var primitiveSet = new PrimitiveSet();
        primitiveSet.Read(r);
        this.primitiveSets[i] = primitiveSet;
      }
    }
  }
}