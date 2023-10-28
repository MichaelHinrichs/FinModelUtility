using System.Collections.Generic;

using fin.color;

namespace fin.model {
  // Read only
  public interface IReadOnlyLighting {
    IReadOnlyList<IReadOnlyLight> Lights { get; }

    IColor AmbientLightColor { get; }
    float AmbientLightStrength { get; }
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

    IReadOnlyVector3 Position { get; }
    IReadOnlyVector3 Normal { get; }

    float Strength { get; }
    IColor Color { get; }

    IReadOnlyVector3 CosineAttenuation { get; }
    IReadOnlyVector3 DistanceAttenuation { get; }

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

    ILight SetPosition(IReadOnlyVector3 position);
    ILight SetNormal(IReadOnlyVector3 normal);

    float IReadOnlyLight.Strength => this.Strength;
    new float Strength { get; set; }
    ILight SetColor(IColor color);

    ILight SetCosineAttenuation(IReadOnlyVector3 cosineAttenuation);
    ILight SetDistanceAttenuation(IReadOnlyVector3 distanceAttenuation);

    ILight SetAttenuationFunction(AttenuationFunction attenuationFunction);
    ILight SetDiffuseFunction(DiffuseFunction diffuseFunction);
  }
}