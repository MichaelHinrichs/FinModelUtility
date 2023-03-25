namespace fin.gl.material {
  public static class CommonShaderPrograms {
    private static GlShaderProgram? texturelessShaderProgram_;

    public static GlShaderProgram TEXTURELESS_SHADER_PROGRAM {
      get {
        if (CommonShaderPrograms.texturelessShaderProgram_ == null) {
          CommonShaderPrograms.texturelessShaderProgram_ =
              GlShaderProgram.FromShaders(@"
# version 120

varying vec4 vertexColor;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
}", @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor;

void main() {
    fragColor = vertexColor;
}");
        }

        return texturelessShaderProgram_;
      }
    }

    public static string VERTEX_SRC { get; } = @"
# version 330

uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;

layout(location = 0) in vec3 in_Position;
layout(location = 1) in vec3 in_Normal;
layout(location = 2) in vec4 in_Tangent;
layout(location = 3) in vec2 in_Uvs[4];
layout(location = 7) in vec4 in_Colors[2];

out vec3 vertexNormal;
out vec3 tangent;
out vec3 binormal;
out vec2 normalUv;
out vec2 uv0;
out vec2 uv1;
out vec2 uv2;
out vec2 uv3;
out vec4 vertexColor0;
out vec4 vertexColor1;

void main() {
    gl_Position = projectionMatrix * modelViewMatrix * vec4(in_Position, 1);
    vertexNormal = normalize(modelViewMatrix * vec4(in_Normal, 0)).xyz;
    tangent = normalize(modelViewMatrix * vec4(in_Tangent)).xyz;
    binormal = cross(vertexNormal, tangent); 
    normalUv = normalize(projectionMatrix * modelViewMatrix * vec4(in_Normal, 0)).xy;
    vertexColor0 = in_Colors[0];
    vertexColor1 = in_Colors[1];
    uv0 = in_Uvs[0].st;
    uv1 = in_Uvs[1].st;
    uv2 = in_Uvs[2].st;
    uv3 = in_Uvs[3].st;
}";
  }
}