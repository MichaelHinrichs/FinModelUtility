using System;
using System.Collections.Generic;

using fin.language.equations.fixedFunction;
using fin.model;

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

      var vertexShaderSrc = @"
# version 120

in vec3 in_Position;
in vec3 in_Normal;
in vec2 in_Uvs[4];

varying vec3 vertexNormal;
varying vec2 normalUv;
varying vec4 vertexColor0_;
varying vec4 vertexColor1_;
varying vec2 uv0;
varying vec2 uv1;
varying vec2 uv2;
varying vec2 uv3;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(in_Position, 1);
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(in_Normal, 0)).xyz;
    normalUv = normalize(gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(in_Normal, 0)).xy;
    vertexColor0_ = vec4(0.5, 0.5, 0.5, 1);
    vertexColor1_ = vec4(0, 0, 0, 1);
    uv0 = in_Uvs[0].st;
    uv1 = in_Uvs[1].st;
    uv2 = in_Uvs[2].st;
    uv3 = in_Uvs[3].st;
}";

      var pretty =
          new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
              .Print(fixedFunctionMaterial.Equations);

      var fragmentShaderSrc =
          new FixedFunctionEquationsGlslPrinter(
                  fixedFunctionMaterial.TextureSources)
              .Print(fixedFunctionMaterial);

      this.impl_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

      var finTextures = fixedFunctionMaterial.TextureSources;

      var nSupportedTextures = 8;
      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < nSupportedTextures; ++i) {
        var finTexture = i < (finTextures?.Count ?? 0)
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