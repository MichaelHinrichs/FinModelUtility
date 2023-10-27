using System.Numerics;
using System.Text;

using fin.model;

namespace fin.shaders.glsl {
  public static class GlslUtil {
    public static TNumber UseThenAdd<TNumber>(ref TNumber value, TNumber delta)
        where TNumber : INumber<TNumber> {
      var initialValue = value;
      value += delta;
      return initialValue;
    }

    // TODO: Only include uvs/colors as needed
    public static string GetVertexSrc(IModel model, bool useBoneMatrices) {
      var location = 0;

      var vertexSrc = new StringBuilder();

      vertexSrc.Append($@"
# version 330

uniform mat4 {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME};
uniform mat4 {GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME};");

      if (useBoneMatrices) {
        vertexSrc.Append(@$"
uniform mat4 {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[{1 + model.Skin.BoneWeights.Count}];");
      }

      vertexSrc.Append(@$"

layout(location = {location++}) in vec3 in_Position;
layout(location = {location++}) in vec3 in_Normal;
layout(location = {location++}) in vec4 in_Tangent;");

      if (useBoneMatrices) {
        vertexSrc.Append(@$"
layout(location = {location++}) in int in_MatrixId;");
      }

      vertexSrc.Append(@$"
layout(location = {UseThenAdd(ref location, MaterialConstants.MAX_UVS)}) in vec2 in_Uvs[{MaterialConstants.MAX_UVS}];
layout(location = {UseThenAdd(ref location, MaterialConstants.MAX_COLORS)}) in vec4 in_Colors[{MaterialConstants.MAX_COLORS}];

out vec3 vertexNormal;
out vec3 tangent;
out vec3 binormal;
out vec2 normalUv;");

      for (var i = 0; i < MaterialConstants.MAX_UVS; ++i) {
        vertexSrc.Append($@"
out vec2 uv{i};");
      }

      for (var i = 0; i < MaterialConstants.MAX_COLORS; ++i) {
        vertexSrc.Append($@"
out vec4 vertexColor{i};");
      }

      vertexSrc.Append(@"
void main() {");

      if (useBoneMatrices) {
        vertexSrc.Append($@"
  mat4 vertexMatrix = {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[in_MatrixId];

  mat4 vertexModelMatrix = {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME} * vertexMatrix;
  mat4 projectionVertexModelMatrix = {GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME} * vertexModelMatrix;

  gl_Position = projectionVertexModelMatrix * vec4(in_Position, 1);
  vertexNormal = normalize(vertexModelMatrix * vec4(in_Normal, 0)).xyz;
  tangent = normalize(vertexModelMatrix * vec4(in_Tangent)).xyz;
  binormal = cross(vertexNormal, tangent);
  normalUv = normalize(projectionVertexModelMatrix * vec4(in_Normal, 0)).xy;");
      } else {
        vertexSrc.Append($@"
  gl_Position = {GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME} * {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME} * vec4(in_Position, 1);
  vertexNormal = normalize({GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME} * vec4(in_Normal, 0)).xyz;
  tangent = normalize({GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME} * vec4(in_Tangent)).xyz;
  binormal = cross(vertexNormal, tangent); 
  normalUv = normalize({GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME} * {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME} * vec4(in_Normal, 0)).xy;");
      }

      for (var i = 0; i < MaterialConstants.MAX_UVS; ++i) {
        vertexSrc.Append($@"
  uv{i} = in_Uvs[{i}];");
      }

      for (var i = 0; i < MaterialConstants.MAX_COLORS; ++i) {
        vertexSrc.Append($@"
  vertexColor{i} = in_Colors[{i}];");
      }

      vertexSrc.Append(@"
}");

      return vertexSrc.ToString();
    }

    public static string GetLightHeader(bool withAmbientLightColor) {
      return
          $$"""

            struct Light {
              bool enabled;
              vec3 position;
              vec3 normal;
              vec4 color;
            };

            uniform Light lights[{{MaterialConstants.MAX_LIGHTS}}];
            {{(withAmbientLightColor ? "uniform vec3 ambientLightColor;" : "")}}
            """;
    }

    public static string GetLightFunctions(bool withAmbientOcclusion) {
      return
          $$"""

            vec3 getDiffuseLightColor(Light light, vec3 vertexNormal) {
              vec3 diffuseLightNormal = normalize(light.normal);
              float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);
              float lightAmount = min(diffuseLightAmount, 1);
              return lightAmount * light.color.rgb;
            }

            vec3 getMergedDiffuseLightColor(vec3 vertexNormal) {
              int enabledLightCount;
            
              vec3 mergedLightColor;
              for (int i = 0; i < {{MaterialConstants.MAX_LIGHTS}}; ++i) {
                if (lights[i].enabled) {
                  enabledLightCount++;
                  mergedLightColor += getDiffuseLightColor(lights[i], vertexNormal);
                }
              }
            
              return enabledLightCount == 0 ? vec3(1) : mergedLightColor / enabledLightCount;
            }

            vec3 applyLightingColor(vec3 diffuseColor,{{(withAmbientOcclusion ? " float ambientOcclusionAmount," : "")}} vec3 vertexNormal) {
              vec3 mergedDiffuseLightColor = getMergedDiffuseLightColor(vertexNormal);
            
              vec3 mergedLightColor = min({{(withAmbientOcclusion ? "ambientOcclusionAmount * " : "")}}ambientLightColor + mergedDiffuseLightColor, 1);
              return diffuseColor * mergedLightColor;
            }
            """;
    }


    public static string GetTextureStruct() {
      return
          """

          struct Texture {
            sampler2D sampler;
            vec2 clampS;
            vec2 clampT;
            mat2x3 transform;
          };
          """;
    }

    public static string GetTypeOfTexture(ITexture? finTexture)
      => RequiresFancyTextureData(finTexture) ? "Texture" : "sampler2D";

    public static string ReadColorFromTexture(
        string textureName,
        string uvName,
        ITexture? finTexture) {
      if (!RequiresFancyTextureData(finTexture)) {
        return $"texture({textureName}, {uvName})";
      }

      return
          $"texture({textureName}.sampler, " +
          "clamp(" +
          $"({textureName}.transform * {uvName}).xy, " +
          $"vec2({textureName}.clampS.x, {textureName}.clampT.x), " +
          $"vec2({textureName}.clampS.y, {textureName}.clampT.y)" +
          ")" + // clamp
          ")"; // texture
    }

    public static bool RequiresFancyTextureData(ITexture? finTexture)
      => finTexture != null &&
         (finTexture.Offset != null ||
          finTexture.RotationRadians != null ||
          finTexture.Scale != null ||
          finTexture.ClampS != null ||
          finTexture.ClampT != null);
  }
}