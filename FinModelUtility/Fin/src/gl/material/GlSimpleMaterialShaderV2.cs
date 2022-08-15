using System;

using fin.model;
using fin.model.util;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_;
    private readonly int diffuseTextureLocation_;

    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(IMaterial material) {
      this.Material = material;

      var vertexShaderSrc = @"
# version 120

in vec3 in_Position;
in vec3 in_Normal;
in vec2 in_Uv0;

varying vec4 vertexPosition;
varying vec4 vertexColor;
varying vec3 vertexNormal;
varying vec2 normalUv;
varying vec2 uv;

void main() {
    vertexPosition = gl_ModelViewMatrix * vec4(in_Position, 1);
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(in_Position, 1);

    vertexNormal = normalize(gl_ModelViewMatrix * vec4(in_Normal, 0)).xyz;
    normalUv = normalize(gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(in_Normal, 0)).xy;
    vertexColor = vec4(1);
    uv = in_Uv0;
}";

      var fragmentShaderSrc = @$"
# version 130 

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

    public void Use() {
      this.impl_.Use();

      GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();
    }
  }
}