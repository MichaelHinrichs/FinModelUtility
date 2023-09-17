using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;

using fin.color;
using fin.image;
using fin.io;
using fin.language.equations.fixedFunction;
using fin.schema.vector;
using fin.util.image;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    private partial class MaterialManagerImpl {
      public IStandardMaterial AddStandardMaterial() {
        var material = new StandardMaterialImpl();
        this.materials_.Add(material);
        return material;
      }
    }

    private class StandardMaterialImpl : BMaterialImpl, IStandardMaterial {
      public override IEnumerable<ITexture> Textures {
        get {
          if (this.DiffuseTexture != null) {
            yield return this.DiffuseTexture;
          }

          if (this.MaskTexture != null) {
            yield return this.MaskTexture;
          }

          if (this.AmbientOcclusionTexture != null) {
            yield return this.AmbientOcclusionTexture;
          }

          if (this.NormalTexture != null) {
            yield return this.NormalTexture;
          }

          if (this.EmissiveTexture != null) {
            yield return this.EmissiveTexture;
          }

          if (this.SpecularTexture != null) {
            yield return this.SpecularTexture;
          }
        }
      }

      public ITexture? DiffuseTexture { get; set; }
      public ITexture? MaskTexture { get; set; }
      public ITexture? AmbientOcclusionTexture { get; set; }
      public ITexture? NormalTexture { get; set; }
      public ITexture? EmissiveTexture { get; set; }
      public ITexture? SpecularTexture { get; set; }
    }
  }
}