using System.Drawing;


namespace gx {
  public interface IPopulatedMaterial {
    string Name { get; }
    GxCullMode CullMode { get; }

    Color[] MaterialColors { get; }
    IColorChannelControl?[] ColorChannelControls { get; }
    Color[] AmbientColors { get; }

    ITevOrder?[] TevOrderInfos { get; }
    ITevStageProps?[] TevStageInfos { get; }

    IAlphaCompare AlphaCompare { get; }
    IBlendFunction BlendMode { get; }

    ITexCoordGen?[] TexCoordGens { get; }
  }
}