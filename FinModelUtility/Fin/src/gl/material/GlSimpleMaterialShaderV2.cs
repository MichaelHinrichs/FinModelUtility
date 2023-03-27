using System;

using fin.model;
using fin.model.util;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial> {
    private int diffuseTextureLocation_;
    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(IReadOnlyMaterial material) :
        base(material) {
      var primaryFinTexture = PrimaryTextureFinder.GetFor(material);
      this.primaryGlTexture_ = primaryFinTexture != null
          ? GlTexture.FromTexture(primaryFinTexture)
          : GlMaterialConstants.NULL_WHITE_TEXTURE;
    }

    protected override void DisposeInternal()
      => GlMaterialConstants.DisposeIfNotCommon(this.primaryGlTexture_);

    protected override GlShaderProgram GenerateShaderProgram(
        IReadOnlyMaterial material) {
      var fragmentShaderSrc = @$"
# version 330

uniform sampler2D diffuseTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor0;
in vec3 vertexNormal;
in vec2 uv0;

void main() {{
    vec4 diffuseColor = texture(diffuseTexture, uv0);

    fragColor = diffuseColor * vertexColor0;

    vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
    float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);

    float ambientLightAmount = .3;

    float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);

    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);

    if (fragColor.a < .95) {{
      discard;
    }}
}}";

      var impl = GlShaderProgram.FromShaders(CommonShaderPrograms.VERTEX_SRC, fragmentShaderSrc);
      this.diffuseTextureLocation_ = impl.GetUniformLocation("diffuseTexture");

      return impl;
    }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) {
      GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();
    }
  }
}