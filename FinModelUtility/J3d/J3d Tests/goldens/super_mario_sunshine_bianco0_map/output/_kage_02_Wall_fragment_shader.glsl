# version 330

uniform sampler2D texture0;

in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(vec3(0,0.1568627506494522,0.27450981736183167), 0, 1);

  float alphaComponent = texture(texture0, uv0).a;

  fragColor = vec4(colorComponent, alphaComponent);
}
