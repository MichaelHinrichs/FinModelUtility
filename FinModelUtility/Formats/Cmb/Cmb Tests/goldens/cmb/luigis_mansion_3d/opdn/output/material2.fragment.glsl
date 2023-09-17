# version 400


struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform Light lights[8];

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform vec3 color_3dsColor2;
uniform vec3 color_3dsColor3;
uniform float scalar_3dsAlpha1;

in vec2 normalUv;
in vec3 vertexNormal;
in vec4 vertexColor0;
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

  vec3 colorComponent = (color_3dsColor3 + vec3(texture(texture0, uv0).rgb.g))*(vec3(texture(texture1, uv0).rgb.b)*texture(texture2, asin(normalUv) / 3.14159 + 0.5).rgb + color_3dsColor2*vec3(1 + -1*vertexColor0.rgb.r*vertexColor0.rgb.r*2) + vertexColor0.rgb)*texture(texture1, uv0).rgb*vec3(2)*individualLightColors[0].rgb;

  float alphaComponent = vertexColor0.a*texture(texture1, uv0).a*scalar_3dsAlpha1;

  fragColor = vec4(colorComponent, alphaComponent);
}
