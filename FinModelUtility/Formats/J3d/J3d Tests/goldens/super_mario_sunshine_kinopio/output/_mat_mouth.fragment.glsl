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
uniform Texture texture0;
uniform Texture texture1;
uniform Texture texture2;
uniform sampler2D texture3;
uniform vec3 color_GxMaterialColor0;
uniform vec3 color_GxAmbientColor0;
uniform vec3 color_GxMaterialColor1;
uniform vec3 color_GxAmbientColor1;
uniform float scalar_GxMaterialAlpha1;

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

  vec3 colorComponent = clamp(clamp(texture(texture0.sampler, clamp((texture0.transform * uv0).xy, vec2(texture0.clampS.x, texture0.clampT.x), vec2(texture0.clampS.y, texture0.clampT.y))).rgb*(vec3(texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).a).g*65280 + vec3(texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).a).r*255 > 0.375*65280 + 0.375*255 ? vec3(1) : vec3(0)), 0, 1) + texture(texture2.sampler, clamp((texture2.transform * uv1).xy, vec2(texture2.clampS.x, texture2.clampT.x), vec2(texture2.clampS.y, texture2.clampT.y))).rgb*(vec3(1) + vec3(-1)*(vec3(texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).a).g*65280 + vec3(texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).a).r*255 > 0.375*65280 + 0.375*255 ? vec3(1) : vec3(0))) + clamp((vec3(0.5) + texture(texture3, uv0).rgb*(vec3(1) + vec3(-1)*vec3(0.5)) + color_GxMaterialColor0*clamp((individualLightColors[0].rgb + color_GxAmbientColor0), 0, 1)*vec3(0.5) + vec3(-0.5)), 0, 1)*(vec3(1) + vec3(-1)*vec3(0.2980392156862745,0.4235294117647059,0.3803921568627451)) + color_GxMaterialColor1*clamp((individualLightColors[2].rgb + clamp(color_GxAmbientColor1, 0, 1)), 0, 1)*vec3(0.2980392156862745,0.4235294117647059,0.3803921568627451) + vec3(-0.5), 0, 1);

  float alphaComponent = scalar_GxMaterialAlpha1;

  fragColor = vec4(colorComponent, alphaComponent);
}
