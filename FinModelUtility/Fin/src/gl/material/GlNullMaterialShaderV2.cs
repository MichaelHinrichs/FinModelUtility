using fin.model;
using OpenTK;
using System;

using OpenTK.Graphics.OpenGL;

namespace fin.gl.material {
  public class GlNullMaterialShaderV2 : IGlMaterialShader {
    private readonly GlShaderProgram impl_ =
      GlShaderProgram.FromShaders(CommonShaderPrograms.VERTEX_SRC, @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor0;

void main() {
    fragColor = vertexColor0;
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
