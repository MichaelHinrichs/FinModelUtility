using System;

using fin.model;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlStandardMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_;

    private GlTexture diffuseTexture_;
    private GlTexture normalTexture_;
    private GlTexture ambientOcclusionTexture_;
    private GlTexture emissiveTexture_;

    public GlStandardMaterialShaderV2(IStandardMaterial standardMaterial) {
      this.Material = standardMaterial;

      var fragmentShaderSrc = @$"
# version 330 

uniform sampler2D diffuseTexture;
uniform sampler2D normalTexture;
uniform sampler2D ambientOcclusionTexture;
uniform sampler2D emissiveTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor0;
in vec3 vertexNormal;
in vec2 uv0;

void main() {{
    vec4 diffuseColor = texture(diffuseTexture, uv0);
    vec4 ambientOcclusionColor = texture(ambientOcclusionTexture, uv0);
    vec4 emissiveColor = texture(emissiveTexture, uv0);

    fragColor = diffuseColor * vertexColor0;

    vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
    float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);

    float ambientLightAmount = .3;

    float lightAmount = ambientOcclusionColor.r * min(ambientLightAmount + diffuseLightAmount, 1);

    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);
    fragColor.rgb += emissiveColor.rgb;

    fragColor.rgb = min(fragColor.rgb, 1);

    if (fragColor.a < .95) {{
      discard;
    }}
}}";

      /*

// compute derivations of the world position
    vec3 p_dx = dFdx(vertexPosition);
    vec3 p_dy = dFdy(vertexPosition);
    // compute derivations of the texture coordinate
    vec2 tc_dx = dFdx(uv);
    vec2 tc_dy = dFdy(uv);
    // compute initial tangent and bi-tangent
    vec3 t = normalize( tc_dy.y * p_dx - tc_dx.y * p_dy );
    vec3 b = normalize( tc_dy.x * p_dx - tc_dx.x * p_dy ); // sign inversion
    // get new tangent from a given mesh normal
    vec3 n = normalize(n_obj_i);
    vec3 x = cross(n, t);
    t = cross(x, n);
    t = normalize(t);
    // get updated bi-tangent
    x = cross(b, n);
    b = cross(n, x);
    b = normalize(b);
    mat3 tbn = mat3(t, b, n);



    vec4 diffuseColor = texture(diffuseTexture, uv);

    fragColor = diffuseColor * vertexColor;

    vec3 normalColor = texture(normalTexture, uv).rgb;
    vec3 fragNormal = normalize(2 * (normalColor - .5));
    vec3 normal_viewSpace = tbn * normalize((fragNormal * 2.0) - 1.0);
       
       */


      this.impl_ =
          GlShaderProgram.FromShaders(CommonShaderPrograms.VERTEX_SRC, fragmentShaderSrc);

      var diffuseTexture = standardMaterial.DiffuseTexture;
      this.diffuseTexture_ = diffuseTexture != null
                                 ? GlTexture.FromTexture(diffuseTexture)
                                 : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var normalTexture = standardMaterial.NormalTexture;
      this.normalTexture_ = normalTexture != null
                                ? GlTexture.FromTexture(normalTexture)
                                : GlMaterialConstants.NULL_GRAY_TEXTURE;

      var ambientOcclusionTexture = standardMaterial.AmbientOcclusionTexture;
      this.ambientOcclusionTexture_ =
          ambientOcclusionTexture != null
              ? GlTexture.FromTexture(ambientOcclusionTexture)
              : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var emissiveTexture = standardMaterial.EmissiveTexture;
      this.emissiveTexture_ = emissiveTexture != null
                                  ? GlTexture.FromTexture(emissiveTexture)
                                  : GlMaterialConstants.NULL_BLACK_TEXTURE;
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      GlMaterialConstants.DisposeIfNotCommon(this.diffuseTexture_);
      GlMaterialConstants.DisposeIfNotCommon(this.normalTexture_);
      GlMaterialConstants.DisposeIfNotCommon(this.ambientOcclusionTexture_);
      GlMaterialConstants.DisposeIfNotCommon(this.emissiveTexture_);
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

      var diffuseTextureLocation =
          this.impl_.GetUniformLocation("diffuseTexture");
      GL.Uniform1(diffuseTextureLocation, 0);
      this.diffuseTexture_.Bind(0);

      var normalTextureLocation =
          this.impl_.GetUniformLocation("normalTexture");
      GL.Uniform1(normalTextureLocation, 1);
      this.normalTexture_.Bind(1);

      var ambientOcclusionTextureLocation =
          this.impl_.GetUniformLocation("ambientOcclusionTexture");
      GL.Uniform1(ambientOcclusionTextureLocation, 2);
      this.ambientOcclusionTexture_.Bind(2);

      var emissiveTextureLocation =
          this.impl_.GetUniformLocation("emissiveTexture");
      GL.Uniform1(emissiveTextureLocation, 3);
      this.emissiveTexture_.Bind(3);

      var useLightingLocation = this.impl_.GetUniformLocation("useLighting");
      GL.Uniform1(useLightingLocation, this.UseLighting ? 1f : 0f);
    }
  }
}