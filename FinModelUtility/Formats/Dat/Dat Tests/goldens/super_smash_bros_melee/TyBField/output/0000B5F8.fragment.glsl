# version 400

in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = vec3(2);

  float alphaComponent = 1;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.95)) {
    discard;
  }
}
