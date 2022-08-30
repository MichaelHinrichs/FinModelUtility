using System;
using System.Linq;

using fin.util.image;


namespace fin.model.util {
  public static class PrimaryTextureFinder {
    public static ITexture? GetFor(IMaterial material) {
      if (material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return PrimaryTextureFinder.GetFor(fixedFunctionMaterial);
      }
      if (material is ITextureMaterial textureMaterial) {
        return PrimaryTextureFinder.GetFor(textureMaterial);
      }
      if (material is IStandardMaterial standardMaterial) {
        return PrimaryTextureFinder.GetFor(standardMaterial);
      }

      throw new NotImplementedException();
    }

    public static ITexture? GetFor(ITextureMaterial material)
      => material.Texture;

    public static ITexture? GetFor(IFixedFunctionMaterial material) {
      var equations = material.Equations;

      var textures = material.Textures;

      // TODO: Use some kind of priority class

      var compiledTexture = material.CompiledTexture;
      if (compiledTexture != null) {
        return compiledTexture;
      }

      var prioritizedTextures = textures
                                // Sort by UV type, "normal" first
                                .OrderByDescending(
                                    texture =>
                                        texture.ColorType == ColorType.COLOR)
                                .ThenByDescending(
                                    texture =>
                                        ImageUtil.GetTransparencyType(
                                            texture.Image) ==
                                        ImageTransparencyType.OPAQUE)
                                .ToArray();

      if (prioritizedTextures.Length > 0) {
        // TODO: First or last?
        return prioritizedTextures[0];
      }

      return material.Textures.LastOrDefault((ITexture?) null);

      // TODO: Prioritize textures w/ color rather than intensity
      // TODO: Prioritize textures w/ standard texture sets
    }

    public static ITexture? GetFor(IStandardMaterial material)
      => material.DiffuseTexture ?? material.AmbientOcclusionTexture;
  }
}