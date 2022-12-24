using System;
using System.Collections.Generic;

using fin.language.equations.fixedFunction;
using fin.model;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlFixedFunctionMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_;

    private readonly IList<GlTexture> textures_;

    public GlFixedFunctionMaterialShaderV2(
        IFixedFunctionMaterial fixedFunctionMaterial) {
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

      var finTextures = fixedFunctionMaterial.TextureSources;

      var nSupportedTextures = 8;
      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < nSupportedTextures; ++i) {
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


    public IMaterial Material { get; }

    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 modelViewMatrix);
      GL.UniformMatrix4(this.impl_.GetUniformLocation("modelViewMatrix"),
                        false, ref modelViewMatrix);

      GL.GetFloat(GetPName.ProjectionMatrix, out Matrix4 projectionMatrix);
      GL.UniformMatrix4(this.impl_.GetUniformLocation("projectionMatrix"),
                        false, ref projectionMatrix);

      for (var t = 0; t < 8; ++t) {
        var textureLocation =
            this.impl_.GetUniformLocation($"texture{t}");
        GL.Uniform1(textureLocation, t);
      }
      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }
    }
  }
}