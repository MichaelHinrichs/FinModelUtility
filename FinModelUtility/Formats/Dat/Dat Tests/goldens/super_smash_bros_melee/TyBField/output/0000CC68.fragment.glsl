#version 400


struct Texture {
  sampler2D sampler;
  vec2 clampMin;
  vec2 clampMax;
  mat2x3 transform2d;
  mat4 transform3d;
};

vec2 transformUv3d(mat4 transform3d, vec2 inUv) {
  vec4 rawTransformedUv = (transform3d * vec4(inUv, 0, 1));

  // We need to manually divide by w for perspective correction!
  return rawTransformedUv.xy / rawTransformedUv.w;
}

uniform Texture texture0;
uniform Texture texture1;

in vec2 normalUv;
in vec2 uv0;
in vec2 uv1;

out vec4 fragColor;

void main() {
  vec3 colorComponent = vec3(2)*texture(texture0.sampler, clamp(transformUv3d(texture0.transform3d, uv0), texture0.clampMin, texture0.clampMax)).rgb*texture(texture1.sampler, clamp(transformUv3d(texture1.transform3d, asin(normalUv) / 3.14159 + 0.5), texture1.clampMin, texture1.clampMax)).rgb;

  float alphaComponent = 1;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a > 0)) {
    discard;
  }
}
