using fin.color;
using System.Collections.Generic;


namespace fin.model.impl {
  public partial class ModelImpl {
    public ILighting Lighting { get; } = new LightingImpl();

    private class LightingImpl : ILighting {
      private readonly List<ILight> lights_ = new();

      public IReadOnlyList<ILight> Lights => lights_;
      public ILight CreateLight() {
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

      public Position Position { get; private set; }
      public ILight SetPosition(Position position) {
        this.Position = position;
        return this;
      }

      public Normal Normal { get; private set; }
      public ILight SetNormal(Normal normal) {
        this.Normal = normal;
        return this;
      }

      public IColor Color { get; private set; } = FinColor.FromRgbFloats(1, 1, 1);
      public ILight SetColor(IColor color) {
        this.Color = color;
        return this;
      }

      public IVector3 CosineAttenuation { get; } 
      public ILight SetCosineAttenuation(IVector3 cosineAttenuation) {
        throw new System.NotImplementedException();
      }

      public IVector3 DistanceAttenuation { get; }
      public ILight SetDistanceAttenuation(IVector3 distanceAttenuation) {
        throw new System.NotImplementedException();
      }

      public AttenuationFunction AttenuationFunction { get; }
      public ILight SetAttenuationFunction(AttenuationFunction attenuationFunction) {
        throw new System.NotImplementedException();
      }

      public DiffuseFunction DiffuseFunction { get; }
      public ILight SetDiffuseFunction(DiffuseFunction diffuseFunction) {
        throw new System.NotImplementedException();
      }
    }
  }
}