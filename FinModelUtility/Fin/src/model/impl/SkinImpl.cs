using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using fin.math.matrix;


namespace fin.model.impl {
  public partial class ModelImpl {
    public ISkin Skin { get; }

    private class SkinImpl : ISkin {
      private readonly IList<IVertex> vertices_;
      private readonly IList<IMesh> meshes_ = new List<IMesh>();

      public SkinImpl() {
        this.vertices_ = new List<IVertex>();
        this.Vertices = new ReadOnlyCollection<IVertex>(this.vertices_);
        this.Meshes = new ReadOnlyCollection<IMesh>(this.meshes_);
      }

      public SkinImpl(int vertexCount) {
        this.vertices_ = new List<IVertex>(vertexCount);
        this.Vertices = new ReadOnlyCollection<IVertex>(this.vertices_);
        this.Meshes = new ReadOnlyCollection<IMesh>(this.meshes_);
      }

      public IReadOnlyList<IVertex> Vertices { get; }

      public IVertex AddVertex(IPosition position) {
        var vertex = new VertexImpl(this.vertices_.Count, position);
        this.vertices_.Add(vertex);
        return vertex;
      }

      public IVertex AddVertex(float x, float y, float z) {
        var vertex = new VertexImpl(this.vertices_.Count, x, y, z);
        this.vertices_.Add(vertex);
        return vertex;
      }


      public IReadOnlyList<IMesh> Meshes { get; }

      public IMesh AddMesh() {
        var mesh = new MeshImpl();
        this.meshes_.Add(mesh);
        return mesh;
      }

      private class MeshImpl : IMesh {
        private readonly IList<IPrimitive> primitives_ = new List<IPrimitive>();

        public MeshImpl() {
          this.Primitives =
              new ReadOnlyCollection<IPrimitive>(this.primitives_);
        }

        public string Name { get; set; }

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
      }

      private class VertexImpl : IVertex {
        public VertexImpl(int index, IPosition position) {
          this.Index = index;
          this.SetLocalPosition(position);
        }

        public VertexImpl(int index, float x, float y, float z) {
          this.Index = index;
          this.SetLocalPosition(x, y, z);
        }

        public int Index { get; }

        public IReadOnlyList<BoneWeight>? Weights { get; private set; }

        public PreprojectMode PreprojectMode { get; set; } =
          PreprojectMode.BONE;

        public IVertex SetBone(IBone bone)
          => this.SetBones(
              new BoneWeight(bone, MatrixTransformUtil.IDENTITY, 1));

        public IVertex SetBones(params BoneWeight[] weights) {
          this.Weights = new ReadOnlyCollection<BoneWeight>(weights);
          return this;
        }


        public IPosition LocalPosition { get; private set; }

        public IVertex SetLocalPosition(IPosition localPosition) {
          this.LocalPosition = localPosition;
          return this;
        }

        public IVertex SetLocalPosition(float x, float y, float z)
          => this.SetLocalPosition(new PositionImpl {X = x, Y = y, Z = z});


        public INormal? LocalNormal { get; private set; }

        public IVertex SetLocalNormal(INormal localNormal) {
          this.LocalNormal = localNormal;
          return this;
        }

        public IVertex SetLocalNormal(float x, float y, float z)
          => this.SetLocalNormal(new NormalImpl {X = x, Y = y, Z = z});


        public ITangent? LocalTangent { get; private set; }

        public IVertex SetLocalTangent(ITangent localTangent) {
          this.LocalTangent = localTangent;
          return this;
        }

        public IVertex SetLocalTangent(float x, float y, float z, float w)
          => this.SetLocalTangent(new TangentImpl {X = x, Y = y, Z = z, W = w});


        public IVertexAttributeArray<IColor>? Colors { get; private set; }

        public IVertex SetColor(IColor? color) => this.SetColor(0, color);

        public IVertex SetColor(int colorIndex, IColor? color) {
          if (color != null) {
            this.Colors ??= new SparseVertexAttributeArray<IColor>();
            this.Colors[colorIndex] = color;
          } else {
            this.Colors?.Set(colorIndex, null);
            if (this.Colors?.Count == 0) {
              this.Colors = null;
            }
          }

          return this;
        }

        public IVertex SetColorBytes(byte r, byte g, byte b, byte a)
          => this.SetColorBytes(0, r, g, b, a);

        public IVertex SetColorBytes(
            int colorIndex,
            byte r,
            byte g,
            byte b,
            byte a)
          => this.SetColor(colorIndex, ColorImpl.FromRgbaBytes(r, g, b, a));

        public IColor? GetColor() => this.GetColor(0);

        public IColor? GetColor(int colorIndex) => this.Colors?.Get(colorIndex);


        public IVertexAttributeArray<ITexCoord>? Uvs { get; private set; }

        public IVertex SetUv(ITexCoord? uv) => this.SetUv(0, uv);
        public IVertex SetUv(float u, float v) => this.SetUv(0, u, v);

        public IVertex SetUv(int uvIndex, ITexCoord? uv) {
          if (uv != null) {
            this.Uvs ??= new SparseVertexAttributeArray<ITexCoord>();
            this.Uvs[uvIndex] = uv;
          } else {
            this.Uvs?.Set(uvIndex, null);
            if (this.Uvs?.Count == 0) {
              this.Uvs = null;
            }
          }

          return this;
        }

        public IVertex SetUv(int uvIndex, float u, float v)
          => this.SetUv(uvIndex, new TexCoordImpl {U = u, V = v});

        public ITexCoord? GetUv() => this.GetUv(0);

        public ITexCoord? GetUv(int uvIndex) => this.Uvs?.Get(uvIndex);
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
    }

    public class TexCoordImpl : ITexCoord {
      public float U { get; init; }
      public float V { get; init; }

      public override string ToString() => $"{{{this.U}, {this.V}}}";
    }
  }
}