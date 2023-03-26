using fin.color;

using System.Collections.Generic;
using System.Numerics;


namespace fin.model.impl {
  public partial class ModelImpl {
    public ILighting Lighting { get; } = new LightingImpl();

    private class LightingImpl : ILighting {
      private readonly List<ILight> lights_ = new();

      public IReadOnlyList<ILight> GlobalLights => lights_;

      public ILight CreateGlobalLight() {
        var light = new LightImpl();
        this.lights_.Add(light);
        return light;
      }
    }

    private class LightImpl : ILight {
      public string Name { get; private set; }

      public ILight SetName(string name) {
        this.Name = name;
        return this;
      }

      public IVector3 Position { get; private set; }

      public ILight SetPosition(IVector3 position) {
        this.Position = position;
        return this;
      }

      public IVector3 Normal { get; private set; }

      public ILight SetNormal(IVector3 normal) {
        this.Normal = normal;
        return this;
      }

      public IColor Color { get; private set; } =
        FinColor.FromRgbFloats(1, 1, 1);

      public ILight SetColor(IColor color) {
        this.Color = color;
        return this;
      }

      public IVector3 CosineAttenuation { get; private set; }

      public ILight SetCosineAttenuation(IVector3 cosineAttenuation) {
        this.CosineAttenuation = cosineAttenuation;
        return this;
      }

      public IVector3 DistanceAttenuation { get; private set; }

      public ILight SetDistanceAttenuation(IVector3 distanceAttenuation) {
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