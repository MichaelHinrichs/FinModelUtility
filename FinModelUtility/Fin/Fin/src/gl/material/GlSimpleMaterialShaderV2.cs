using System.Linq;
using System.Text;

using fin.math;
using fin.model;
using fin.model.util;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderSource : IGlMaterialShaderSource {
    public GlSimpleMaterialShaderSource(IModel model,
                                        IReadOnlyMaterial material,
                                        bool useBoneMatrices) {
      this.VertexShaderSource =
          CommonShaderPrograms.GetVertexSrc(model, useBoneMatrices);

      var hasNormals =
          model.Skin.Meshes
               .SelectMany(mesh => mesh.Primitives)
               .Where(primitive => primitive.Material == material)
               .SelectMany(primitive => primitive.Vertices)
               .Any(vertex => vertex is IReadOnlyNormalVertex {
                   LocalNormal: { }
               });

      var fragmentSrc = new StringBuilder();
      fragmentSrc.Append(@"# version 330
");

      if (hasNormals) {
        fragmentSrc.Append(@"
struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform vec3 ambientLightColor;
uniform Light lights[{MaterialConstants.MAX_LIGHTS}];
");
      }

      fragmentSrc.Append(@$"
uniform sampler2D diffuseTexture;
uniform float {ShaderConstants.UNIFORM_USE_LIGHTING_NAME};

out vec4 fragColor;

in vec4 vertexColor0;");

      if (hasNormals) {
        fragmentSrc.Append(@"
in vec3 vertexNormal;");
      }

      fragmentSrc.Append(@"
in vec2 uv0;
");

      if (hasNormals) {
        fragmentSrc.Append($@"
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

  vec3 mergedLightColor = min(ambientLightColor + mergedDiffuseLightColor, 1);
  return diffuseColor * mergedLightColor;
}}
");
      }

      fragmentSrc.Append(@"
void main() {
  vec4 diffuseColor = texture(diffuseTexture, uv0);

  fragColor = diffuseColor * vertexColor0;");

      if (hasNormals) {
        fragmentSrc.Append($@"
  fragColor.rgb =
      mix(fragColor.rgb, applyLightingColor(fragColor.rgb, vertexNormal),  {ShaderConstants.UNIFORM_USE_LIGHTING_NAME});
");
      }

      fragmentSrc.Append(@"
  if (fragColor.a < .95) {
    discard;
  }
}");

      this.FragmentShaderSource = fragmentSrc.ToString();
    }

    public string VertexShaderSource { get; }
    public string FragmentShaderSource { get; }
  }

  public class GlSimpleMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial> {
    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(
        IModel model,
        IReadOnlyMaterial material,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, material, boneTransformManager, lighting) {
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
        GlShaderProgram shaderProgram) { }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) {
      // 0 is inferred, and therefore unnecessary to pass in.
      // GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();
    }
  }
}