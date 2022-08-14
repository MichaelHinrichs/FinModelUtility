using System;
using System.Collections.Generic;
using System.Linq;

using fin.model;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_;
   
    private readonly IList<GlTexture> textures_;

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
    vertexColor = gl_Color;
    uv = in_Uv0;
}";

      var fragmentShaderSrc = @$"
# version 130 

uniform sampler2D diffuseTexture;
uniform sampler2D normalTexture;
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

      var finTextures = material.Textures.ToArray();

      var nSupportedTextures = 8;
      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < nSupportedTextures; ++i) {
        var finTexture = i < (finTextures?.Length ?? 0)
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

    public void Use() {
      this.impl_.Use();

      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }
    }
  }
}