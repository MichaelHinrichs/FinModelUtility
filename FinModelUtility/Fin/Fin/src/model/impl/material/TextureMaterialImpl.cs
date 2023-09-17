using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    private partial class MaterialManagerImpl {
      public ITextureMaterial AddTextureMaterial(ITexture texture) {
        var material = new TextureMaterialImpl(texture);
        this.materials_.Add(material);
        return material;
      }
    }

    private class TextureMaterialImpl : BMaterialImpl, ITextureMaterial {
      public TextureMaterialImpl(ITexture texture) {
        this.Texture = texture;
        this.Textures = new ReadOnlyCollection<ITexture>(new[] { texture });
      }

      public ITexture Texture { get; }
      public override IEnumerable<ITexture> Textures { get; }
    }
  }
}