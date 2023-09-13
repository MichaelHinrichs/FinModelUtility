# version 330

uniform sampler2D texture0;

uniform vec3 color_GxColor0;
uniform float scalar_GxAlpha0;
in vec4 vertexColor0;
in vec4 vertexColor1;
in vec2 uv0;

out vec4 fragColor;

void main() {
  vec3 colorComponent = clamp(color_GxColor0, 0, 1);

  float alphaComponent = scalar_GxAlpha0*texture(texture0, uv0).a;

  fragColor = vec4(colorComponent, alphaComponent);
}
