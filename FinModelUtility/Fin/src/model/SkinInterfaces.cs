using System.Collections.Generic;

using fin.math.matrix;
using fin.model.impl;

namespace fin.model {
  public interface ISkin {
    IReadOnlyList<IVertex> Vertices { get; }
    IVertex AddVertex(IPosition position);
    IVertex AddVertex(float x, float y, float z);

    IReadOnlyList<IMesh> Meshes { get; }
    IMesh AddMesh();
  }

  public interface IMesh {
    string Name { get; set; }

    IReadOnlyList<IPrimitive> Primitives { get; }

    IPrimitive AddTriangles(params (IVertex, IVertex, IVertex)[] triangles);
    IPrimitive AddTriangles(params IVertex[] vertices);

    IPrimitive AddTriangleStrip(params IVertex[] vertices);
    IPrimitive AddTriangleFan(params IVertex[] vertices);

    IPrimitive AddQuads(params (IVertex, IVertex, IVertex, IVertex)[] quads);
    IPrimitive AddQuads(params IVertex[] vertices);
  }

  public record BoneWeight(
      IBone Bone,
      // TODO: This should be moved to the bone interface instead.
      IReadOnlyFinMatrix4x4 SkinToBone,
      float Weight);

  public interface ITexCoord {
    float U { get; }
    float V { get; }
  }

  public interface IVertex {
    // TODO: Allow caching vertex builders directly on this type.

    int Index { get; }

    IReadOnlyList<BoneWeight>? Weights { get; }
    bool Preproject { get; set; }

    IVertex SetBone(IBone bone);
    IVertex SetBones(params BoneWeight[] weights);

    IPosition LocalPosition { get; }
    IVertex SetLocalPosition(IPosition localPosition);
    IVertex SetLocalPosition(float x, float y, float z);

    INormal? LocalNormal { get; }
    IVertex SetLocalNormal(INormal? localNormal);
    IVertex SetLocalNormal(float x, float y, float z);

    ITangent? LocalTangent { get; }
    IVertex SetLocalTangent(ITangent? localNormal);
    IVertex SetLocalTangent(float x, float y, float z, float w);

    IVertexAttributeArray<IColor>? Colors { get; }
    IVertex SetColor(IColor? color);
    IVertex SetColor(int colorIndex, IColor? color);
    IVertex SetColorBytes(byte r, byte g, byte b, byte a);
    IVertex SetColorBytes(int colorIndex, byte r, byte g, byte b, byte a);
    IColor? GetColor();
    IColor? GetColor(int colorIndex);

    IVertexAttributeArray<ITexCoord>? Uvs { get; }
    IVertex SetUv(ITexCoord? uv);
    IVertex SetUv(float u, float v);
    IVertex SetUv(int uvIndex, ITexCoord? uv);
    IVertex SetUv(int uvIndex, float u, float v);
    ITexCoord? GetUv();
    ITexCoord? GetUv(int uvIndex);
  }

  public enum PrimitiveType {
    TRIANGLES,
    TRIANGLE_STRIP,
    TRIANGLE_FAN,
    QUADS,
    // TODO: Other types.
  }

  public interface IPrimitive {
    PrimitiveType Type { get; }
    IReadOnlyList<IVertex> Vertices { get; }

    IMaterial Material { get; }
    IPrimitive SetMaterial(IMaterial material);
  }
}