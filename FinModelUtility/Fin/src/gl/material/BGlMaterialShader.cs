using System;
using System.Collections.Generic;

using fin.model;

using OpenTK.Graphics.OpenGL;

namespace fin.gl.material {
  public abstract class BGlMaterialShader<TMaterial> : IGlMaterialShader
      where TMaterial : IReadOnlyMaterial {
    private readonly IModel model_;
    private readonly GlShaderProgram impl_;

    private int modelViewMatrixLocation_;
    private int projectionMatrixLocation_;
    private int useLightingLocation_;

    protected BGlMaterialShader(IModel model,
                                TMaterial material) {
      this.model_ = model;
      this.Material = material;
      this.impl_ = this.GenerateShaderProgram(material);

      this.modelViewMatrixLocation_ =
          this.impl_.GetUniformLocation("modelViewMatrix");
      this.projectionMatrixLocation_ =
          this.impl_.GetUniformLocation("projectionMatrix");
      this.useLightingLocation_ = this.impl_.GetUniformLocation("useLighting");
    }

    ~BGlMaterialShader() => this.Dispose();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      this.DisposeInternal();
    }

    protected abstract void DisposeInternal();

    protected abstract GlShaderProgram
        GenerateShaderProgram(TMaterial material);

    protected abstract void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram);

    public IReadOnlyMaterial Material { get; }

    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      var modelViewMatrix = GlTransform.ModelViewMatrix;
      GlTransform.UniformMatrix4(this.modelViewMatrixLocation_,
                                 modelViewMatrix);

      var projectionMatrix = GlTransform.ProjectionMatrix;
      GlTransform.UniformMatrix4(this.projectionMatrixLocation_,
                                 projectionMatrix);

      GL.Uniform1(this.useLightingLocation_, this.UseLighting ? 1f : 0f);

      this.SetUpLightUniforms_(this.impl_,
                               this.model_.Lighting.Lights,
                               "lights",
                               MaterialConstants.MAX_LIGHTS);

      this.PassUniformsAndBindTextures(this.impl_);
    }

    private void SetUpLightUniforms_(GlShaderProgram impl,
                                     IReadOnlyList<ILight> lights,
                                     string name,
                                     int max) {
      for (var i = 0; i < max; ++i) {
        var isEnabled = i < lights.Count && lights[i].Enabled;
        var enabledLocation = impl.GetUniformLocation($"{name}[{i}].enabled");
        GL.Uniform1(enabledLocation, isEnabled ? 1 : 0);

        if (!isEnabled) {
          continue;
        }

        var light = lights[i];

        var position = light.Position;
        var positionLocation = impl.GetUniformLocation($"{name}[{i}].position");
        GL.Uniform3(positionLocation, position.X, position.Y, position.Z);

        var normal = light.Normal;
        var normalLocation = impl.GetUniformLocation($"{name}[{i}].normal");
        GL.Uniform3(normalLocation, normal.X, normal.Y, normal.Z);

        var color = light.Color;
        var colorLocation = impl.GetUniformLocation($"{name}[{i}].color");
        GL.Uniform4(colorLocation, color.Rf, color.Gf, color.Bf, color.Af);
      }
    }
  }
}