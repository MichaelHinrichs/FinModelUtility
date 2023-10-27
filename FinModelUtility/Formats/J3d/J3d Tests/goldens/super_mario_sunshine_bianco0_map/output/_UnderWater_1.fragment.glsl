# version 400


struct Texture {
  sampler2D sampler;
  vec2 clampMin;
  vec2 clampMax;
  mat2x3 transform2d;
  mat4 transform3d;
};
uniform Texture texture0;

in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(texture(texture0.sampler, clamp((texture0.transform2d * uv0).xy, texture0.clampMin, texture0.clampMax)).rgb*vertexColor0.rgb, 0, 1);

  float alphaComponent = texture(texture0.sampler, clamp((texture0.transform2d * uv0).xy, texture0.clampMin, texture0.clampMax)).a*vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);
}
