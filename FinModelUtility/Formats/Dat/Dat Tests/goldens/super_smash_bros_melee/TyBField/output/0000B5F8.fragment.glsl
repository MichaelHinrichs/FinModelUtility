# version 400


struct Texture {
  sampler2D sampler;
  vec2 clampMin;
  vec2 clampMax;
  mat2x3 transform2d;
  mat4 transform3d;
};
uniform Texture texture0;

in vec2 normalUv;
in vec4 vertexColor0;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = texture(texture0.sampler, clamp(asin((texture0.transform3d * vec4(normalUv, 0, 1)).xy) / 3.14159 + 0.5, texture0.clampMin, texture0.clampMax)).rgb;

  float alphaComponent = 1;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.95)) {
    discard;
  }
}
