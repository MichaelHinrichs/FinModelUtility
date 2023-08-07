using System.Linq;
using System.Text;

using fin.model;

namespace fin.shaders.glsl {
  public class SimpleShaderSourceGlsl : IShaderSourceGlsl {
    public SimpleShaderSourceGlsl(IModel model,
                                  IReadOnlyMaterial material,
                                  bool useBoneMatrices) {
      this.VertexShaderSource =
          GlslUtil.GetVertexSrc(model, useBoneMatrices);

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
        fragmentSrc.Append($@"
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

      fragmentSrc.Append(@$"
uniform sampler2D diffuseTexture;
uniform float {GlslConstants.UNIFORM_USE_LIGHTING_NAME};

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
      mix(fragColor.rgb, applyLightingColor(fragColor.rgb, vertexNormal),  {GlslConstants.UNIFORM_USE_LIGHTING_NAME});
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
}