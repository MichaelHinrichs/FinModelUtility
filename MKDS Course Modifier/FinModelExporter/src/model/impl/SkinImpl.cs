using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using fin.math.matrix;

namespace fin.model.impl {
  public partial class ModelImpl {
    public ISkin Skin { get; } = new SkinImpl();

    private class SkinImpl : ISkin {
      private readonly IList<IVertex> vertices_ = new List<IVertex>();
      private readonly IList<IPrimitive> primitives_ = new List<IPrimitive>();

      public SkinImpl() {
        this.Vertices = new ReadOnlyCollection<IVertex>(this.vertices_);
        this.Primitives = new ReadOnlyCollection<IPrimitive>(this.primitives_);
      }

      public IReadOnlyList<IVertex> Vertices { get; }

      public IVertex AddVertex(float x, float y, float z) {
        var vertex = new VertexImpl(this.vertices_.Count, x, y, z);
        this.vertices_.Add(vertex);
        return vertex;
      }

      public IReadOnlyList<IPrimitive> Primitives { get; }

      public IPrimitive AddTriangles(
          params (IVertex, IVertex, IVertex)[] triangles) {
        var vertices = new IVertex[3 * triangles.Length];
        for (var i = 0; i < triangles.Length; ++i) {
          var triangle = triangles[i];
          vertices[3 * i] = triangle.Item1;
          vertices[3 * i + 1] = triangle.Item2;
          vertices[3 * i + 2] = triangle.Item3;
        }
        return this.AddTriangles(vertices);
      }

      public IPrimitive AddTriangles(params IVertex[] vertices) {
        Debug.Assert(vertices.Length % 3 == 0);
        var primitive = new PrimitiveImpl(PrimitiveType.TRIANGLES, vertices);
        this.primitives_.Add(primitive);
        return primitive;
      }

      public IPrimitive AddTriangleStrip(params IVertex[] vertices) {
        var primitive =
            new PrimitiveImpl(PrimitiveType.TRIANGLE_STRIP, vertices);
        this.primitives_.Add(primitive);
        return primitive;
      }

      public IPrimitive AddTriangleFan(params IVertex[] vertices) {
        var primitive =
            new PrimitiveImpl(PrimitiveType.TRIANGLE_FAN, vertices);
        this.primitives_.Add(primitive);
        return primitive;
      }

      public IPrimitive AddQuads(
          params (IVertex, IVertex, IVertex, IVertex)[] quads) {
        var vertices = new IVertex[4 * quads.Length];
        for (var i = 0; i < quads.Length; ++i) {
          var quad = quads[i];
          vertices[4 * i] = quad.Item1;
          vertices[4 * i + 1] = quad.Item2;
          vertices[4 * i + 2] = quad.Item3;
          vertices[4 * i + 3] = quad.Item4;
        }
        return this.AddQuads(vertices);
      }

      public IPrimitive AddQuads(params IVertex[] vertices) {
        Debug.Assert(vertices.Length % 4 == 0);
        var primitive = new PrimitiveImpl(PrimitiveType.QUADS, vertices);
        this.primitives_.Add(primitive);
        return primitive;
      }

      private class VertexImpl : IVertex {
        private IList<ITexCoord>? uvs_;

        public VertexImpl(int index, float x, float y, float z) {
          this.Index = index;
          this.SetLocalPosition(x, y, z);
        }

        public int Index { get; }

        public IReadOnlyList<BoneWeight>? Weights { get; private set; }

        public IVertex SetBone(IBone bone)
          => this.SetBones(
              new BoneWeight(bone, MatrixTransformUtil.IDENTITY, 1));

        public IVertex SetBones(params BoneWeight[] weights) {
          this.Weights = new ReadOnlyCollection<BoneWeight>(weights);
          return this;
        }


        public IPosition LocalPosition { get; } = new PositionImpl();

        public IVertex SetLocalPosition(float x, float y, float z) {
          this.LocalPosition.X = x;
          this.LocalPosition.Y = y;
          this.LocalPosition.Z = z;
          return this;
        }


        public INormal? LocalNormal { get; private set; }

        public IVertex SetLocalNormal(float x, float y, float z) {
          this.LocalNormal ??= new NormalImpl();
          this.LocalNormal.X = x;
          this.LocalNormal.Y = y;
          this.LocalNormal.Z = z;
          return this;
        }


        public IColor? Color { get; }

        public IVertex SetColor(byte r, byte g, byte b)
          => throw new NotImplementedException();


        public IReadOnlyList<ITexCoord>? Uvs { get; private set; }

        public IVertex SetUv(float u, float v) => this.SetUv(0, u, v);

        public IVertex SetUv(int uvIndex, float u, float v) {
          if (this.uvs_ == null) {
            this.uvs_ = new List<ITexCoord>();
            this.Uvs = new ReadOnlyCollection<ITexCoord>(this.uvs_);
          }

          // TODO: Optimize this.
          var index = -1;
          for (var i = 0; i < this.uvs_.Count; ++i) {
            if (this.uvs_[i].TexCoordIndex == uvIndex) {
              index = i;
              break;
            }
          }

          var uv = new TexCoordImpl {TexCoordIndex = uvIndex, U = u, V = v};
          if (index != -1) {
            this.uvs_[index] = uv;
          } else {
            this.uvs_.Add(uv);
          }

          return this;
        }

        public ITexCoord? GetUv() => this.GetUv(0);

        public ITexCoord? GetUv(int uvIndex)
          => this.uvs_?.FirstOrDefault(uv => uv.TexCoordIndex == uvIndex);
      }


      private class PrimitiveImpl : IPrimitive {
        public PrimitiveImpl(PrimitiveType type, params IVertex[] vertices) {
          this.Type = type;
          this.Vertices = new ReadOnlyCollection<IVertex>(vertices);
        }

        public PrimitiveType Type { get; }
        public IReadOnlyList<IVertex> Vertices { get; }

        public IMaterial Material { get; private set; }

        public IPrimitive SetMaterial(IMaterial material) {
          this.Material = material;
          return this;
        }
      }

      private class TexCoordImpl : ITexCoord {
        public int TexCoordIndex { get; init; }
        public float U { get; init; }
        public float V { get; init; }

        public override string ToString() => $"{{{this.U}, {this.V}}}";
      }
    }
  }
}