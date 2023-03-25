using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using fin.color;
using fin.data;
using fin.math;

using System.Threading.Tasks;

using Microsoft.Toolkit.HighPerformance.Helpers;

using System;


namespace fin.model.impl {
  public partial class ModelImpl {
    public ISkin Skin { get; }

    private class SkinImpl : ISkin {
      private readonly IList<IVertex> vertices_;
      private readonly IList<IMesh> meshes_ = new List<IMesh>();

      private readonly BoneWeightsDictionary boneWeightsDictionary_ = new();

      private readonly IndexableDictionary<IBone, IBoneWeights>
          boneWeightsByBone_ = new();

      public SkinImpl() {
        this.vertices_ = new List<IVertex>();
        this.Vertices = new ReadOnlyCollection<IVertex>(this.vertices_);
        this.Meshes = new ReadOnlyCollection<IMesh>(this.meshes_);
      }

      public SkinImpl(int vertexCount) {
        this.vertices_ = new List<IVertex>(vertexCount);
        for (var i = 0; i < vertexCount; ++i) {
          this.vertices_.Add(default);
        }

        ParallelHelper.For(0, vertexCount, new VertexGenerator(this.vertices_));

        this.Vertices = new ReadOnlyCollection<IVertex>(this.vertices_);
        this.Meshes = new ReadOnlyCollection<IMesh>(this.meshes_);
      }

      private readonly struct VertexGenerator : IAction {
        private readonly IList<IVertex> vertices_;

        public VertexGenerator(IList<IVertex> vertices) {
          this.vertices_ = vertices;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(int index)
          => this.vertices_[index] = new VertexImpl(index, 0, 0, 0);
      }

      public IReadOnlyList<IVertex> Vertices { get; }

      public IVertex AddVertex(Position position) {
        lock (this.vertices_) {
          var vertex = new VertexImpl(this.vertices_.Count, position);
          this.vertices_.Add(vertex);
          return vertex;
        }
      }

      public IVertex AddVertex(IVector3 position)
        => this.AddVertex(new Position(position.X, position.Y, position.Z));

      public IVertex AddVertex(float x, float y, float z) {
        lock (this.vertices_) {
          var vertex = new VertexImpl(this.vertices_.Count, x, y, z);
          this.vertices_.Add(vertex);
          return vertex;
        }
      }


      public IReadOnlyList<IMesh> Meshes { get; }

      public IMesh AddMesh() {
        var mesh = new MeshImpl();
        this.meshes_.Add(mesh);
        return mesh;
      }

      public IReadOnlyList<IBoneWeights> BoneWeights
        => this.boneWeightsDictionary_.List;

      public IBoneWeights GetOrCreateBoneWeights(
          PreprojectMode preprojectMode,
          IBone bone) {
        if (!this.boneWeightsByBone_.TryGetValue(bone, out var boneWeights)) {
          boneWeights = this.CreateBoneWeights(
              preprojectMode,
              new BoneWeight(bone, FinMatrix4x4.IDENTITY, 1));
          this.boneWeightsByBone_[bone] = boneWeights;
        }

        return boneWeights;
      }

      public IBoneWeights GetOrCreateBoneWeights(PreprojectMode preprojectMode,
                                                 params IBoneWeight[] weights)
        => boneWeightsDictionary_.GetOrCreate(preprojectMode, weights);

      public IBoneWeights CreateBoneWeights(PreprojectMode preprojectMode,
                                            params IBoneWeight[] weights)
        => this.boneWeightsDictionary_.Create(preprojectMode, weights);


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

        public ILinesPrimitive AddLines(params (IVertex, IVertex)[] lines) {
          var vertices = new IVertex[2 * lines.Length];
          for (var i = 0; i < lines.Length; ++i) {
            var line = lines[i];
            vertices[2 * i] = line.Item1;
            vertices[2 * i + 1] = line.Item2;
          }

          return this.AddLines(vertices);
        }

        public ILinesPrimitive AddLines(params IVertex[] lines) {
          Debug.Assert(lines.Length % 2 == 0);
          var primitive = new LinesPrimitiveImpl(lines);
          this.primitives_.Add(primitive);
          return primitive;
        }

        public IPointsPrimitive AddPoints(params IVertex[] points) {
          var primitive = new PointsPrimitiveImpl(points);
          this.primitives_.Add(primitive);
          return primitive;
        }
      }

      private class VertexImpl : IVertex {
        public VertexImpl(int index, Position position) {
          this.Index = index;
          this.SetLocalPosition(position);
        }

        public VertexImpl(int index, float x, float y, float z) {
          this.Index = index;
          this.SetLocalPosition(x, y, z);
        }

        public int Index { get; }

