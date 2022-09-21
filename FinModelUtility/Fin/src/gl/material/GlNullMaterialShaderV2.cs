using fin.model;
using OpenTK;
using System;

using OpenTK.Graphics.OpenGL;

namespace fin.gl.material {
  public class GlNullMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_ =
      GlShaderProgram.FromShaders(@"
# version 330

uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;

layout(location = 0) in vec3 in_Position;
layout(location = 1) in vec3 in_Normal;
layout(location = 2) in vec2 in_Uvs[4];
layout(location = 6) in vec4 in_Colors[2];

out vec4 vertexPosition;
out vec4 vertexColor;
out vec3 vertexNormal;
out vec2 normalUv;

void main() {
    vertexPosition = modelViewMatrix * vec4(in_Position, 1);
    gl_Position = projectionMatrix * modelViewMatrix * vec4(in_Position, 1);

    vertexNormal = normalize(modelViewMatrix * vec4(in_Normal, 0)).xyz;
    normalUv = normalize(projectionMatrix * modelViewMatrix * vec4(in_Normal, 0)).xy;
    vertexColor = in_Colors[0];
}", @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor;

void main() {
    fragColor = vertexColor;
}");

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
    }

    public IMaterial Material { get; }
    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 modelViewMatrix);
      GL.UniformMatrix4(this.impl_.GetUniformLocation("modelViewMatrix"),
        false, ref modelViewMatrix);

      GL.GetFloat(GetPName.ProjectionMatrix, out Matrix4 projectionMatrix);
      GL.UniformMatrix4(this.impl_.GetUniformLocation("projectionMatrix"),
        false, ref projectionMatrix);
    }
  }
}
