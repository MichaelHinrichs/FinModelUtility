using System.Collections.Generic;

using fin.color;
using fin.math.xyz;

namespace fin.model {
  // Read only
  public interface IReadOnlyLighting {
    IReadOnlyList<IReadOnlyLight> Lights { get; }

    IColor AmbientLightColor { get; }
    float AmbientLightStrength { get; }
  }

  public enum LightSourceType {
    UNDEFINED,
    POSITION,
    RAY,
    LINE,
  }

  public enum AttenuationFunction {
    NONE,
    SPECULAR,
    SPOT,
  }

  public enum DiffuseFunction {
    NONE,
    SIGNED,
    CLAMP,
  }

  public interface IReadOnlyLight {
    string Name { get; }
    bool Enabled { get; }

    LightSourceType SourceType { get; }

    IReadOnlyXyz? Position { get; }
    IReadOnlyXyz? Normal { get; }

    float Strength { get; }
    IColor Color { get; }

    IReadOnlyXyz? CosineAttenuation { get; }
    IReadOnlyXyz? DistanceAttenuation { get; }

    AttenuationFunction AttenuationFunction { get; }
    DiffuseFunction DiffuseFunction { get; }
  }

  // Mutable
  public interface ILighting : IReadOnlyLighting {
    IReadOnlyList<IReadOnlyLight> IReadOnlyLighting.Lights => this.Lights;
    new IReadOnlyList<ILight> Lights { get; }

    ILight CreateLight();

    IColor IReadOnlyLighting.AmbientLightColor => this.AmbientLightColor;
    new IColor AmbientLightColor { get; set; }
    float IReadOnlyLighting.AmbientLightStrength => this.AmbientLightStrength;
    new float AmbientLightStrength { get; set; }
  }

  public interface ILight : IReadOnlyLight {
    ILight SetName(string name);

    bool IReadOnlyLight.Enabled => this.Enabled;
    new bool Enabled { get; set; }

    ILight SetPosition(IReadOnlyXyz position);
    ILight SetNormal(IReadOnlyXyz normal);

    float IReadOnlyLight.Strength => this.Strength;
    new float Strength { get; set; }
    ILight SetColor(IColor color);

    ILight SetCosineAttenuation(IReadOnlyXyz cosineAttenuation);
    ILight SetDistanceAttenuation(IReadOnlyXyz distanceAttenuation);

    ILight SetAttenuationFunction(AttenuationFunction attenuationFunction);
    ILight SetDiffuseFunction(DiffuseFunction diffuseFunction);
  }
}