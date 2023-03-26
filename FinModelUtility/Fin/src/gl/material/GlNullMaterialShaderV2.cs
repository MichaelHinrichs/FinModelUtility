using fin.model;
using System;

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

    private readonly int modelViewMatrixLocation_;
    private readonly int projectionMatrixLocation_;


    public GlNullMaterialShaderV2() {
      this.modelViewMatrixLocation_ = impl_.GetUniformLocation("modelViewMatrix");
      this.projectionMatrixLocation_ = impl_.GetUniformLocation("projectionMatrix");
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
    }

    public IReadOnlyMaterial Material { get; }
    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      var modelViewMatrix = GlTransform.ModelViewMatrix;
      GlTransform.UniformMatrix4(this.modelViewMatrixLocation_, modelViewMatrix);

      var projectionMatrix = GlTransform.ProjectionMatrix;
      GlTransform.UniformMatrix4(this.projectionMatrixLocation_, projectionMatrix);
    }
  }
}
