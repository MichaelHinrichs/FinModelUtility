﻿using System;
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

      vertexSrc.Append($"""

                        # version 330

                        uniform mat4 {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME};
                        uniform mat4 {GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME};
                        """);

      if (useBoneMatrices) {
        vertexSrc.Append($"""

                          uniform mat4 {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[{1 + model.Skin.BoneWeights.Count}];
                          """);
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

out vec3 vertexPosition;
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

  vec4 rawVertexPosition = vertexModelMatrix * vec4(in_Normal, 1);
  vertexPosition = rawVertexPosition.xyz / rawVertexPosition.w;
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
        vertexSrc.Append($"""
                          
                            vertexColor{i} = in_Colors[{i}];
                          """);
      }

      vertexSrc.Append(@"
}");

      return vertexSrc.ToString();
    }

    public static string GetLightHeader(bool withAmbientLight) {
      return
          $$"""

            struct Light {
              bool enabled;
  
              int sourceType;
              vec3 position;
              vec3 normal;

              vec4 color;
              
              int diffuseFunction;
              int attenuationFunction;
              vec3 cosineAttenuation;
              vec3 distanceAttenuation;
            };

            uniform Light lights[{{MaterialConstants.MAX_LIGHTS}}];
            {{(withAmbientLight ? """
                                  uniform vec4 ambientLightColor;
                                  """ : "")}}
            """;
    }

    public static string GetIndividualLightColorsFunction() {
      // Shamelessly stolen from:
      // https://github.com/LordNed/JStudio/blob/93c5c4479ffb1babefe829cfc9794694a1cb93e6/JStudio/J3D/ShaderGen/VertexShaderGen.cs#L336C9-L336C9
      return
          $$"""
            void getLightNormalAndAttenuation(Light light, vec3 position, vec3 normal, out vec3 lightNormal, out float attenuation) {
              vec3 surfaceToLight = light.position - position;
              
              lightNormal = (light.sourceType == {{(int) LightSourceType.LINE}}) 
                ? light.normal : normalize(surfaceToLight);
            
              if (light.attenuationFunction == {{(int) AttenuationFunction.NONE}}) {
                attenuation = 1;
                return;
              }
              

              // Attenuation is calculated as a fraction, (cosine attenuation) / (distance attenuation).
            
              // Numerator (Cosine attenuation)
              vec3 cosAttn = light.cosineAttenuation;
              
              vec3 attnDotLhs = (light.attenuationFunction == {{(int) AttenuationFunction.SPECULAR}})
                ? normal : lightNormal;
              float attn = dot(attnDotLhs, light.normal);
              vec3 attnPowers = vec3(1, attn, attn*attn);

              float attenuationNumerator = max(0, dot(cosAttn, attnPowers));

              // Denominator (Distance attenuation)
              float attenuationDenominator = 1;
              if (light.sourceType != {{(int) LightSourceType.LINE}}) {
                vec3 distAttn = light.distanceAttenuation;
                
                if (light.attenuationFunction == {{(int) AttenuationFunction.SPECULAR}}) {
                  float attn = max(0, dot(normal, light.normal));
                  if (light.diffuseFunction != {{(int) DiffuseFunction.NONE}}) {
                    distAttn = normalize(distAttn);
                  }
                  
                  attenuationDenominator = dot(distAttn, attnPowers);
                } else {
                  float dist2 = dot(surfaceToLight, surfaceToLight);
                  float dist = sqrt(dist2);
                  attenuationDenominator = dot(distAttn, vec3(1, dist, dist2));
                }
              }

              attenuation = attenuationNumerator / attenuationDenominator;
            }

            void getIndividualLightColors(Light light, vec3 position, vec3 normal, float shininess, out vec4 diffuseColor, out vec4 specularColor) {
              if (!light.enabled) {
                 diffuseColor = specularColor = vec4(0);
                 return;
              }
            
              vec3 lightNormal;
              float attenuation;
              getLightNormalAndAttenuation(light, position, normal, lightNormal, attenuation);
            
              float lightAmount = 1;
              if (light.diffuseFunction == {{(int) DiffuseFunction.SIGNED}} || light.diffuseFunction == {{(int) DiffuseFunction.CLAMP}}) {
                float diffuseLightAmount = max(-dot(normal, lightNormal), 0);
                lightAmount = min(diffuseLightAmount, 1);
              }
            
              diffuseColor = light.color * lightAmount * attenuation;
              
              specularColor = vec4(0);
            }
            """;
    }

    public static string GetMergedLightColorsFunctions(
        bool withAmbientOcclusion) {
      return
          $$"""

            void getMergedLightColors(vec3 position, vec3 normal, float shininess, out vec4 diffuseColor, out vec4 specularColor) {
              for (int i = 0; i < {{MaterialConstants.MAX_LIGHTS}}; ++i) {
                vec4 currentDiffuseColor;
                vec4 currentSpecularColor;
              
                getIndividualLightColors(lights[i], position, normal, shininess, currentDiffuseColor, currentSpecularColor);
            
                diffuseColor += currentDiffuseColor;
                specularColor += currentSpecularColor;
              }
            }

            vec4 applyMergedLightingColors(vec3 position, vec3 normal, float shininess, vec4 diffuseSurfaceColor, vec4 specularSurfaceColor{{(withAmbientOcclusion ? ", float ambientOcclusionAmount" : "")}}) {
              vec4 mergedDiffuseLightColor;
              vec4 mergedSpecularLightColor;
              getMergedLightColors(position, normal, shininess, mergedDiffuseLightColor, mergedSpecularLightColor);
            
              // We double it because all the other kids do. (Other fixed-function games.)
              vec4 diffuseComponent = 2 * diffuseSurfaceColor * ({{(withAmbientOcclusion ? "ambientOcclusionAmount * " : "")}}ambientLightColor + mergedDiffuseLightColor);
              vec4 specularComponent = specularSurfaceColor * mergedSpecularLightColor;
              
              return min(diffuseComponent + specularComponent, 1);
            }
            """;
    }


    public static string GetTextureStruct() {
      return
          """

          struct Texture {
            sampler2D sampler;
            vec2 clampMin;
            vec2 clampMax;
            mat2x3 transform2d;
            mat4 transform3d;
          };

          vec2 transformUv3d(mat4 transform3d, vec2 inUv) {
            vec4 rawTransformedUv = (transform3d * vec4(inUv, 0, 1));
          
            // We need to manually divide by w for perspective correction!
            return rawTransformedUv.xy / rawTransformedUv.w;
          }

          """;
    }

    public static string GetTypeOfTexture(ITexture? finTexture)
      => RequiresFancyTextureData(finTexture) ? "Texture" : "sampler2D";

    public static string ReadColorFromTexture(
        string textureName,
        string rawUvName,
        ITexture? finTexture)
      => ReadColorFromTexture(textureName, rawUvName, t => t, finTexture);

    public static string ReadColorFromTexture(
        string textureName,
        string rawUvName,
        Func<string, string> uvConverter,
        ITexture? finTexture) {
      if (!RequiresFancyTextureData(finTexture)) {
        return $"texture({textureName}, {uvConverter(rawUvName)})";
      }

      string transformedUv;
      if (!(finTexture?.IsTransform3d ?? false)) {
        transformedUv = $"({textureName}.transform2d * {rawUvName}).xy";
      } else {
        transformedUv =
            $"transformUv3d({textureName}.transform3d, {rawUvName})";
      }

      return
          $"texture({textureName}.sampler, " +
          "clamp(" +
          $"{uvConverter(transformedUv)}, " +
          $"{textureName}.clampMin, " +
          $"{textureName}.clampMax" +
          ")" + // clamp
          ")"; // texture
    }

    public static bool RequiresFancyTextureData(ITexture? finTexture)
      => finTexture != null &&
         (finTexture.WrapModeU == WrapMode.MIRROR_CLAMP ||
          finTexture.WrapModeV == WrapMode.MIRROR_CLAMP ||
          finTexture.Offset != null ||
          finTexture.RotationRadians != null ||
          finTexture.Scale != null ||
          finTexture.ClampS != null ||
          finTexture.ClampT != null);
  }
}