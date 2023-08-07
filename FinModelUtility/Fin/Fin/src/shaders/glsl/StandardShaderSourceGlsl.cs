using System.Linq;
using System.Text;

using fin.model;

namespace fin.shaders.glsl {
  public class StandardShaderSourceGlsl : IShaderSourceGlsl {
    public StandardShaderSourceGlsl(
        IModel model,
        IStandardMaterial material,
        bool useBoneMatrices) {
      this.VertexShaderSource = GlslUtil.GetVertexSrc(model, useBoneMatrices);

      var hasNormalTexture = material.NormalTexture != null;
      var hasNormals = hasNormalTexture ||
                       model.Skin.Meshes
                            .SelectMany(mesh => mesh.Primitives)
                            .Where(primitive => primitive.Material == material)
                            .SelectMany(primitive => primitive.Vertices)
                            .Any(vertex => vertex is IReadOnlyNormalVertex {
                              LocalNormal: { }
                            });

      var fragmentShaderSrc = new StringBuilder();
      fragmentShaderSrc.Append(@"# version 330
");

      if (hasNormals) {
        fragmentShaderSrc.Append($@"
struct Light {{
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
}};

uniform vec3 ambientLightColor;
uniform Light lights[{MaterialConstants.MAX_LIGHTS}];
");
      }

      fragmentShaderSrc.Append(@"
uniform sampler2D diffuseTexture;");

      if (hasNormalTexture) {
        fragmentShaderSrc.Append("uniform sampler2D normalTexture;");
      }

      fragmentShaderSrc.Append(@$"
uniform sampler2D ambientOcclusionTexture;
uniform sampler2D emissiveTexture;
uniform float {GlslConstants.UNIFORM_USE_LIGHTING_NAME};

out vec4 fragColor;

in vec4 vertexColor0;");

      if (hasNormals) {
        fragmentShaderSrc.Append(@"
in vec3 vertexNormal;
in vec3 tangent;
in vec3 binormal;");
      }

      fragmentShaderSrc.Append(@$"
in vec2 uv0;
");

      if (hasNormals) {
        fragmentShaderSrc.Append($@"
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

vec3 applyLightingColor(vec3 diffuseColor, float ambientOcclusionAmount, vec3 vertexNormal) {{
  vec3 mergedDiffuseLightColor = getMergedDiffuseLightColor(vertexNormal);

  vec3 mergedLightColor = ambientOcclusionAmount * min(ambientLightColor + mergedDiffuseLightColor, 1);
  return diffuseColor * mergedLightColor;
}}
");
      }

      fragmentShaderSrc.Append(@"
void main() {
  vec4 diffuseColor = texture(diffuseTexture, uv0);
  vec4 ambientOcclusionColor = texture(ambientOcclusionTexture, uv0);
  vec4 emissiveColor = texture(emissiveTexture, uv0);

  fragColor = diffuseColor * vertexColor0;
");

      if (hasNormals) {
        if (!hasNormalTexture) {
          fragmentShaderSrc.Append(@"
  vec3 fragNormal = vertexNormal;");
        } else {
          fragmentShaderSrc.Append(@"
  vec3 textureNormal = texture(normalTexture, uv0).xyz * 2 - 1;    
  vec3 fragNormal = normalize(mat3(tangent, binormal, vertexNormal) * textureNormal);");
        }

        // TODO: Is this right?
        fragmentShaderSrc.Append(@$"
  fragColor.rgb = mix(fragColor.rgb, applyLightingColor(fragColor.rgb, ambientOcclusionColor.r, fragNormal), {GlslConstants.UNIFORM_USE_LIGHTING_NAME});
");
      }

      // TODO: Is this right?
      fragmentShaderSrc.Append(@"
  fragColor.rgb += emissiveColor.rgb;

  fragColor.rgb = min(fragColor.rgb, 1);

  if (fragColor.a < .95) {
    discard;
  }
}");

      this.FragmentShaderSource = fragmentShaderSrc.ToString();
    }

    public string VertexShaderSource { get; }
    public string FragmentShaderSource { get; set; }
  }
}