        public IBoneWeights? BoneWeights { get; private set; }

        public IVertex SetBoneWeights(IBoneWeights boneWeights) {
          this.BoneWeights = boneWeights;
          return this;
        }


        public Position LocalPosition { get; private set; }

        public IVertex SetLocalPosition(Position localPosition) {
          this.LocalPosition = localPosition;
          return this;
        }

        public IVertex SetLocalPosition(IVector3 localPosition)
          => this.SetLocalPosition(new Position(localPosition.X,
                                         localPosition.Y,
                                         localPosition.Z));


        public IVertex SetLocalPosition(float x, float y, float z)
          => this.SetLocalPosition(new Position(x, y, z));


        public Normal? LocalNormal { get; private set; }

        public IVertex SetLocalNormal(Normal? localNormal) {
          this.LocalNormal = localNormal;
          return this;
        }

        public IVertex SetLocalNormal(IVector3? localNormal)
          => this.SetLocalNormal(localNormal != null
                                     ? new Normal(localNormal.X,
                                                  localNormal.Y,
                                                  localNormal.Z)
                                     : null);

        public IVertex SetLocalNormal(float x, float y, float z)
          => this.SetLocalNormal(new Normal(x, y ,z));


        public Tangent? LocalTangent { get; private set; }

        public IVertex SetLocalTangent(Tangent? localTangent) {
          this.LocalTangent = localTangent;
          return this;
        }

        public IVertex SetLocalTangent(float x, float y, float z, float w)
          => this.SetLocalTangent(new Tangent(x, y, z, w));


        public IVertexAttributeArray<IColor>? Colors { get; private set; }

        public IVertex SetColor(IColor? color) {
          if (color != null) {
            this.Colors ??= new SingleVertexAttribute<IColor>();
            this.Colors[0] = color;
          } else {
            this.Colors?.Set(0, null);
            if (this.Colors?.Count == 0) {
              this.Colors = null;
            }
          }

          return this;
        }

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
          => this.SetColor(FinColor.FromRgbaBytes(r, g, b, a));

        public IVertex SetColorBytes(
            int colorIndex,
            byte r,
            byte g,
            byte b,
            byte a)
          => this.SetColor(colorIndex, FinColor.FromRgbaBytes(r, g, b, a));

        public IColor? GetColor() => this.GetColor(0);

        public IColor? GetColor(int colorIndex) => this.Colors?.Get(colorIndex);


        public IVertexAttributeArray<ITexCoord>? Uvs { get; private set; }

        public IVertex SetUv(ITexCoord? uv) {
          if (uv == null) {
            this.Uvs = null;
          } else {
            this.Uvs ??= new SingleVertexAttribute<ITexCoord>();
            this.Uvs[0] = uv;
          }

          return this;
        }

        public IVertex SetUv(float u, float v) {
          this.Uvs ??= new SingleVertexAttribute<ITexCoord>();
          this.Uvs[0] = new TexCoordImpl { U = u, V = v };
          return this;
        }

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
          => this.SetUv(uvIndex, new TexCoordImpl { U = u, V = v });

        public ITexCoord? GetUv() => this.GetUv(0);

        public ITexCoord? GetUv(int uvIndex) => this.Uvs?.Get(uvIndex);
      }

      private class PrimitiveImpl : BPrimitiveImpl {
        public PrimitiveImpl(PrimitiveType type, params IVertex[] vertices) :
            base(type, vertices) { }
      }

      private class LinesPrimitiveImpl : BPrimitiveImpl, ILinesPrimitive {
        public LinesPrimitiveImpl(params IVertex[] vertices) : base(
            PrimitiveType.LINES,
            vertices) { }

        public float LineWidth { get; private set; }

        public ILinesPrimitive SetLineWidth(float width) {
          this.LineWidth = width;
          return this;
        }
      }

      private class PointsPrimitiveImpl : BPrimitiveImpl, IPointsPrimitive {
        public PointsPrimitiveImpl(params IVertex[] vertices) : base(
            PrimitiveType.POINTS,
            vertices) { }

        public float Radius { get; private set; }

        public IPointsPrimitive SetRadius(float radius) {
          this.Radius = radius;
          return this;
        }
      }

      private abstract class BPrimitiveImpl : IPrimitive {
        public BPrimitiveImpl(PrimitiveType type, params IVertex[] vertices) {
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

        public VertexOrder VertexOrder { get; private set; } = VertexOrder.FLIP;

        public IPrimitive SetVertexOrder(VertexOrder vertexOrder) {
          this.VertexOrder = vertexOrder;
          return this;
        }

        public uint InversePriority { get; private set; }

        public IPrimitive SetInversePriority(uint inversePriority) {
          this.InversePriority = inversePriority;
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