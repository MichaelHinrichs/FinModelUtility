# version 400

in vec4 vertexColor0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = vertexColor0.rgb;

  float alphaComponent = 1;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.95)) {
    discard;
  }
}
