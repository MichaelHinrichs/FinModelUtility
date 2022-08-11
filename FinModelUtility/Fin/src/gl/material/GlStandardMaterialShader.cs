using System;

using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlStandardMaterialShader : IGlMaterialShader {
    private readonly GlShaderProgram impl_;

    private GlTexture diffuseTexture_;

    public GlStandardMaterialShader(IStandardMaterial standardMaterial) {
      this.Material = standardMaterial;

      var vertexShaderSrc = @"
# version 120

in vec2 in_uv0;

varying vec3 vertexColor;
varying vec3 vertexNormal;
varying vec2 normalUv;
varying vec2 uv;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    normalUv = normalize(gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(gl_Normal, 0)).xy;
    vertexColor = gl_Color;
    uv = gl_MultiTexCoord0.st;
}";

      var fragmentShaderSrc = @$"
# version 130 

uniform sampler2D diffuseTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor;
in vec3 vertexNormal;
in vec2 uv;

void main() {{
    vec4 diffuseColor = texture(diffuseTexture, uv0);

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

      var diffuseTexture = standardMaterial.DiffuseTexture;
      this.diffuseTexture_ = diffuseTexture != null
                                 ? new GlTexture(diffuseTexture)
                                 : GlMaterialConstants.NULL_WHITE_TEXTURE;
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      GlMaterialConstants.DisposeIfNotCommon(this.diffuseTexture_);
    }


    public IMaterial Material { get; }

    public void Use() {
      this.impl_.Use();

      var diffuseTextureLocation =
          this.impl_.GetUniformLocation("diffuseTexture");
      GL.Uniform1(diffuseTextureLocation, 0);
      this.diffuseTexture_.Bind(0);
    }
  }
}