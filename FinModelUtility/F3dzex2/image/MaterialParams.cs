using fin.model;
using fin.util.hash;

namespace f3dzex2.image {
  public struct MaterialParams {
    public MaterialParams() { }

    public TextureParams TextureParams0 { get; set; } = new();
    public TextureParams TextureParams1 { get; set; } = new();

    public CullingMode CullingMode { get; set; }

    public override int GetHashCode() => FluentHash.Start()
                                                   .With(this.TextureParams0)
                                                   .With(this.TextureParams1)
                                                   .With(CullingMode);

    public override bool Equals(object? other) {
      if (ReferenceEquals(this, other)) {
        return true;
      }

      if (other is MaterialParams otherMaterialParams) {
        return TextureParams0.Equals(otherMaterialParams.TextureParams0) &&
               TextureParams1.Equals(otherMaterialParams.TextureParams1) &&
               CullingMode == otherMaterialParams.CullingMode;
      }

      return false;
    }
  }
}
