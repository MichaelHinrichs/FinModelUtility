using fin.color;


namespace fin.model {
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

  public interface ILight {
    string Name { get; }
    
    IPosition Position { get; }
    INormal Normal { get; }

    IColor Color { get; }

    IVector3 CosineAttenuation { get; }
    IVector3 DistanceAttenuation { get; }

    AttenuationFunction AttenuationFunction { get; }
    DiffuseFunction DiffuseFunction { get; }
  }
}
