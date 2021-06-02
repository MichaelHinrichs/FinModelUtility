using System.Collections.Generic;

using MathNet.Numerics.LinearAlgebra;

namespace fin.model {
  public interface ISkin {
    IReadOnlyList<IVertex> Vertices { get; }
    IVertex AddVertex(float x, float y, float z);

    IReadOnlyList<IPrimitive> Primitives { get; }

    IPrimitive AddTriangles(params (IVertex, IVertex, IVertex)[] triangles);
    IPrimitive AddTriangles(params IVertex[] vertices);

    IPrimitive AddTriangleStrip(params IVertex[] vertices);

    IPrimitive AddQuads(params (IVertex, IVertex, IVertex, IVertex)[] quads);
    IPrimitive AddQuads(params IVertex[] vertices);
  }

  public record BoneWeight(
      IBone Bone,
      Matrix<double> SkinToBone,
      float Weight);

  public interface IUv {
    float U { get; set; }
    float V { get; set; }
  }

  public interface IVertex {
    // TODO: Allow caching vertex builders directly on this type.

    IReadOnlyList<BoneWeight>? Weights { get; }
    IVertex SetBone(IBone bone);
    IVertex SetBones(params BoneWeight[] weights);

    IPosition LocalPosition { get; }
    IVertex SetLocalPosition(float x, float y, float z);

    INormal? LocalNormal { get; }
    IVertex SetLocalNormal(float x, float y, float z);

    IColor? Color { get; }
    IVertex SetColor(byte r, byte g, byte b);

    IReadOnlyList<IUv>? Uvs { get; }
    IVertex SetUv(float u, float v);
    IVertex SetUv(int uvIndex, float u, float v);
  }

  public enum PrimitiveType {
    TRIANGLES,
    TRIANGLE_STRIP,
    QUADS,
    // TODO: Other types.
  }

  public interface IPrimitive {
    PrimitiveType Type { get; }
    IReadOnlyList<IVertex> Vertices { get; }

    IPrimitive SetMaterial(IMaterial material);
  }
}