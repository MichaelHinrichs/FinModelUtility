using System.Drawing;
using System.Numerics;

using fin.color;
using fin.data;
using fin.model.impl;

namespace fin.model {
  public interface IReadOnlyVertex : IIndexable {
    IBoneWeights? BoneWeights { get; }
    Position LocalPosition { get; }
  }

  public interface IVertex : IReadOnlyVertex {
    void SetBoneWeights(IBoneWeights boneWeights);

    void SetLocalPosition(Position localPosition);
    void SetLocalPosition(Vector3 localPosition);
    void SetLocalPosition(IVector3 localPosition);
    void SetLocalPosition(float x, float y, float z);
  }


  public interface IReadOnlyNormalVertex : IReadOnlyVertex {
    Normal? LocalNormal { get; }
  }

  public interface INormalVertex : IReadOnlyNormalVertex, IVertex {
    void SetLocalNormal(Normal? localNormal);
    void SetLocalNormal(Vector3? localNormal);
    void SetLocalNormal(IVector3? localNormal);
    void SetLocalNormal(float x, float y, float z);
  }


  public interface IReadOnlyTangentVertex : IReadOnlyVertex {
    Tangent? LocalTangent { get; }
  }

  public interface ITangentVertex : IReadOnlyTangentVertex, IVertex {
    void SetLocalTangent(Tangent? localTangent);
    void SetLocalTangent(Vector4? localTangent);
    void SetLocalTangent(IVector4? localTangent);
    void SetLocalTangent(float x, float y, float z, float w);
  }


  public interface IReadOnlyNormalTangentVertex : IReadOnlyNormalVertex,
                                                  IReadOnlyTangentVertex { }

  public interface INormalTangentVertex : IReadOnlyNormalTangentVertex,
                                          INormalVertex, ITangentVertex { }


  public interface IReadOnlySingleColorVertex : IReadOnlyVertex {
    IColor? GetColor();
  }

  public interface ISingleColorVertex : IReadOnlySingleColorVertex, IVertex {
    void SetColor(Color? color);
    void SetColor(IColor? color);
    void SetColor(Vector4? color);
    void SetColor(IVector4? color);
    void SetColorBytes(byte r, byte g, byte b, byte a);
  }


  public interface IReadOnlyMultiColorVertex : IReadOnlySingleColorVertex {
    int ColorCount { get; }
    IColor? GetColor(int colorIndex);
  }

  public interface IMultiColorVertex : IReadOnlyMultiColorVertex,
                                       ISingleColorVertex {
    void SetColor(int colorIndex, IColor? color);

    void SetColorBytes(int colorIndex,
                       byte r,
                       byte g,
                       byte b,
                       byte a);
  }


  public interface IReadOnlySingleUvVertex : IReadOnlyVertex {
    ITexCoord? GetUv();
  }

  public interface ISingleUvVertex : IReadOnlySingleUvVertex, IVertex {
    void SetUv(ITexCoord? uv);
    void SetUv(Vector2? uv);
    void SetUv(IVector2? uv);
    void SetUv(float u, float v);
  }


  public interface IReadOnlyMultiUvVertex : IReadOnlySingleUvVertex {
    int UvCount { get; }
    ITexCoord? GetUv(int uvIndex);
  }

  public interface IMultiUvVertex : IReadOnlyMultiUvVertex, ISingleUvVertex {
    void SetUv(int uvIndex, ITexCoord? uv);
    void SetUv(int uvIndex, float u, float v);
  }
}