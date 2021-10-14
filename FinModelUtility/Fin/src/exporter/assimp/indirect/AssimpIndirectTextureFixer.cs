using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using Assimp;

using fin.model;
using fin.model.util;

namespace fin.exporter.assimp.indirect {
  using WrapMode = fin.model.WrapMode;
  using FinBlendMode = fin.model.BlendMode;

  public class AssimpIndirectTextureFixer {
    public void Fix(IModel model, Scene sc) {
      // Imports the textures
      var finTextures = new HashSet<ITexture>();
      foreach (var finMaterial in model.MaterialManager.All) {
        foreach (var finTexture in finMaterial.Textures) {
          finTextures.Add(finTexture);
        }
      }

      var originalMaterialOrder =
          sc.Materials.Select(material => material.Name).ToArray();

      sc.Textures.Clear();

      foreach (var finTexture in finTextures) {
        var imageData = finTexture.ImageData;

        var imageBytes = new MemoryStream();
        imageData.Save(imageBytes, ImageFormat.Png);

        var assTexture =
            new EmbeddedTexture("png",
                                imageBytes.ToArray(),
                                finTexture.Name);
        assTexture.Filename = finTexture.Name + ".png";

        sc.Textures.Add(assTexture);
      }

      // Need to keep order the same because Assimp references them by index.
      for (var m = 0; m < originalMaterialOrder.Length; ++m) {
        var originalMaterialName = originalMaterialOrder[m];
        var finMaterial =
            model.MaterialManager.All
                 .FirstOrDefault(finMaterial => finMaterial.Name == originalMaterialName);

        if (finMaterial == null) {
          continue;
        }
        
        var assMaterial = new Material {Name = finMaterial.Name};

        var finTexture = PrimaryTextureFinder.GetFor(finMaterial);
        var assTextureSlot = new TextureSlot {
            FilePath = finTexture.Name + ".png",
            // TODO: FBX doesn't support mirror. Blegh
            WrapModeU = this.ConvertWrapMode_(finTexture.WrapModeU),
            WrapModeV = this.ConvertWrapMode_(finTexture.WrapModeV)
        };

        assTextureSlot.TextureType = TextureType.Diffuse;
        assTextureSlot.UVIndex = finTexture.UvIndex;

        assMaterial.AddMaterialTexture(assTextureSlot);

        // Meshes should already have material indices set.
        sc.Materials[m] = assMaterial;
      }
    }

    private TextureWrapMode ConvertWrapMode_(WrapMode wrapMode)
      => wrapMode switch {
          WrapMode.CLAMP         => TextureWrapMode.Clamp,
          WrapMode.REPEAT        => TextureWrapMode.Wrap,
          WrapMode.MIRROR_REPEAT => TextureWrapMode.Mirror,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(wrapMode),
                   wrapMode,
                   null)
      };
  }
}