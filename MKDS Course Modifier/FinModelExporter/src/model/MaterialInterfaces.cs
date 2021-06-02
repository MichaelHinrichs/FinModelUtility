using System.Collections.Generic;

namespace fin.model {
  public interface IMaterialManager {
    IReadOnlyList<IMaterial> All { get; }
    IMaterial AddMaterial();
  }


  public interface IMaterial {
    string Name { get; set; }

    IReadOnlyList<ITexture> Textures { get; }
    
    IShader Shader { get; }
  }


  public interface IShader {
    IReadOnlyList<IShaderParam> Params { get; }
  }

  public interface IShaderParam {
    string Name { get; }
  }

  public interface IShaderParam<out T> : IShaderParam {
    T Default { get; }
  }


  // TODO: Support empty white materials
  // TODO: Support basic diffuse materials
  // TODO: Support lit/unlit
  // TODO: Support merged diffuse/normal/etc. materials

  public interface ILayerMaterial : IMaterial {
    IReadOnlyList<ILayer> Layers { get; }

    ILayer AddColorLayer(byte r, byte g, byte b);

    // TODO: Force a default here?
    ILayer AddColorShaderParamLayer(string name);

    // TODO: Where to source data from?
    ILayer AddTextureLayer();

    // TODO: Generate shader based on layers.
  }

  public enum BlendMode {
    ADD,
    MULTIPLY,
  }

  public interface ILayer {
    IColorSource ColorSource { get; }
    BlendMode BlendMode { get; }
  }

  public enum ColorSourceType {
    COLOR,
    SHADER_PARAM,
    TEXTURE,
  }

  public interface IColorSource {
    ColorSourceType Type { get; }
  }

  public interface IColor : IColorSource {
    // TODO: Specify as RGB

    float Rf { get; }
    float Gf { get; }
    float Bf { get; }

    byte Rb { get; }
    byte Gb { get; }
    byte Bb { get; }
  }

  public interface IColorShaderParam : IColorSource, IShaderParam<IColor> {
  }

  public enum UvType {
    NORMAL,
    SPHERICAL,
  }

  public interface ITexture : IColorSource {
    string Name { get; set; }

    int UvIndex { get; }
    UvType UvType { get; }

    IImage Image { get; }

    // TODO: UV Scaling
    // TODO: Repeating types (clamp/repeat/back-and-forth)
    // TODO: Support fixed # of repeats
    // TODO: Support animated textures
    // TODO: Support animated texture index param
  }

  public interface IImage {
    // TODO: How to specify image data?
  }
}
