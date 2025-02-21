﻿using System;
using System.Numerics;
using System.Text;

using fin.model;
using fin.math.xyz;

namespace fin.shaders.glsl {
  public enum FinShaderType {
    FIXED_FUNCTION,
    TEXTURE,
    COLOR,
    STANDARD,
    NULL
  }

  public static class GlslUtil {
    public static FinShaderType
        GetShaderType(this IReadOnlyMaterial? material) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER &&
          !DebugFlags.ENABLE_WEIGHT_COLORS &&
          material is IFixedFunctionMaterial) {
        return FinShaderType.FIXED_FUNCTION;
      }

      if (material is IStandardMaterial) {
        return FinShaderType.STANDARD;
      }

      if (material is IColorMaterial) {
        return FinShaderType.COLOR;
      }

      if (material != null && material is not INullMaterial) {
        return FinShaderType.TEXTURE;
      }

      return FinShaderType.NULL;
    }

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

                        #version 330

                        uniform mat4 {GlslConstants.UNIFORM_MODEL_MATRIX_NAME};
                        uniform mat4 {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME};
                        uniform mat4 {GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME};
                        uniform vec3 {GlslConstants.UNIFORM_CAMERA_POSITION_NAME};
                        """);

      if (useBoneMatrices) {
        vertexSrc.Append($"""

                          uniform mat4 {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[{1 + model.Skeleton.Bones.Count}];
                          """);
      }

      vertexSrc.Append(@$"

layout(location = {location++}) in vec3 in_Position;
layout(location = {location++}) in vec3 in_Normal;
layout(location = {location++}) in vec4 in_Tangent;");

      if (useBoneMatrices) {
        vertexSrc.Append(
            $"layout(location = {location++}) in ivec4 in_BoneIds;");
        vertexSrc.Append(
            $"layout(location = {location++}) in vec4 in_BoneWeights;");
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

      vertexSrc.Append($$"""

                         void main() {
                           mat4 mvpMatrix = {{GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME}} * {{GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME}};
                         """);

      if (useBoneMatrices) {
        vertexSrc.Append($@"
  mat4 mergedBoneMatrix = {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[in_BoneIds.x] * in_BoneWeights.x +
                          {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[in_BoneIds.y] * in_BoneWeights.y +
                          {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[in_BoneIds.z] * in_BoneWeights.z +
                          {GlslConstants.UNIFORM_BONE_MATRICES_NAME}[in_BoneIds.w] * in_BoneWeights.w;

  mat4 vertexModelMatrix = {GlslConstants.UNIFORM_MODEL_MATRIX_NAME} * mergedBoneMatrix;
  mat4 projectionVertexModelMatrix = {GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME} * {GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME} * mergedBoneMatrix;

  gl_Position = projectionVertexModelMatrix * vec4(in_Position, 1);

  vertexPosition = vec3(vertexModelMatrix * vec4(in_Position, 1));
  vertexNormal = normalize(vertexModelMatrix * vec4(in_Normal, 0)).xyz;
  tangent = normalize(vertexModelMatrix * vec4(in_Tangent)).xyz;
  binormal = cross(vertexNormal, tangent);
  normalUv = normalize(projectionVertexModelMatrix * vec4(in_Normal, 0)).xy;");
      } else {
        vertexSrc.Append($@"
  gl_Position = mvpMatrix * vec4(in_Position, 1);
  vertexNormal = normalize({GlslConstants.UNIFORM_MODEL_MATRIX_NAME} * vec4(in_Normal, 0)).xyz;
  tangent = normalize({GlslConstants.UNIFORM_MODEL_MATRIX_NAME} * vec4(in_Tangent)).xyz;
  binormal = cross(vertexNormal, tangent); 
  normalUv = normalize(mvpMatrix * vec4(in_Normal, 0)).xy;");
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
            uniform vec3 {{GlslConstants.UNIFORM_CAMERA_POSITION_NAME}};
            """;
    }

    public static string GetGetIndividualLightColorsFunction() {
      // Shamelessly stolen from:
      // https://github.com/LordNed/JStudio/blob/93c5c4479ffb1babefe829cfc9794694a1cb93e6/JStudio/J3D/ShaderGen/VertexShaderGen.cs#L336C9-L336C9
      return
          $$"""
            void getSurfaceToLightNormalAndAttenuation(Light light, vec3 position, vec3 normal, out vec3 surfaceToLightNormal, out float attenuation) {
              vec3 surfaceToLight = light.position - position;
              
              surfaceToLightNormal = (light.sourceType == {{(int) LightSourceType.LINE}})
                ? -light.normal : normalize(surfaceToLight);
            
              if (light.attenuationFunction == {{(int) AttenuationFunction.NONE}}) {
                attenuation = 1;
                return;
              }
              
            
              // Attenuation is calculated as a fraction, (cosine attenuation) / (distance attenuation).
            
              // Numerator (Cosine attenuation)
              vec3 cosAttn = light.cosineAttenuation;
              
              vec3 attnDotLhs = (light.attenuationFunction == {{(int) AttenuationFunction.SPECULAR}})
                ? normal : surfaceToLightNormal;
              float attn = dot(attnDotLhs, light.normal);
              vec3 attnPowers = vec3(1, attn, attn*attn);
            
              float attenuationNumerator = max(0, dot(cosAttn, attnPowers));
            
              // Denominator (Distance attenuation)
              float attenuationDenominator = 1;
              if (light.sourceType != {{(int) LightSourceType.LINE}}) {
                vec3 distAttn = light.distanceAttenuation;
                
                if (light.attenuationFunction == {{(int) AttenuationFunction.SPECULAR}}) {
                  float attn = max(0, -dot(normal, light.normal));
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
            
              vec3 surfaceToLightNormal = vec3(0);
              float attenuation = 0;
              getSurfaceToLightNormalAndAttenuation(light, position, normal, surfaceToLightNormal, attenuation);
            
              float diffuseLightAmount = 1;
              if (light.diffuseFunction == {{(int) DiffuseFunction.SIGNED}} || light.diffuseFunction == {{(int) DiffuseFunction.CLAMP}}) {
                diffuseLightAmount = max(0, dot(normal, surfaceToLightNormal));
              }
              diffuseColor = light.color * diffuseLightAmount * attenuation;
              
              if (dot(normal, surfaceToLightNormal) >= 0) {
                vec3 surfaceToCameraNormal = normalize(cameraPosition - position);
                float specularLightAmount = pow(max(0, dot(reflect(-surfaceToLightNormal, normal), surfaceToCameraNormal)), {{GlslConstants.UNIFORM_SHININESS_NAME}});
                specularColor = light.color * specularLightAmount * attenuation;
              }
            }
            """;
    }

    public static string GetGetMergedLightColorsFunction() {
      return
          $$"""
            void getMergedLightColors(vec3 position, vec3 normal, float shininess, out vec4 diffuseColor, out vec4 specularColor) {
              for (int i = 0; i < {{MaterialConstants.MAX_LIGHTS}}; ++i) {
                vec4 currentDiffuseColor = vec4(0);
                vec4 currentSpecularColor = vec4(0);
              
                getIndividualLightColors(lights[i], position, normal, shininess, currentDiffuseColor, currentSpecularColor);
            
                diffuseColor += currentDiffuseColor;
                specularColor += currentSpecularColor;
              }
            }
            """;
    }

    public static string GetApplyMergedLightColorsFunction(
        bool withAmbientOcclusion) {
      return
          $$"""
            vec4 applyMergedLightingColors(vec3 position, vec3 normal, float shininess, vec4 diffuseSurfaceColor, vec4 specularSurfaceColor{{(withAmbientOcclusion ? ", float ambientOcclusionAmount" : "")}}) {
              vec4 mergedDiffuseLightColor = vec4(0);
              vec4 mergedSpecularLightColor = vec4(0);
              getMergedLightColors(position, normal, shininess, mergedDiffuseLightColor, mergedSpecularLightColor);
            
              // We double it because all the other kids do. (Other fixed-function games.)
              vec4 diffuseComponent = 2 * diffuseSurfaceColor * ({{(withAmbientOcclusion ? "ambientOcclusionAmount * " : "")}}ambientLightColor + mergedDiffuseLightColor);
              vec4 specularComponent = specularSurfaceColor * mergedSpecularLightColor;
              
              return clamp(diffuseComponent + specularComponent, 0, 1);
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
        transformedUv
            = $"({textureName}.transform2d * {uvConverter(rawUvName)}).xy";
      } else {
        transformedUv =
            $"transformUv3d({textureName}.transform3d, {uvConverter(rawUvName)})";
      }

      return
          $"texture({textureName}.sampler, " +
          "clamp(" +
          $"{transformedUv}, " +
          $"{textureName}.clampMin, " +
          $"{textureName}.clampMax" +
          ")" + // clamp
          ")";  // texture
    }

    public static bool RequiresFancyTextureData(ITexture? finTexture) {
      if (finTexture == null) {
        return false;
      }

      if (finTexture.WrapModeU == WrapMode.MIRROR_CLAMP ||
          finTexture.WrapModeV == WrapMode.MIRROR_CLAMP) {
        return true;
      }

      if (finTexture.Offset != null) {
        var offset = finTexture.Offset;
        if (!offset.IsRoughly0()) {
          return true;
        }
      }

      if (finTexture.RotationRadians != null) {
        var radians = finTexture.RotationRadians;
        if (!radians.IsRoughly0()) {
          return true;
        }
      }

      if (finTexture.Scale != null) {
        var scale = finTexture.Scale;
        if (!scale.IsRoughly1()) {
          return true;
        }
      }

      return finTexture.ClampS != null || finTexture.ClampT != null;
    }
  }
}