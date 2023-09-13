# version 330

uniform sampler2D texture0;

struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform Light lights[8];

uniform vec3 color_GxAmbientColor0;
uniform float scalar_GxAlpha2;
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

  vec3 colorComponent = clamp((vec3(scalar_GxAlpha2) + vertexColor0.rgb*clamp((individualLightColors[0].rgb + color_GxAmbientColor0), 0, 1))*vec3(2)*texture(texture0, uv0).rgb, 0, 1);

  float alphaComponent = texture(texture0, uv0).a;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.5019608)) {
    discard;
  }
}
