using System;

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
                                          IReadOnlyMultiColorVertex { }

  public interface IVertexUvAccessor : IVertexTargeter,
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
    public IColor? GetColor() => this.colorAccessor_.GetColor();

    public IColor? GetColor(int colorIndex)
      => this.colorAccessor_.GetColor(colorIndex);

    public int UvCount => this.uvAccessor_.UvCount;
    public ITexCoord? GetUv() => this.uvAccessor_.GetUv();
    public ITexCoord? GetUv(int uvIndex) => this.uvAccessor_.GetUv(uvIndex);


    private abstract class BAccessor : IReadOnlyVertex {
      public int Index => throw new NotImplementedException();
      public IBoneWeights? BoneWeights => throw new NotImplementedException();
      public Position LocalPosition => throw new NotImplementedException();
    }


    private sealed class NullNormalAccessor : BAccessor, IVertexNormalAccessor {
      public void Target(IReadOnlyVertex vertex) { }
      public Normal? LocalNormal => new Normal();
    }

    private sealed class NormalAccessor : BAccessor, IVertexNormalAccessor {
      private IReadOnlyNormalVertex normalVertex_;

      public void Target(IReadOnlyVertex vertex) {
        this.normalVertex_ = vertex as IReadOnlyNormalVertex;
      }

      public Normal? LocalNormal => this.normalVertex_.LocalNormal;
    }


    private sealed class NullTangentAccessor
        : BAccessor, IVertexTangentAccessor {
      public void Target(IReadOnlyVertex vertex) { }
      public Tangent? LocalTangent => new Tangent();
    }

    private sealed class TangentAccessor : BAccessor, IVertexTangentAccessor {
      private IReadOnlyTangentVertex tangentVertex_;

      public void Target(IReadOnlyVertex vertex) {
        this.tangentVertex_ = vertex as IReadOnlyTangentVertex;
      }

      public Tangent? LocalTangent => this.tangentVertex_.LocalTangent;
    }


    private sealed class NullColorAccessor
        : BAccessor, IVertexColorAccessor {
      public void Target(IReadOnlyVertex vertex) { }

      public int ColorCount => 0;
      public IColor? GetColor() => null;
      public IColor? GetColor(int colorIndex) => null;
    }

    private sealed class SingleColorAccessor : BAccessor, IVertexColorAccessor {
      private IReadOnlySingleColorVertex colorVertex_;

      public void Target(IReadOnlyVertex vertex) {
        this.colorVertex_ = vertex as IReadOnlySingleColorVertex;
      }

      public int ColorCount => this.GetColor() != null ? 1 : 0;
      public IColor? GetColor() => this.colorVertex_.GetColor();
      public IColor? GetColor(int colorIndex) => this.colorVertex_.GetColor();
    }

    private sealed class MultiColorAccessor : BAccessor, IVertexColorAccessor {
      private IReadOnlyMultiColorVertex colorVertex_;

      public void Target(IReadOnlyVertex vertex) {
        this.colorVertex_ = vertex as IReadOnlyMultiColorVertex;
      }

      public int ColorCount => this.colorVertex_.ColorCount;
      public IColor? GetColor() => this.colorVertex_.GetColor();

      public IColor? GetColor(int colorIndex)
        => this.colorVertex_.GetColor(colorIndex);
    }


    private sealed class NullUvAccessor
        : BAccessor, IVertexUvAccessor {
      public void Target(IReadOnlyVertex vertex) { }

      public int UvCount => 0;
      public ITexCoord? GetUv() => null;
      public ITexCoord? GetUv(int uvIndex) => null;
    }

    private sealed class SingleUvAccessor : BAccessor, IVertexUvAccessor {
      private IReadOnlySingleUvVertex uvVertex_;

      public void Target(IReadOnlyVertex vertex) {
        this.uvVertex_ = vertex as IReadOnlySingleUvVertex;
      }

      public int UvCount => this.GetUv() != null ? 1 : 0;
      public ITexCoord? GetUv() => this.uvVertex_.GetUv();
      public ITexCoord? GetUv(int uvIndex) => this.uvVertex_.GetUv();
    }

    private sealed class MultiUvAccessor : BAccessor, IVertexUvAccessor {
      private IReadOnlyMultiUvVertex uvVertex_;

      public void Target(IReadOnlyVertex vertex) {
        this.uvVertex_ = vertex as IReadOnlyMultiUvVertex;
      }

      public int UvCount => this.uvVertex_.UvCount;
      public ITexCoord? GetUv() => this.uvVertex_.GetUv();
      public ITexCoord? GetUv(int uvIndex) => this.uvVertex_.GetUv(uvIndex);
    }
  }
}