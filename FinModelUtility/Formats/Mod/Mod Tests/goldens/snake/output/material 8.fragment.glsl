# version 400


struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform Light lights[8];
uniform vec3 ambientLightColor;

uniform sampler2D diffuseTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor0;
in vec3 vertexNormal;
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

vec3 applyLightingColor(vec3 diffuseColor, vec3 vertexNormal) {
  vec3 mergedDiffuseLightColor = getMergedDiffuseLightColor(vertexNormal);

  vec3 mergedLightColor = min(ambientLightColor + mergedDiffuseLightColor, 1);
  return diffuseColor * mergedLightColor;
}

void main() {
  vec4 diffuseColor = texture(diffuseTexture, uv0);

  fragColor = diffuseColor * vertexColor0;
  fragColor.rgb =
      mix(fragColor.rgb, applyLightingColor(fragColor.rgb, vertexNormal),  useLighting);

  if (fragColor.a < .95) {
    discard;
  }
}