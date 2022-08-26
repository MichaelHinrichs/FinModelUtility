using System;

using fin.model;
using fin.model.util;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_;

    private readonly int diffuseTextureLocation_;

    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(IMaterial material) {
      this.Material = material;

      var vertexShaderSrc = @"
# version 330

uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;

layout(location = 0) in vec3 in_Position;
layout(location = 1) in vec3 in_Normal;
layout(location = 2) in vec2 in_Uvs[4];
layout(location = 6) in vec4 in_Colors[2];

out vec4 vertexPosition;
out vec4 vertexColor;
out vec3 vertexNormal;
out vec2 normalUv;
out vec2 uv;

void main() {
    vertexPosition = modelViewMatrix * vec4(in_Position, 1);
    gl_Position = projectionMatrix * modelViewMatrix * vec4(in_Position, 1);

    vertexNormal = normalize(modelViewMatrix * vec4(in_Normal, 0)).xyz;
    normalUv = normalize(projectionMatrix * modelViewMatrix * vec4(in_Normal, 0)).xy;
    vertexColor = in_Colors[0];
    uv = in_Uvs[0];
}";

      var fragmentShaderSrc = @$"
# version 330

uniform sampler2D diffuseTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexPosition;
in vec4 vertexColor;
in vec3 vertexNormal;
in vec2 uv;

void main() {{
    vec4 diffuseColor = texture(diffuseTexture, uv);

    fragColor = diffuseColor * vertexColor;

    vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
    float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);

    float ambientLightAmount = .3;

    float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);

    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);

    if (fragColor.a < .95) {{
      discard;
    }}
}}";

      this.impl_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

      this.diffuseTextureLocation_ =
          this.impl_.GetUniformLocation("diffuseTexture");

      var primaryFinTexture = PrimaryTextureFinder.GetFor(material);
      this.primaryGlTexture_ = primaryFinTexture != null
                                   ? new GlTexture(primaryFinTexture)
                                   : GlMaterialConstants.NULL_WHITE_TEXTURE;
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      GlMaterialConstants.DisposeIfNotCommon(this.primaryGlTexture_);
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

      GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();

      GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();

      GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();

      var useLightingLocation = this.impl_.GetUniformLocation("useLighting");
      GL.Uniform1(useLightingLocation, this.UseLighting ? 1f : 0f);
    }
  }
}