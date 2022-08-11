using System;
using System.Collections.Generic;
using System.Linq;

using fin.model;


namespace fin.gl.material {
  public class GlSimpleMaterialShader : IGlMaterialShader {
    private readonly IList<GlTexture> textures_;

    public GlSimpleMaterialShader(IMaterial material) {
      this.Material = material;

      var finTextures = material.Textures.ToArray();

      var nSupportedTextures = 8;
      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < nSupportedTextures; ++i) {
        var finTexture = i < (finTextures?.Length ?? 0)
                             ? finTextures[i]
                             : null;

        this.textures_.Add(finTexture != null
                               ? new GlTexture(finTexture)
                               : GlMaterialConstants.NULL_WHITE_TEXTURE);
      }
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var texture in this.textures_) {
        GlMaterialConstants.DisposeIfNotCommon(texture);
      }
      this.textures_.Clear();
    }


    public IMaterial Material { get; }

    public void Use() {
      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }
    }
  }
}