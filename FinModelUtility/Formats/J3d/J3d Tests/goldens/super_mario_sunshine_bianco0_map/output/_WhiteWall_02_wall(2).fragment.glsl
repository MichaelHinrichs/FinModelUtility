# version 400


struct Texture {
  sampler2D sampler;
  vec2 clampS;
  vec2 clampT;
  mat2x3 transform;
};
uniform Texture texture0;
uniform Texture texture1;

in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;
in vec2 uv1;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp((texture(texture0.sampler, clamp((texture0.transform * uv0).xy, vec2(texture0.clampS.x, texture0.clampT.x), vec2(texture0.clampS.y, texture0.clampT.y))).rgb*vec3(vertexColor0.a) + texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).rgb*(vec3(1) + vec3(-1)*vec3(vertexColor0.a)))*vertexColor0.rgb, 0, 1);

  float alphaComponent = (texture(texture0.sampler, clamp((texture0.transform * uv0).xy, vec2(texture0.clampS.x, texture0.clampT.x), vec2(texture0.clampS.y, texture0.clampT.y))).a*0.4980392156862745 + texture(texture1.sampler, clamp((texture1.transform * uv1).xy, vec2(texture1.clampS.x, texture1.clampT.x), vec2(texture1.clampS.y, texture1.clampT.y))).a*(1 + -1*0.4980392156862745))*vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);
}
