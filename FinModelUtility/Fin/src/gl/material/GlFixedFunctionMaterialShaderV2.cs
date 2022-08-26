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

      var vertexShaderSrc = @"
# version 330

uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;

layout(location = 0) in vec3 in_Position;
layout(location = 1) in vec3 in_Normal;
layout(location = 2) in vec2 in_Uvs[4];
layout(location = 6) in vec4 in_Colors[2];

out vec3 vertexNormal;
out vec2 normalUv;
out vec2 uv0;
out vec2 uv1;
out vec2 uv2;
out vec2 uv3;
out vec4 vertexColor0;
out vec4 vertexColor1;

void main() {
    gl_Position = projectionMatrix * modelViewMatrix * vec4(in_Position, 1);
    vertexNormal = normalize(modelViewMatrix * vec4(in_Normal, 0)).xyz;
    normalUv = normalize(projectionMatrix * modelViewMatrix * vec4(in_Normal, 0)).xy;
    vertexColor0 = in_Colors[0];
    vertexColor1 = in_Colors[1];
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