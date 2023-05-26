using System;
using System.Runtime.CompilerServices;

using fin.color;

namespace fin.model {
  public interface IVertexTargeter {
    void Target(IReadOnlyVertex vertex);
  }

  public interface IVertexAccessor
      : IVertexNormalAccessor,
        IVertexTangentAccessor,
        IVertexColorAccessor,
        IVertexUvAccessor { }

  public interface IVertexNormalAccessor : IVertexTargeter,
                                           IReadOnlyNormalVertex { }

  public interface IVertexTangentAccessor : IVertexTargeter,
                                            IReadOnlyTangentVertex { }

  public interface IVertexColorAccessor : IVertexTargeter,
                                          IReadOnlySingleColorVertex,
                                          IReadOnlyMultiColorVertex { }

  public interface IVertexUvAccessor : IVertexTargeter,
                                       IReadOnlySingleUvVertex,
                                       IReadOnlyMultiUvVertex { }


  /// <summary>
  ///   Assumes all vertices are the same, consistent type.
  /// </summary>
  public class ConsistentVertexAccessor : IVertexAccessor {
    private IReadOnlyVertex currentVertex_;
    private readonly IVertexNormalAccessor normalAccessor_;
    private readonly IVertexTangentAccessor tangentAccessor_;
    private readonly IVertexColorAccessor colorAccessor_;
    private readonly IVertexUvAccessor uvAccessor_;

    public static IVertexAccessor GetAccessorForModel(IModel model)
      => new ConsistentVertexAccessor(model);

    private ConsistentVertexAccessor(IModel model) {
      var skin = model.Skin;
      var firstVertex = skin.Vertices.Count > 0 ? skin.Vertices[0] : null;

      this.normalAccessor_ = firstVertex is IReadOnlyNormalVertex
          ? new NormalAccessor()
          : new NullNormalAccessor();
      this.tangentAccessor_ = firstVertex is IReadOnlyTangentVertex
          ? new TangentAccessor()
          : new NullTangentAccessor();
      this.colorAccessor_ = firstVertex is IReadOnlyMultiColorVertex
          ? new MultiColorAccessor()
          : firstVertex is IReadOnlySingleColorVertex
              ? new SingleColorAccessor()
              : new NullColorAccessor();
      this.uvAccessor_ = firstVertex is IReadOnlyMultiUvVertex
          ? new MultiUvAccessor()
          : firstVertex is IReadOnlySingleUvVertex
              ? new SingleUvAccessor()
              : new NullUvAccessor();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Target(IReadOnlyVertex vertex) {
      this.currentVertex_ = vertex;
      this.normalAccessor_.Target(vertex);
      this.tangentAccessor_.Target(vertex);
      this.colorAccessor_.Target(vertex);
      this.uvAccessor_.Target(vertex);
    }

    public int Index => this.currentVertex_.Index;

    public IBoneWeights? BoneWeights => this.currentVertex_.BoneWeights;
    public Position LocalPosition => this.currentVertex_.LocalPosition;

    public Normal? LocalNormal => this.normalAccessor_.LocalNormal;
    public Tangent? LocalTangent => this.tangentAccessor_.LocalTangent;

    public int ColorCount => this.colorAccessor_.ColorCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IColor? GetColor() => this.colorAccessor_.GetColor();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IColor? GetColor(int colorIndex)
      => this.colorAccessor_.GetColor(colorIndex);

    public int UvCount => this.uvAccessor_.UvCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TexCoord? GetUv() => this.uvAccessor_.GetUv();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TexCoord? GetUv(int uvIndex) => this.uvAccessor_.GetUv(uvIndex);


    private abstract class BAccessor : IReadOnlyVertex {
      public int Index => throw new NotImplementedException();
      public IBoneWeights? BoneWeights => throw new NotImplementedException();
      public Position LocalPosition => throw new NotImplementedException();
    }


    private sealed class NullNormalAccessor : BAccessor, IVertexNormalAccessor {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) { }

      public Normal? LocalNormal => new Normal();
    }

    private sealed class NormalAccessor : BAccessor, IVertexNormalAccessor {
      private IReadOnlyNormalVertex normalVertex_;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) {
        this.normalVertex_ = vertex as IReadOnlyNormalVertex;
      }

      public Normal? LocalNormal => this.normalVertex_.LocalNormal;
    }


    private sealed class NullTangentAccessor
        : BAccessor, IVertexTangentAccessor {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) { }

      public Tangent? LocalTangent => new Tangent();
    }

    private sealed class TangentAccessor : BAccessor, IVertexTangentAccessor {
      private IReadOnlyTangentVertex tangentVertex_;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) {
        this.tangentVertex_ = vertex as IReadOnlyTangentVertex;
      }

      public Tangent? LocalTangent => this.tangentVertex_.LocalTangent;
    }


    private sealed class NullColorAccessor
        : BAccessor, IVertexColorAccessor {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) { }

      public int ColorCount => 0;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IColor? GetColor() => null;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IColor? GetColor(int colorIndex) => null;
    }

    private sealed class SingleColorAccessor : BAccessor, IVertexColorAccessor {
      private IReadOnlySingleColorVertex colorVertex_;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) {
        this.colorVertex_ = vertex as IReadOnlySingleColorVertex;
      }

      public int ColorCount => this.GetColor() != null ? 1 : 0;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IColor? GetColor() => this.colorVertex_.GetColor();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IColor? GetColor(int colorIndex) => this.colorVertex_.GetColor();
    }

    private sealed class MultiColorAccessor : BAccessor, IVertexColorAccessor {
      private IReadOnlyMultiColorVertex colorVertex_;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) {
        this.colorVertex_ = vertex as IReadOnlyMultiColorVertex;
      }

      public int ColorCount => this.colorVertex_.ColorCount;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IColor? GetColor() => this.GetColor(0);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IColor? GetColor(int colorIndex)
        => this.colorVertex_.GetColor(colorIndex);
    }


    private sealed class NullUvAccessor
        : BAccessor, IVertexUvAccessor {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) { }

      public int UvCount => 0;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv() => null;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv(int uvIndex) => null;
    }

    private sealed class SingleUvAccessor : BAccessor, IVertexUvAccessor {
      private IReadOnlySingleUvVertex uvVertex_;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) {
        this.uvVertex_ = vertex as IReadOnlySingleUvVertex;
      }

      public int UvCount => this.GetUv() != null ? 1 : 0;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv() => this.uvVertex_.GetUv();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv(int uvIndex) => this.uvVertex_.GetUv();
    }

    private sealed class MultiUvAccessor : BAccessor, IVertexUvAccessor {
      private IReadOnlyMultiUvVertex uvVertex_;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) {
        this.uvVertex_ = vertex as IReadOnlyMultiUvVertex;
      }

      public int UvCount => this.uvVertex_.UvCount;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv() => this.GetUv(0);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv(int uvIndex) => this.uvVertex_.GetUv(uvIndex);
    }
  }
}