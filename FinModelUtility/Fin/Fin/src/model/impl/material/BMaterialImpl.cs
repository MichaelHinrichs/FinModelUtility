using System.Collections.Generic;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    private abstract class BMaterialImpl : IMaterial {
      public abstract IEnumerable<ITexture> Textures { get; }

      public string? Name { get; set; }
      public CullingMode CullingMode { get; set; }

      public DepthMode DepthMode { get; set; }
      public DepthCompareType DepthCompareType { get; set; }

      public bool IgnoreLights { get; set; }

      public float Shininess { get; set; } =
        MaterialConstants.DEFAULT_SHININESS;
    }
  }
}