# version 330

uniform sampler2D texture0;
uniform sampler2D texture2;
uniform sampler2D texture3;

struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform Light lights[8];

uniform vec3 color_GxMaterialColor0;
uniform vec3 color_GxAmbientColor0;
uniform float scalar_GxAlpha0;
in vec2 normalUv;
in vec3 vertexNormal;
in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

vec4 getLightColor(Light light) {
  if (!light.enabled) {
    return vec4(0);
  }

  vec3 diffuseLightNormal = normalize(light.normal);
  float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);
  float lightAmount = min(diffuseLightAmount, 1);
  return lightAmount * light.color;
}

void main() {
  vec4 individualLightColors[8];
  for (int i = 0; i < 8; ++i) {
    vec4 lightColor = getLightColor(lights[i]);
    individualLightColors[i] = lightColor;
  }

  vec3 colorComponent = clamp(vec3(0.5)*texture(texture2, acos(normalUv) / 3.14159).rgb + vec3(0.5) + color_GxMaterialColor0*clamp((individualLightColors[0].rgb + color_GxAmbientColor0), 0, 1)*texture(texture0, uv0).rgb*vec3(2)*clamp((vec3(0.5)*texture(texture2, acos(normalUv) / 3.14159).rgb + vec3(0.5)), 0, 1) + vec3(-0.5), 0, 1);

  float alphaComponent = texture(texture3, uv0).a + -1*(1 + -1*scalar_GxAlpha0);

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.003921569)) {
    discard;
  }
}
