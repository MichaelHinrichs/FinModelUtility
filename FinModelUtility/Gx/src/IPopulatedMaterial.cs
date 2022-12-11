using System.Drawing;


namespace gx {
  public enum GxCullMode {
    None = 0,  // Do not cull any primitives
    Front = 1, // Cull front-facing primitives
    Back = 2,  // Cull back-facing primitives
    All = 3    // Cull all primitives
  }

  public interface IPopulatedMaterial {
    string Name { get; }
    GxCullMode CullMode { get; }

    Color[] MaterialColors { get; }
    IColorChannelControl?[] ColorChannelControls { get; }
    Color[] AmbientColors { get; }

    Color[] KonstColors { get; }
    Color[] TevColors { get; }

    ITevOrder?[] TevOrderInfos { get; }
    ITevStageProps?[] TevStageInfos { get; }

    IAlphaCompare AlphaCompare { get; }
    IBlendFunction BlendMode { get; }

    ITexCoordGen?[] TexCoordGens { get; }

    short[] TextureIndices { get; }
  }
}