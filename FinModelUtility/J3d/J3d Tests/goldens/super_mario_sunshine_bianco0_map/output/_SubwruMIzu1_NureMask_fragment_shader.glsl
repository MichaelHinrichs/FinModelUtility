# version 330

uniform sampler2D texture0;

in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(texture(texture0, uv0).rgb*vec3(0.21568627450980393,0.1568627450980392,0.11764705882352941), 0, 1);

  float alphaComponent = texture(texture0, uv0).a*0.19607843137254902;

  fragColor = vec4(colorComponent, alphaComponent);
}
