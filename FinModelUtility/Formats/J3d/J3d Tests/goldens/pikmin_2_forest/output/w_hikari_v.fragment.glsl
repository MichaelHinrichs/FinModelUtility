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
  vec2 clampS;
  vec2 clampT;
  mat2x3 transform;
};
uniform sampler2D texture0;
uniform Texture texture1;
uniform Texture texture2;
uniform vec3 color_GxAmbientColor0;
uniform vec3 color_GxMaterialColor0;
uniform vec3 color_GxColor6;
uniform vec3 color_GxColor5;
uniform float scalar_GxMaterialAlpha0;

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

  vec3 colorComponent = color_GxMaterialColor0*clamp((individualLightColors[0].rgb + color_GxAmbientColor0), 0, 1)*clamp((color_GxColor6*(vec3(1) + vec3(-1)*texture(texture0, uv0).rgb) + color_GxColor5*texture(texture0, uv0).rgb), 0, 1);

  float alphaComponent = texture(texture2.sampler, clamp((texture2.transform * uv1).xy, vec2(texture2.clampS.x, texture2.clampT.x), vec2(texture2.clampS.y, texture2.clampT.y))).a*texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).a*scalar_GxMaterialAlpha0*texture(texture0, uv0).a;

  fragColor = vec4(colorComponent, alphaComponent);
}
