using System;
using System.Collections.Generic;

using fin.language.equations.fixedFunction;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlFixedFunctionMaterialShaderV2
      : BGlMaterialShader<IReadOnlyFixedFunctionMaterial> {
    private int[] textureLocations_ =
        new int[MaterialConstants.MAX_TEXTURES];

    private IList<GlTexture> textures_;

    public GlFixedFunctionMaterialShaderV2(
        IModel model,
        IReadOnlyFixedFunctionMaterial fixedFunctionMaterial)
        : base(model, fixedFunctionMaterial) { }

    protected override void DisposeInternal() {
      foreach (var texture in this.textures_) {
        GlMaterialConstants.DisposeIfNotCommon(texture);
      }
    }

    protected override GlShaderProgram GenerateShaderProgram(
        IReadOnlyFixedFunctionMaterial material) {
      var fragmentShaderSrc =
          new FixedFunctionEquationsGlslPrinter(material.TextureSources)
              .Print(material);

      var impl =
          GlShaderProgram.FromShaders(CommonShaderPrograms.VERTEX_SRC,
                                      fragmentShaderSrc);

      for (var i = 0; i < MaterialConstants.MAX_TEXTURES; ++i) {
        this.textureLocations_[i] =
            impl.GetUniformLocation($"texture{i}");
      }

      var finTextures = material.TextureSources;

      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < MaterialConstants.MAX_TEXTURES; ++i) {
        var finTexture = i < (finTextures?.Count ?? 0)
            ? finTextures[i]
            : null;

        this.textures_.Add(finTexture != null
                               ? GlTexture.FromTexture(finTexture)
                               : GlMaterialConstants.NULL_WHITE_TEXTURE);
      }

      return impl;
    }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) {
      for (var t = 0; t < MaterialConstants.MAX_TEXTURES; ++t) {
        GL.Uniform1(textureLocations_[t], t);
      }

      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }
    }
  }
}