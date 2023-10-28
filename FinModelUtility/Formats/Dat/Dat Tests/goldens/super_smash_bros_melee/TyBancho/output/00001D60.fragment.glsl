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

vec2 transformUv3d(mat4 transform3d, vec2 inUv) {
  vec4 rawTransformedUv = (transform3d * vec4(inUv, 0, 1));

  // We need to manually divide by w for perspective correction!
  return rawTransformedUv.xy / rawTransformedUv.w;
}

uniform Texture texture0;

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

  vec3 colorComponent = texture(texture0.sampler, clamp(transformUv3d(texture0.transform3d, uv0), texture0.clampMin, texture0.clampMax)).rgb*(vec3(1) + individualLightColors[0].rgb);

  float alphaComponent = 1;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.95)) {
    discard;
  }
}
