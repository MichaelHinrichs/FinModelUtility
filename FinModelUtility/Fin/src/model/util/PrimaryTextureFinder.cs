using System;

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

      //var priorityQueue = new PriorityQueue<ITexture>();

      // TODO: If only a const color, create a texture for that
      return material.Textures.Count > 0 ? material.Textures[0] : null;

      // TODO: Prioritize textures w/ color rather than intensity
      // TODO: Prioritize textures w/ standard texture sets
    }

    public static ITexture? GetFor(ILayerMaterial material)
      => material.Textures.Count > 0 ? material.Textures[0] : null;
  }
}