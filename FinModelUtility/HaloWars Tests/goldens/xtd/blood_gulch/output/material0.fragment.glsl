# version 330

struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform vec3 ambientLightColor;
uniform Light lights[8];

uniform sampler2D diffuseTexture;
uniform sampler2D ambientOcclusionTexture;
uniform sampler2D emissiveTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor0;
in vec3 vertexNormal;
in vec3 tangent;
in vec3 binormal;
in vec2 uv0;

vec3 getDiffuseLightColor(Light light, vec3 vertexNormal) {
  vec3 diffuseLightNormal = normalize(light.normal);
  float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);
  float lightAmount = min(diffuseLightAmount, 1);
  return lightAmount * light.color.rgb;
}

vec3 getMergedDiffuseLightColor(vec3 vertexNormal) {
  int enabledLightCount;

  vec3 mergedLightColor;
  for (int i = 0; i < 8; ++i) {
    if (lights[i].enabled) {
      enabledLightCount++;
      mergedLightColor += getDiffuseLightColor(lights[i], vertexNormal);
    }
  }

  return enabledLightCount == 0 ? vec3(1) : mergedLightColor / enabledLightCount;
}

vec3 applyLightingColor(vec3 diffuseColor, float ambientOcclusionAmount, vec3 vertexNormal) {
  vec3 mergedDiffuseLightColor = getMergedDiffuseLightColor(vertexNormal);

  vec3 mergedLightColor = ambientOcclusionAmount * min(ambientLightColor + mergedDiffuseLightColor, 1);
  return diffuseColor * mergedLightColor;
}

void main() {
  vec4 diffuseColor = texture(diffuseTexture, uv0);
  vec4 ambientOcclusionColor = texture(ambientOcclusionTexture, uv0);
  vec4 emissiveColor = texture(emissiveTexture, uv0);

  fragColor = diffuseColor * vertexColor0;

  vec3 fragNormal = vertexNormal;
  fragColor.rgb = mix(fragColor.rgb, applyLightingColor(fragColor.rgb, ambientOcclusionColor.r, fragNormal), useLighting);

  fragColor.rgb += emissiveColor.rgb;

  fragColor.rgb = min(fragColor.rgb, 1);

  if (fragColor.a < .95) {
    discard;
  }
}