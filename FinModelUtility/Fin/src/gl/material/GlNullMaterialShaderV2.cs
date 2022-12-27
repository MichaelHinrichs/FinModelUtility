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

      var modelViewMatrix = GlTransform.ModelViewMatrix;
      GlTransform.UniformMatrix4(this.impl_.GetUniformLocation("modelViewMatrix"),
                        modelViewMatrix);

      var projectionMatrix = GlTransform.ProjectionMatrix;
      GlTransform.UniformMatrix4(this.impl_.GetUniformLocation("projectionMatrix"), projectionMatrix);
    }
  }
}
