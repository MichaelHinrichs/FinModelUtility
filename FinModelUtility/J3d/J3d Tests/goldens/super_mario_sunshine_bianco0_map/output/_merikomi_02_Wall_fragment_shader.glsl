# version 330


in vec4 vertexColor0;
in vec4 vertexColor1;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(vertexColor0.rgb, 0, 1);

  float alphaComponent = vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);
}
