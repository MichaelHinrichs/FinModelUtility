# version 400


struct Texture {
  sampler2D sampler;
  vec2 clampS;
  vec2 clampT;
  mat2x3 transform;
};
uniform Texture texture0;

in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(texture(texture0.sampler, clamp((texture0.transform * uv0).xy, vec2(texture0.clampS.x, texture0.clampT.x), vec2(texture0.clampS.y, texture0.clampT.y))).rgb*vertexColor0.rgb, 0, 1);

  float alphaComponent = texture(texture0.sampler, clamp((texture0.transform * uv0).xy, vec2(texture0.clampS.x, texture0.clampT.x), vec2(texture0.clampS.y, texture0.clampT.y))).a*vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.5019608)) {
    discard;
  }
}
