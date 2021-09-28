using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using Assimp;

using fin.model;

namespace fin.src.exporter.assimp.indirect {
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

      sc.Textures.Clear();
      sc.Materials.Clear();

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

      foreach (var finMaterial in model.MaterialManager.All) {
        var assMaterial = new Material {Name = finMaterial.Name};

        if (finMaterial is ILayerMaterial layerMaterial) {
          var addLayers =
              layerMaterial
                  .Layers
                  .Where(layer => layer.BlendMode == FinBlendMode.ADD)
                  .ToArray();
          var multiplyLayers =
              layerMaterial
                  .Layers
                  .Where(layer => layer.BlendMode == FinBlendMode.MULTIPLY)
                  .ToArray();

          if (addLayers.Length == 0) {
            throw new NotSupportedException("Expected to find an add layer!");
          }
          if (addLayers.Length > 1) {
            ;
          }
          if (addLayers.Length > 2) {
            throw new NotSupportedException("Too many add layers for GLTF!");
          }

          for (var i = 0; i < addLayers.Length; ++i) {
            var layer = addLayers[i];

            // TODO: Support flat color layers by generating a 1x1 clamped texture of that color.
            if (layer.ColorSource is ITexture finTexture) {
              var assTextureSlot = new TextureSlot {
                  FilePath = finTexture.Name + ".png",
                  WrapModeU = this.ConvertWrapMode_(finTexture.WrapModeU),
                  WrapModeV = this.ConvertWrapMode_(finTexture.WrapModeV)
              };

              // TODO: FBX doesn't support mirror. Blegh

              if (i == 0) {
                assTextureSlot.TextureType = TextureType.Diffuse;
              } else {
                assTextureSlot.TextureType = TextureType.Emissive;
              }

              // TODO: Set blend mode
              //assTextureSlot.Operation =

              assTextureSlot.UVIndex = layer.TexCoordIndex;

              // TODO: Set texture coord type

              assMaterial.AddMaterialTexture(assTextureSlot);
            }
          }

          // Meshes should already have material indices set.
          sc.Materials.Add(assMaterial);
        }
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