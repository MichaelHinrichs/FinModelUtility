using System;

namespace fin.model.impl {
  // TODO: Add logic for optimizing the model.
  public partial class ModelImpl<TVertex> 
      : IModel<ISkin<TVertex>> where TVertex : IReadOnlyVertex {
    public ModelImpl(Func<int, Position, TVertex> vertexCreator,
                     ILighting? lighting = null) {
      this.Skin = new SkinImpl(vertexCreator);
      this.Lighting = lighting ?? new LightingImpl();
    }

    // TODO: Rewrite this to take in options instead.
    public ModelImpl(int vertexCount,
                     Func<int, Position, TVertex> vertexCreator,
                     ILighting? lighting = null) {
      this.Skin = new SkinImpl(vertexCount, vertexCreator);
      this.Lighting = lighting ?? new LightingImpl();
    }
  } 

  public class ModelImpl : ModelImpl<NormalTangentMultiColorMultiUvVertexImpl> {
    public ModelImpl() : base((index, position)
                                  => new NormalTangentMultiColorMultiUvVertexImpl(index, position)) { }

    // TODO: Rewrite this to take in options instead.
    public ModelImpl(int vertexCount) :
        base(vertexCount,
             (index, position) => new NormalTangentMultiColorMultiUvVertexImpl(index, position)) { }
  }
}