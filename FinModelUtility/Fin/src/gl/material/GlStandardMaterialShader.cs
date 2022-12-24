using System;

using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlStandardMaterialShader : IGlMaterialShader {
    private readonly GlShaderProgram impl_;

    private GlTexture diffuseTexture_;
    private GlTexture normalTexture_;

    public GlStandardMaterialShader(IStandardMaterial standardMaterial) {
      this.Material = standardMaterial;

      var vertexShaderSrc = @"
# version 120

varying vec4 vertexPosition;
varying vec4 vertexColor;
varying vec3 vertexNormal;
varying vec2 normalUv;
varying vec2 uv;

void main() {
    vertexPosition = gl_ModelViewMatrix * gl_Vertex;
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;

    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    normalUv = normalize(gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(gl_Normal, 0)).xy;
    vertexColor = gl_Color;
    uv = gl_MultiTexCoord0.st;
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
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

      var diffuseTexture = standardMaterial.DiffuseTexture;
      this.diffuseTexture_ = diffuseTexture != null
                                 ? GlTexture.FromTexture(diffuseTexture)
                                 : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var normalTexture = standardMaterial.NormalTexture;
      this.normalTexture_ = normalTexture != null
                                 ? GlTexture.FromTexture(normalTexture)
                                 : GlMaterialConstants.NULL_GRAY_TEXTURE;
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      GlMaterialConstants.DisposeIfNotCommon(this.diffuseTexture_);
      GlMaterialConstants.DisposeIfNotCommon(this.normalTexture_);
    }


    public IMaterial Material { get; }

    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      var diffuseTextureLocation =
          this.impl_.GetUniformLocation("diffuseTexture");
      GL.Uniform1(diffuseTextureLocation, 0);
      this.diffuseTexture_.Bind(0);

      var normalTextureLocation =
          this.impl_.GetUniformLocation("normalTexture");
      GL.Uniform1(normalTextureLocation, 1);
      this.normalTexture_.Bind(1);
    }
  }
}