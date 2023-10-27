# version 400


struct Texture {
  sampler2D sampler;
  vec2 clampMin;
  vec2 clampMax;
  mat2x3 transform;
};
uniform Texture texture0;
uniform vec3 color_GxMaterialColor0;
uniform float scalar_GxMaterialAlpha0;

in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(texture(texture0.sampler, clamp((texture0.transform * uv0).xy, texture0.clampMin, texture0.clampMax)).rgb*color_GxMaterialColor0, 0, 1);

  float alphaComponent = texture(texture0.sampler, clamp((texture0.transform * uv0).xy, texture0.clampMin, texture0.clampMax)).a*scalar_GxMaterialAlpha0;

  fragColor = vec4(colorComponent, alphaComponent);
}
