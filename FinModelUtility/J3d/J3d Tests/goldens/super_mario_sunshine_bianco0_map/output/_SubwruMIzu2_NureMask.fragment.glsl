# version 330


uniform vec3 color_GxMaterialColor0;
in vec4 vertexColor0;
in vec4 vertexColor1;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(vec3(vertexColor0.a)*color_GxMaterialColor0, 0, 1);

  float alphaComponent = vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);
}
