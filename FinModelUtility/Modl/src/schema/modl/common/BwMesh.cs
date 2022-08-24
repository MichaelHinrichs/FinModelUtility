namespace modl.schema.modl.common {
  public class BwMesh {
    public uint Flags { get; set; }
    public uint MaterialIndex { get; set; }
    public List<BwTriangleStrip> TriangleStrips { get; set; }
  }

  public class BwTriangleStrip {
    public List<BwVertexAttributeIndices> VertexAttributeIndicesList {
      get;
      set;
    }
  }

  public class BwVertexAttributeIndices {
    public double Fraction { get; set; }
    public ushort PositionIndex { get; set; }
    public ushort? NormalIndex { get; set; }
    public int? NodeIndex { get; set; }
    public ushort?[] TexCoordIndices { get; } = new ushort?[8];
  }
}
