using fin.model;
using fin.model.util;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderSource : IGlMaterialShaderSource {
    public GlSimpleMaterialShaderSource() {
      this.FragmentShaderSource = @$"# version 330

struct Light {{
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
}};

uniform Light lights[{MaterialConstants.MAX_LIGHTS}];

uniform sampler2D diffuseTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor0;
in vec3 vertexNormal;
in vec2 uv0;

vec3 getDiffuseLightColor(Light light, vec3 vertexNormal) {{
  vec3 diffuseLightNormal = normalize(light.normal);
  float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);
  float lightAmount = min(diffuseLightAmount, 1);
  return lightAmount * light.color.rgb;
}}

vec3 getMergedDiffuseLightColor(vec3 vertexNormal) {{
  int enabledLightCount;

  vec3 mergedLightColor;
  for (int i = 0; i < {MaterialConstants.MAX_LIGHTS}; ++i) {{
    if (lights[i].enabled) {{
      enabledLightCount++;
      mergedLightColor += getDiffuseLightColor(lights[i], vertexNormal);
    }}
  }}

  return enabledLightCount == 0 ? vec3(1) : mergedLightColor / enabledLightCount;
}}

vec3 applyLightingColor(vec3 diffuseColor, vec3 vertexNormal) {{
  vec3 mergedDiffuseLightColor = getMergedDiffuseLightColor(vertexNormal);

  vec3 ambientLightColor = vec3(1);
  float ambientLightAmount = .3;

  vec3 mergedLightColor = min(ambientLightAmount * ambientLightColor + mergedDiffuseLightColor, 1);
  return diffuseColor * mergedLightColor;
}}

void main() {{
  vec4 diffuseColor = texture(diffuseTexture, uv0);

  fragColor = diffuseColor * vertexColor0;

  vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
  float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);

  float ambientLightAmount = .3;

  float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);

  fragColor.rgb = mix(fragColor.rgb, applyLightingColor(fragColor.rgb, vertexNormal), useLighting);

  if (fragColor.a < .95) {{
    discard;
  }}
}}";
    }

    public string VertexShaderSource => CommonShaderPrograms.VERTEX_SRC;
    public string FragmentShaderSource { get; }
  }

  public class GlSimpleMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial> {
    private int diffuseTextureLocation_;
    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(IReadOnlyMaterial material,
                                    ILighting? lighting) :
        base(material, lighting) {
      var primaryFinTexture = PrimaryTextureFinder.GetFor(material);
      this.primaryGlTexture_ = primaryFinTexture != null
          ? GlTexture.FromTexture(primaryFinTexture)
          : GlMaterialConstants.NULL_WHITE_TEXTURE;
    }

    protected override void DisposeInternal() {
      if (this.DisposeTextures) {
        GlMaterialConstants.DisposeIfNotCommon(this.primaryGlTexture_);
      }
    }

    protected override void Setup(
        IReadOnlyMaterial material,
        GlShaderProgram shaderProgram) {
      this.diffuseTextureLocation_ =
          shaderProgram.GetUniformLocation("diffuseTexture");
    }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) {
      GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();
    }
  }
}