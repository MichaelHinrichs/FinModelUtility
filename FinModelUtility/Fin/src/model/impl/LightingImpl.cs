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

      public IPosition Position { get; } = new PositionImpl();
      public ILight SetPosition(IPosition position) {
        this.Position.X = position.X;
        this.Position.Y = position.Y;
        this.Position.Z = position.Z;
        return this;
      }

      public INormal Normal { get; } = new NormalImpl();
      public ILight SetNormal(INormal normal) {
        this.Normal.X = normal.X;
        this.Normal.Y = normal.Y;
        this.Normal.Z = normal.Z;
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