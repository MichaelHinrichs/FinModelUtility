using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

using fin.color;

namespace fin.model.impl {
  // TODO: Add logic for optimizing the model.
  public partial class ModelImpl<TVertex> 
      : IModel<ISkin<TVertex>> where TVertex : IReadOnlyVertex {
    public ModelImpl(Func<int, Position, TVertex> vertexCreator) {
      this.Skin = new SkinImpl(vertexCreator);
    }

    // TODO: Rewrite this to take in options instead.
    public ModelImpl(int vertexCount,
                     Func<int, Position, TVertex> vertexCreator) {
      this.Skin = new SkinImpl(vertexCount, vertexCreator);
    }
  } 

  public class ModelImpl : ModelImpl<VertexImpl> {
    public ModelImpl() : base((index, position)
                                  => new VertexImpl(index, position)) { }

    // TODO: Rewrite this to take in options instead.
    public ModelImpl(int vertexCount) :
        base(vertexCount,
             (index, position) => new VertexImpl(index, position)) { }
  }

  public class VertexImpl : IVertex,
                           INormalTangentVertex,
                           IMultiColorVertex,
                           IMultiUvVertex {
    private IVertexAttributeArray<IColor>? colors_;
    private IVertexAttributeArray<ITexCoord>? uvs_;

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

    public void SetBoneWeights(IBoneWeights boneWeights) {
      this.BoneWeights = boneWeights;
    }


    public Position LocalPosition { get; private set; }

    public void SetLocalPosition(Position localPosition) {
      this.LocalPosition = localPosition;
    }

    public void SetLocalPosition(Vector3 localPosition)
      => this.SetLocalPosition(new Position(localPosition.X,
                                            localPosition.Y,
                                            localPosition.Z));

    public void SetLocalPosition(IVector3 localPosition)
      => this.SetLocalPosition(new Position(localPosition.X,
                                            localPosition.Y,
                                            localPosition.Z));


    public void SetLocalPosition(float x, float y, float z)
      => this.SetLocalPosition(new Position(x, y, z));


    public Normal? LocalNormal { get; private set; }

    public void SetLocalNormal(Normal? localNormal) {
      this.LocalNormal = localNormal;
    }

    public void SetLocalNormal(Vector3? localNormal)
      => this.SetLocalNormal(localNormal != null
                                 ? new Normal(localNormal.Value.X,
                                              localNormal.Value.Y,
                                              localNormal.Value.Z)
                                 : null);

    public void SetLocalNormal(IVector3? localNormal)
      => this.SetLocalNormal(localNormal != null
                                 ? new Normal(localNormal.X,
                                              localNormal.Y,
                                              localNormal.Z)
                                 : null);

    public void SetLocalNormal(float x, float y, float z)
      => this.SetLocalNormal(new Normal(x, y, z));


    public Tangent? LocalTangent { get; private set; }

    public void SetLocalTangent(Tangent? localTangent) {
      this.LocalTangent = localTangent;
    }

    public void SetLocalTangent(Vector4? localTangent)
      => this.SetLocalTangent(localTangent != null
                                  ? new Tangent(localTangent.Value.X,
                                                localTangent.Value.Y,
                                                localTangent.Value.Z,
                                                localTangent.Value.W)
                                  : null);

    public void SetLocalTangent(IVector4? localTangent)
      => this.SetLocalTangent(localTangent != null
                                  ? new Tangent(localTangent.X,
                                                localTangent.Y,
                                                localTangent.Z,
                                                localTangent.W)
                                  : null);

    public void SetLocalTangent(float x, float y, float z, float w)
      => this.SetLocalTangent(new Tangent(x, y, z, w));

    public void SetColor(Color? color) {
      if (color != null) {
        this.colors_ ??= new SingleVertexAttribute<IColor>();
        this.colors_[0] = FinColor.FromSystemColor(color.Value);
      } else {
        this.colors_?.Set(0, null);
        if (this.colors_?.Count == 0) {
          this.colors_ = null;
        }
      }
    }

    public void SetColor(IColor? color) {
      if (color != null) {
        this.colors_ ??= new SingleVertexAttribute<IColor>();
        this.colors_[0] = color;
      } else {
        this.colors_?.Set(0, null);
        if (this.colors_?.Count == 0) {
          this.colors_ = null;
        }
      }
    }

    public void SetColor(Vector4? color)
      => this.SetColor(color != null
                           ? FinColor.FromRgbaFloats(
                               color.Value.X,
                               color.Value.Y,
                               color.Value.Z,
                               color.Value.W)
                           : null);

    public void SetColor(IVector4? color)
      => this.SetColor(color != null
                           ? FinColor.FromRgbaFloats(
                               color.X,
                               color.Y,
                               color.Z,
                               color.W)
                           : null);

    public void SetColor(int colorIndex, IColor? color) {
      if (color != null) {
        this.colors_ ??= new SparseVertexAttributeArray<IColor>();
        this.colors_[colorIndex] = color;
      } else {
        this.colors_?.Set(colorIndex, null);
        if (this.colors_?.Count == 0) {
          this.colors_ = null;
        }
      }
    }

    public void SetColorBytes(byte r, byte g, byte b, byte a)
      => this.SetColor(FinColor.FromRgbaBytes(r, g, b, a));

    public void SetColorBytes(
        int colorIndex,
        byte r,
        byte g,
        byte b,
        byte a)
      => this.SetColor(colorIndex, FinColor.FromRgbaBytes(r, g, b, a));

    public int ColorCount => this.colors_?.Count ?? 0;
    public IColor? GetColor() => this.GetColor(0);

    public IColor? GetColor(int colorIndex) => this.colors_?.Get(colorIndex);


    public int UvCount => this.uvs_?.Count ?? 0;

    public void SetUv(ITexCoord? uv) {
      if (uv == null) {
        this.uvs_ = null;
      } else {
        this.uvs_ ??= new SingleVertexAttribute<ITexCoord>();
        this.uvs_[0] = uv;
      }
    }

    public void SetUv(Vector2? uv)
      => this.SetUv(uv != null
                        ? new TexCoordImpl { U = uv.Value.X, V = uv.Value.Y }
                        : null);

    public void SetUv(IVector2? uv)
      => this.SetUv(uv != null
                        ? new TexCoordImpl { U = uv.X, V = uv.Y }
                        : null);

    public void SetUv(float u, float v) {
      this.uvs_ ??= new SingleVertexAttribute<ITexCoord>();
      this.uvs_[0] = new TexCoordImpl { U = u, V = v };
    }

    public void SetUv(int uvIndex, ITexCoord? uv) {
      if (uv != null) {
        this.uvs_ ??= new SparseVertexAttributeArray<ITexCoord>();
        this.uvs_[uvIndex] = uv;
      } else {
        this.uvs_?.Set(uvIndex, null);
        if (this.uvs_?.Count == 0) {
          this.uvs_ = null;
        }
      }
    }

    public void SetUv(int uvIndex, float u, float v)
      => this.SetUv(uvIndex, new TexCoordImpl { U = u, V = v });

    public ITexCoord? GetUv() => this.GetUv(0);

    public ITexCoord? GetUv(int uvIndex) => this.uvs_?.Get(uvIndex);
  }

  public class TexCoordImpl : ITexCoord {
    public float U { get; init; }
    public float V { get; init; }

    public override string ToString() => $"{{{this.U}, {this.V}}}";
  }
}