using System.Collections;

namespace gx {
  public struct GxVertexDescriptor :
      IEnumerable<(GxVertexAttribute, GxAttributeType?)> {
    public GxVertexDescriptor(uint value) {
      this.Value = value;
    }

    public uint Value { get; }

    IEnumerator IEnumerable.GetEnumerator()
      => this.cachedEnumerator_ ??= this.GetEnumerator();

    private IEnumerator<(GxVertexAttribute, GxAttributeType?)>?
        cachedEnumerator_;

    public IEnumerator<(GxVertexAttribute, GxAttributeType?)> GetEnumerator() {
      // Read flags from value
      var value = this.Value;

      var posMatFlag = value & 1;
      var hasPosMat = posMatFlag == 1;
      value >>= 1;

      byte hasTexMatBits = 0;
      for (var i = 0; i < 8; ++i) {
        hasTexMatBits |= (byte) ((value & 1) << i);
        value >>= 1;
      }

      var positionFormatFlag = value & 3;
      var positionFormat = (GxAttributeType) positionFormatFlag;
      value >>= 2;

      var normalFormatFlag = value & 3;
      var normalFormat = (GxAttributeType) normalFormatFlag;
      value >>= 2;

      var colorFormat0 = (GxAttributeType) (value & 3);
      value >>= 2;
      var colorFormat1 = (GxAttributeType) (value & 3);
      value >>= 2;

      byte hasTexCoordBits = 0;
      for (var i = 0; i < 8; ++i) {
        hasTexCoordBits |= (byte) ((value & 1) << i);
        value >>= 1;
      }

      if (value != 0) {
        throw new NotImplementedException();
      }


      // Generate enumerator
      if (hasPosMat) {
        yield return (GxVertexAttribute.PosMatIdx, null);
      }

      for (var i = 0; i < 8; ++i) {
        if (((hasTexMatBits >> i) & 1) != 0) {
          yield return (GxVertexAttribute.Tex0MatIdx + i, null);
        }
      }

      if (positionFormat != GxAttributeType.NOT_PRESENT) {
        yield return (GxVertexAttribute.Position, positionFormat);
      }

      if (normalFormat != GxAttributeType.NOT_PRESENT) {
        yield return (GxVertexAttribute.Normal, normalFormat);
      }

      if (colorFormat0 != GxAttributeType.NOT_PRESENT) {
        yield return (GxVertexAttribute.Color0, colorFormat0);
      }
      if (colorFormat1 != GxAttributeType.NOT_PRESENT) {
        yield return (GxVertexAttribute.Color1, colorFormat1);
      }

      for (var i = 0; i < 8; ++i) {
        if (((hasTexCoordBits >> i) & 1) != 0) {
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