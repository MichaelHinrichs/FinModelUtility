# version 400


struct Light {
  bool enabled;
  vec3 position;
  vec3 normal;
  vec4 color;
};

uniform Light lights[8];


struct Texture {
  sampler2D sampler;
  vec2 clampMin;
  vec2 clampMax;
  mat2x3 transform2d;
  mat4 transform3d;
};
uniform sampler2D texture0;
uniform Texture texture1;
uniform Texture texture2;
uniform vec3 color_GxAmbientColor0;

in vec2 normalUv;
in vec3 vertexNormal;
in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;
in vec2 uv1;

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

  vec3 colorComponent = clamp(vertexColor0.rgb*clamp((individualLightColors[0].rgb + color_GxAmbientColor0), 0, 1)*clamp((texture(texture2.sampler, clamp((texture2.transform2d * uv1).xy, texture2.clampMin, texture2.clampMax)).rgb + texture(texture1.sampler, clamp(acos((texture1.transform2d * normalUv).xy) / 3.14159, texture1.clampMin, texture1.clampMax)).rgb*texture(texture0, uv0).rgb*vertexColor0.rgb*clamp((individualLightColors[0].rgb + color_GxAmbientColor0), 0, 1)*vec3(0.5)*vec3(2)), 0, 1)*vec3(2), 0, 1);

  float alphaComponent = vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);
}
