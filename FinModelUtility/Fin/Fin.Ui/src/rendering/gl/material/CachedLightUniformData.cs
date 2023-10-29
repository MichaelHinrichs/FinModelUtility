using fin.model;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class CachedLightUniformData {
    public IReadOnlyLight Light { get; }

    public int EnabledLocation { get; }

    public int SourceTypeLocation { get; }
    public int PositionLocation { get; }
    public int NormalLocation { get; }

    public int ColorLocation { get; }

    public int DiffuseFunctionLocation { get; }
    public int AttenuationFunctionLocation { get; }
    public int CosineAttenuationLocation { get; }
    public int DistanceAttenuationLocation { get; }

    public CachedLightUniformData(
        int lightIndex,
        IReadOnlyLight light,
        GlShaderProgram shaderProgram) {
      this.Light = light;

      var lightAccessor = $"{MaterialConstants.LIGHTS_NAME}[{lightIndex}]";

      this.EnabledLocation =
          shaderProgram.GetUniformLocation($"{lightAccessor}.enabled");

      this.SourceTypeLocation =
          shaderProgram.GetUniformLocation($"{lightAccessor}.sourceType");
      this.PositionLocation =
          shaderProgram.GetUniformLocation($"{lightAccessor}.position");
      this.NormalLocation =
          shaderProgram.GetUniformLocation($"{lightAccessor}.normal");

      this.ColorLocation =
          shaderProgram.GetUniformLocation($"{lightAccessor}.color");

      this.DiffuseFunctionLocation =
          shaderProgram.GetUniformLocation($"{lightAccessor}.diffuseFunction");
      this.AttenuationFunctionLocation =
          shaderProgram.GetUniformLocation(
              $"{lightAccessor}.attenuationFunction");
      this.CosineAttenuationLocation =
          shaderProgram.GetUniformLocation(
              $"{lightAccessor}.cosineAttenuation");
      this.DistanceAttenuationLocation =
          shaderProgram.GetUniformLocation(
              $"{lightAccessor}.distanceAttenuation");
    }

    public void PassInUniforms() {
      var light = this.Light;
      if (!light.Enabled) {
        return;
      }

      GL.Uniform1(this.EnabledLocation, 1);

      GL.Uniform1(this.SourceTypeLocation, (int) light.SourceType);

      var position = light.Position;
      if (position != null) {
        GL.Uniform3(this.PositionLocation,
                    position.X,
                    position.Y,
                    position.Z);
      }

      var normal = light.Normal;
      if (normal != null) {
        GL.Uniform3(this.NormalLocation,
                    normal.X,
                    normal.Y,
                    normal.Z);
      }

      var strength = light.Strength;
      var color = light.Color;
      GL.Uniform4(this.ColorLocation,
                  color.Rf * strength,
                  color.Gf * strength,
                  color.Bf * strength,
                  color.Af * strength);

      GL.Uniform1(this.DiffuseFunctionLocation, (int) light.DiffuseFunction);
      GL.Uniform1(this.AttenuationFunctionLocation,
                  (int) light.AttenuationFunction);

      var cosineAttenuation = light.CosineAttenuation;
      if (cosineAttenuation != null) {
        GL.Uniform3(this.CosineAttenuationLocation,
                    cosineAttenuation.X,
                    cosineAttenuation.Y,
                    cosineAttenuation.Z);
      }

      var distanceAttenuation = light.DistanceAttenuation;
      if (distanceAttenuation != null) {
        GL.Uniform3(this.DistanceAttenuationLocation,
                    distanceAttenuation.X,
                    distanceAttenuation.Y,
                    distanceAttenuation.Z);
      }
    }
  }
}