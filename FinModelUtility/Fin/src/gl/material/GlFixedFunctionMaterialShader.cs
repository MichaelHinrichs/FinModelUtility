using System;
using System.Collections.Generic;

using fin.language.equations.fixedFunction;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlFixedFunctionMaterialShader : IGlMaterialShader {
    private readonly GlShaderProgram impl_;

    private readonly IList<GlTexture> textures_;

    public GlFixedFunctionMaterialShader(
        IFixedFunctionMaterial fixedFunctionMaterial) {
      this.Material = fixedFunctionMaterial;

      // TODO: Sometimes vertex colors are passed in from model, and sometimes they
      // represent lighting. How to tell the difference??

      var vertexShaderSrc = @"
# version 120

in vec2 in_uv0;

varying vec3 vertexNormal;
varying vec2 normalUv;
varying vec4 vertexColor0;
varying vec4 vertexColor1;
varying vec2 uv0;
varying vec2 uv1;
varying vec2 uv2;
varying vec2 uv3;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    normalUv = normalize(gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(gl_Normal, 0)).xy;
    vertexColor0 = gl_Color;
    vertexColor1 = vec4(0, 0, 0, 1);
    uv0 = gl_MultiTexCoord0.st;
    uv1 = gl_MultiTexCoord1.st;
    uv2 = gl_MultiTexCoord2.st;
    uv3 = gl_MultiTexCoord3.st;
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