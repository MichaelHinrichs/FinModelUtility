using System.Collections;


namespace gx {
  public class GxVertexDescriptor :
      IEnumerable<(GxVertexAttribute, GxAttributeType?)> {
    public bool HasPosMat { get; set; }
    public bool[] HasTexMats { get; } = new bool[8];
    public GxAttributeType PositionFormat { get; set; }
    public GxAttributeType NormalFormat { get; set; }
    public GxAttributeType[] ColorFormats { get; } = new GxAttributeType[2];
    public bool[] HasTexCoord { get; } = new bool[8];

    public void FromValue(uint value) {
      var posMatFlag = value & 1;
      this.HasPosMat = posMatFlag == 1;
      value >>= 1;

      for (var i = 0; i < 8; ++i) {
        this.HasTexMats[i] = (value & 1) == 1;
        value >>= 1;
      }

      var positionFormatFlag = value & 3;
      this.PositionFormat = (GxAttributeType) positionFormatFlag;
      value >>= 2;

      var normalFormatFlag = value & 3;
      this.NormalFormat = (GxAttributeType) normalFormatFlag;
      value >>= 2;

      for (var i = 0; i < 2; ++i) {
        this.ColorFormats[i] = (GxAttributeType) (value & 3);
        value >>= 2;
      }

      for (var i = 0; i < 8; ++i) {
        this.HasTexCoord[i] = (value & 1) == 1;
        value >>= 1;
      }

      if (value != 0) {
        throw new NotImplementedException();
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<(GxVertexAttribute, GxAttributeType?)> GetEnumerator() {
      if (this.HasPosMat) {
        yield return (GxVertexAttribute.PosMatIdx, null);
      }

      for (var i = 0; i < this.HasTexMats.Length; ++i) {
        if (this.HasTexMats[i]) {
          yield return (GxVertexAttribute.Tex0MatIdx + i, null);
        }
      }

      if (this.PositionFormat != GxAttributeType.NOT_PRESENT) {
        yield return (GxVertexAttribute.Position, this.PositionFormat);
      }
      if (this.NormalFormat != GxAttributeType.NOT_PRESENT) {
        yield return (GxVertexAttribute.Normal, this.NormalFormat);
      }
      for (var i = 0; i < this.ColorFormats.Length; ++i) {
        var colorFormat = this.ColorFormats[i];
        if (colorFormat != GxAttributeType.NOT_PRESENT) {
          yield return (GxVertexAttribute.Color0 + i, colorFormat);
        }
      }

      for (var i = 0; i < this.HasTexCoord.Length; ++i) {
        if (this.HasTexCoord[i]) {
          yield return (GxVertexAttribute.Tex0Coord + i, null);
        }
      }
    }
  }

  public enum GxVertexAttribute {
    PosMatIdx,

    Tex0MatIdx,
    Tex1MatIdx,
    Tex2MatIdx,
    Tex3MatIdx,
    Tex4MatIdx,
    Tex5MatIdx,
    Tex6MatIdx,
    Tex7MatIdx,

    Position,
    Normal,

    Color0,
    Color1,

    Tex0Coord,
    Tex1Coord,
    Tex2Coord,
    Tex3Coord,
    Tex4Coord,
    Tex5Coord,
    Tex6Coord,
    Tex7Coord,
  }
}