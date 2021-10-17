using System;
using System.Linq;

namespace fin.model.util {
  public static class PrimaryTextureFinder {
    public static ITexture? GetFor(IMaterial material) {
      if (material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return PrimaryTextureFinder.GetFor(fixedFunctionMaterial);
      }
      if (material is ITextureMaterial textureMaterial) {
        return PrimaryTextureFinder.GetFor(textureMaterial);
      }
      if (material is ILayerMaterial layerMaterial) {
        return PrimaryTextureFinder.GetFor(layerMaterial);
      }

      throw new NotImplementedException();
    }

    public static ITexture? GetFor(ITextureMaterial material)
      => material.Texture;

    public static ITexture? GetFor(IFixedFunctionMaterial material) {
      var equations = material.Equations;

      var textures = material.Textures;

      // TODO: Use some kind of priority class

      // TODO: Sometimes this needs to be a 

      var colorTextures = textures
                          .Where(
                              texture => texture.ColorType == ColorType.COLOR)
                          .ToArray();
      if (colorTextures.Length > 0) {
        // TODO: First or last?
        return colorTextures.Last();
      }

      // TODO: If only a const color, create a texture for that
      return material.Textures.Count > 0 ? material.Textures.Last() : null;

      // TODO: Prioritize textures w/ color rather than intensity
      // TODO: Prioritize textures w/ standard texture sets
    }

    public static ITexture? GetFor(ILayerMaterial material)
      => material.Textures.Count > 0 ? material.Textures[0] : null;
  }
}