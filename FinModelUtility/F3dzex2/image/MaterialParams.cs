using System.Drawing;

using f3dzex2.displaylist.opcodes;
using fin.model;
using fin.util.hash;

namespace f3dzex2.image {
  public struct MaterialParams {
    public MaterialParams() { }

    public ImageParams ImageParams => TextureParams.ImageParams;
    public TextureParams TextureParams { get; private set; } = new();

    public N64ColorFormat ColorFormat {
      get => this.TextureParams.ColorFormat;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.ColorFormat = value;
        this.TextureParams = textureParams;
      }
    }

    public Color Color {
      get => this.TextureParams.Color;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.Color = value;
        this.TextureParams = textureParams;
      }
    }

    public BitsPerTexel BitsPerTexel {
      get => this.TextureParams.BitsPerTexel;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.BitsPerTexel = value;
        this.TextureParams = textureParams;
      }
    }

    public ushort Width {
      get => this.TextureParams.Width;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.Width = value;
        this.TextureParams = textureParams;
      }
    }

    public ushort Height {
      get => this.TextureParams.Height;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.Height = value;
        this.TextureParams = textureParams;
      }
    }

    public uint SegmentedAddress {
      get => this.TextureParams.SegmentedAddress;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.SegmentedAddress = value;
        this.TextureParams = textureParams;
      }
    }

    public F3dWrapMode WrapModeT {
      get => this.TextureParams.WrapModeT;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.WrapModeT = value;
        this.TextureParams = textureParams;
      }
    }

    public F3dWrapMode WrapModeS {
      get => this.TextureParams.WrapModeS;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.WrapModeS = value;
        this.TextureParams = textureParams;
      }
    }

    public UvType UvType {
      get => this.TextureParams.UvType;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.UvType = value;
        this.TextureParams = textureParams;
      }
    }

    public float TexScaleX {
      get => this.TextureParams.TexScaleX;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.TexScaleX = value;
        this.TextureParams = textureParams;
      }
    }

    public float TexScaleY {
      get => this.TextureParams.TexScaleY;
      set {
        TextureParams textureParams = this.TextureParams;
        textureParams.TexScaleY = value;
        this.TextureParams = textureParams;
      }
    }

    public CullingMode CullingMode { get; set; }

    public override int GetHashCode() => FluentHash.Start()
                                                   .With(this.TextureParams)
                                                   .With(CullingMode);

    public override bool Equals(object? other) {
      if (ReferenceEquals(this, other)) {
        return true;
      }

      if (other is MaterialParams otherMaterialParams) {
        return TextureParams.Equals(otherMaterialParams.TextureParams) &&
               CullingMode == otherMaterialParams.CullingMode;
      }

      return false;
    }
  }
}
