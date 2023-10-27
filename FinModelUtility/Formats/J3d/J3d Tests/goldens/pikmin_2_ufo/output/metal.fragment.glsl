# version 400

uniform vec3 color_GxColor7;

in vec4 vertexColor0;
in vec4 vertexColor1;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(color_GxColor7*vec3(2), 0, 1);

  float alphaComponent = 0;

  fragColor = vec4(colorComponent, alphaComponent);
}
