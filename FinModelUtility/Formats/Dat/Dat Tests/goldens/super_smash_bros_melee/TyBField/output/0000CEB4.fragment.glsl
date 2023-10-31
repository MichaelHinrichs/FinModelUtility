# version 400


out vec4 fragColor;

void main() {
  vec3 colorComponent = vec3(2)*vec3(0.250980406999588);

  float alphaComponent = 0.75;

  fragColor = vec4(colorComponent, alphaComponent);
}
