using System.Collections.Generic;

using fin.color;
using fin.schema.vector;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    public ILighting Lighting { get; }
  }

  public class LightingImpl : ILighting {
    private readonly List<ILight> lights_ = new();

    public IReadOnlyList<ILight> Lights => lights_;

    public ILight CreateLight() {
      var light = new LightImpl();
      this.lights_.Add(light);
      return light;
    }

    public IColor AmbientLightColor { get; set; } =
      FinColor.FromIntensityFloat(.3f);

    private class LightImpl : ILight {
      public string Name { get; private set; }

      public ILight SetName(string name) {
        this.Name = name;
        return this;
      }

      public bool Enabled { get; set; } = true;

      public IReadOnlyVector3 Position { get; private set; } = new Vector3f();

      public ILight SetPosition(IReadOnlyVector3 position) {
        this.Position = position;
        return this;
      }

      public IReadOnlyVector3 Normal { get; private set; } = new Vector3f();

      public ILight SetNormal(IReadOnlyVector3 normal) {
        this.Normal = normal;
        return this;
      }

      public IColor Color { get; private set; } =
        FinColor.FromRgbaFloats(1, 1, 1, 1);

      public ILight SetColor(IColor color) {
        this.Color = color;
        return this;
      }

      public IReadOnlyVector3 CosineAttenuation { get; private set; }

      public ILight SetCosineAttenuation(IReadOnlyVector3 cosineAttenuation) {
        this.CosineAttenuation = cosineAttenuation;
        return this;
      }

      public IReadOnlyVector3 DistanceAttenuation { get; private set; }

      public ILight SetDistanceAttenuation(IReadOnlyVector3 distanceAttenuation) {
        this.DistanceAttenuation = distanceAttenuation;
        return this;
      }

      public AttenuationFunction AttenuationFunction { get; private set; } =
        AttenuationFunction.SPECULAR;

      public ILight SetAttenuationFunction(
          AttenuationFunction attenuationFunction) {
        this.AttenuationFunction = attenuationFunction;
        return this;
      }

      public DiffuseFunction DiffuseFunction { get; private set; } =
        DiffuseFunction.CLAMP;

      public ILight SetDiffuseFunction(DiffuseFunction diffuseFunction) {
        this.DiffuseFunction = diffuseFunction;
        return this;
      }
    }
  }
}