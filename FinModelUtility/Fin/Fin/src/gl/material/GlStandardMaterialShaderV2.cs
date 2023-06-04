using System.Linq;
using System.Text;

using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlStandardMaterialShaderSource : IGlMaterialShaderSource {
    public GlStandardMaterialShaderSource(
        IModel model,
        IStandardMaterial material,
        bool useBoneMatrices) {
      this.VertexShaderSource =
          CommonShaderPrograms.GetVertexSrc(model, useBoneMatrices);

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
uniform float {ShaderConstants.UNIFORM_USE_LIGHTING_NAME};

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
  fragColor.rgb = mix(fragColor.rgb, applyLightingColor(fragColor.rgb, ambientOcclusionColor.r, fragNormal), {ShaderConstants.UNIFORM_USE_LIGHTING_NAME});
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

  public class GlStandardMaterialShaderV2
      : BGlMaterialShader<IStandardMaterial> {
    private GlTexture diffuseTexture_;
    private GlTexture normalTexture_;
    private GlTexture ambientOcclusionTexture_;
    private GlTexture emissiveTexture_;

    public GlStandardMaterialShaderV2(
        IModel model,
        IStandardMaterial standardMaterial,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, standardMaterial, boneTransformManager, lighting) { }

    protected override void DisposeInternal() {
      if (this.DisposeTextures) {
        GlMaterialConstants.DisposeIfNotCommon(this.diffuseTexture_);
        GlMaterialConstants.DisposeIfNotCommon(this.normalTexture_);
        GlMaterialConstants.DisposeIfNotCommon(this.ambientOcclusionTexture_);
        GlMaterialConstants.DisposeIfNotCommon(this.emissiveTexture_);
      }
    }

    protected override void Setup(IStandardMaterial material,
                                  GlShaderProgram shaderProgram) {
      var diffuseTexture = material.DiffuseTexture;
      this.diffuseTexture_ = diffuseTexture != null
          ? GlTexture.FromTexture(diffuseTexture)
          : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var normalTexture = material.NormalTexture;
      this.normalTexture_ = normalTexture != null
          ? GlTexture.FromTexture(normalTexture)
          : GlMaterialConstants.NULL_GRAY_TEXTURE;

      var ambientOcclusionTexture = material.AmbientOcclusionTexture;
      this.ambientOcclusionTexture_ =
          ambientOcclusionTexture != null
              ? GlTexture.FromTexture(ambientOcclusionTexture)
              : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var emissiveTexture = material.EmissiveTexture;
      this.emissiveTexture_ = emissiveTexture != null
          ? GlTexture.FromTexture(emissiveTexture)
          : GlMaterialConstants.NULL_BLACK_TEXTURE;
    }

    protected override void PassUniformsAndBindTextures(GlShaderProgram impl) {
      var diffuseTextureLocation =
          impl.GetUniformLocation("diffuseTexture");
      GL.Uniform1(diffuseTextureLocation, 0);
      this.diffuseTexture_.Bind(0);

      var normalTextureLocation =
          impl.GetUniformLocation("normalTexture");
      GL.Uniform1(normalTextureLocation, 1);
      this.normalTexture_.Bind(1);

      var ambientOcclusionTextureLocation =
          impl.GetUniformLocation("ambientOcclusionTexture");
      GL.Uniform1(ambientOcclusionTextureLocation, 2);
      this.ambientOcclusionTexture_.Bind(2);

      var emissiveTextureLocation =
          impl.GetUniformLocation("emissiveTexture");
      GL.Uniform1(emissiveTextureLocation, 3);
      this.emissiveTexture_.Bind(3);
    }
  }
}