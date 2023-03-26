using System;
using System.Collections.Generic;

using fin.language.equations.fixedFunction;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlFixedFunctionMaterialShaderV2 : IGlMaterialShader {
    private const int NUM_SUPPORTED_TEXTURES = 8;

    private readonly GlShaderProgram impl_;

    private readonly int modelViewMatrixLocation_;
    private readonly int projectionMatrixLocation_;
    private readonly int[] textureLocations_ = new int[NUM_SUPPORTED_TEXTURES];

    private readonly IList<GlTexture> textures_;

    public GlFixedFunctionMaterialShaderV2(
        IReadOnlyFixedFunctionMaterial fixedFunctionMaterial) {
      this.Material = fixedFunctionMaterial;

      // TODO: Sometimes vertex colors are passed in from model, and sometimes they
      // represent lighting. How to tell the difference??

      var pretty =
          new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
              .Print(fixedFunctionMaterial.Equations);

      var fragmentShaderSrc =
          new FixedFunctionEquationsGlslPrinter(
                  fixedFunctionMaterial.TextureSources)
              .Print(fixedFunctionMaterial);

      this.impl_ =
          GlShaderProgram.FromShaders(CommonShaderPrograms.VERTEX_SRC, fragmentShaderSrc);

      this.modelViewMatrixLocation_ = this.impl_.GetUniformLocation("modelViewMatrix");
      this.projectionMatrixLocation_ = this.impl_.GetUniformLocation("projectionMatrix");
      for (var i = 0; i < NUM_SUPPORTED_TEXTURES; ++i) {
        textureLocations_[i] = this.impl_.GetUniformLocation($"texture{i}");
      }

      var finTextures = fixedFunctionMaterial.TextureSources;

      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < NUM_SUPPORTED_TEXTURES; ++i) {
        var finTexture = i < (finTextures?.Count ?? 0)
                             ? finTextures[i]
                             : null;

        this.textures_.Add(finTexture != null
                               ? GlTexture.FromTexture(finTexture)
                               : GlMaterialConstants.NULL_WHITE_TEXTURE);
      }
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      foreach (var texture in this.textures_) {
        GlMaterialConstants.DisposeIfNotCommon(texture);
      }
      this.textures_.Clear();
    }


    public IReadOnlyMaterial Material { get; }

    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      var modelViewMatrix = GlTransform.ModelViewMatrix;
      GlTransform.UniformMatrix4(this.modelViewMatrixLocation_,
                        modelViewMatrix);

      var projectionMatrix = GlTransform.ProjectionMatrix;
      GlTransform.UniformMatrix4(this.projectionMatrixLocation_, projectionMatrix);

      for (var t = 0; t < NUM_SUPPORTED_TEXTURES; ++t) {
        GL.Uniform1(textureLocations_[t], t);
      }
      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }
    }
  }
}