using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using fin.language.equations.fixedFunction;
using fin.util.image;

namespace fin.model.impl {
  public partial class ModelImpl {
    public IMaterialManager MaterialManager { get; } =
      new MaterialManagerImpl();

    private class MaterialManagerImpl : IMaterialManager {
      private IList<IMaterial> materials_ = new List<IMaterial>();

      public MaterialManagerImpl()
        => this.All = new ReadOnlyCollection<IMaterial>(this.materials_);

      public IReadOnlyList<IMaterial> All { get; }

      public ITextureMaterial AddTextureMaterial(ITexture texture) {
        var material = new TextureMaterialImpl(texture);
        this.materials_.Add(material);
        return material;
      }

      public ILayerMaterial AddLayerMaterial() {
        var material = new LayerMaterialImpl();
        this.materials_.Add(material);
        return material;
      }

      public IFixedFunctionMaterial AddFixedFunctionMaterial() {
        var material = new FixedFunctionMaterialImpl();
        this.materials_.Add(material);
        return material;
      }

      public ITexture CreateTexture(Bitmap imageData)
        => new TextureImpl(imageData);
    }

    private class TextureImpl : ITexture {
      public TextureImpl(Bitmap imageData) {
        this.ImageData = imageData;
        this.IsTransparent = BitmapUtil.IsTransparent(imageData);
      }

      public ColorSourceType Type => ColorSourceType.TEXTURE;

      public string Name { get; set; }
      public int UvIndex { get; set; }
      public UvType UvType { get; }

      public ColorType ColorType { get; }

      public Bitmap ImageData { get; }
      public bool IsTransparent { get; }

      public WrapMode WrapModeU { get; set; }
      public WrapMode WrapModeV { get; set; }
    }

    private class TextureMaterialImpl : ITextureMaterial {
      public TextureMaterialImpl(ITexture texture) {
        this.Texture = texture;
        this.Textures = new ReadOnlyCollection<ITexture>(new[] {texture});
      }

      public string Name { get; set; }

      public ITexture Texture { get; }
      public IReadOnlyList<ITexture> Textures { get; }

      public IShader Shader { get; }
      public bool Unlit { get; set; }
    }

    private class LayerMaterialImpl : ILayerMaterial {
      private readonly IList<ITexture> textures_ = new List<ITexture>();
      private readonly IList<ILayer> layers_ = new List<ILayer>();

      public LayerMaterialImpl() {
        this.Textures = new ReadOnlyCollection<ITexture>(this.textures_);
        this.Layers = new ReadOnlyCollection<ILayer>(this.layers_);
      }

      public string Name { get; set; }

      public IReadOnlyList<ITexture> Textures { get; }
      public IShader Shader { get; }

      public IReadOnlyList<ILayer> Layers { get; }

      public ILayer AddColorLayer(byte r, byte g, byte b) {
        throw new System.NotImplementedException();
      }

      public ILayer AddColorShaderParamLayer(string name) {
        throw new System.NotImplementedException();
      }

      public ILayer AddTextureLayer(ITexture texture) {
        this.textures_.Add(texture);

        var layer = new LayerImpl(texture);
        this.layers_.Add(layer);

        return layer;
      }

      private class LayerImpl : ILayer {
        public LayerImpl(IColorSource colorSource) {
          this.ColorSource = colorSource;
        }

        public IColorSource ColorSource { get; }

        public byte TexCoordIndex { get; set; }
        public BlendMode BlendMode { get; set; }
      }
    }

    private class FixedFunctionMaterialImpl : IFixedFunctionMaterial {
      private readonly List<ITexture> textures_ = new();

      private readonly ITexture?[] texturesSources_ = new ITexture[8];
      private readonly IColor?[] colors_ = new IColor[2];
      private readonly float?[] alphas_ = new float?[2];

      public FixedFunctionMaterialImpl() {
        this.Textures = new ReadOnlyCollection<ITexture>(this.textures_);

        this.TextureSources =
            new ReadOnlyCollection<ITexture?>(this.texturesSources_);
        this.ColorSources = new ReadOnlyCollection<IColor?>(this.colors_);
        this.AlphaSources = new ReadOnlyCollection<float?>(this.alphas_);
      }

      public string Name { get; set; }

      public IReadOnlyList<ITexture> Textures { get; }
      public IShader Shader { get; }

      public IFixedFunctionEquations<FixedFunctionSource> Equations { get; } =
        new FixedFunctionEquations<FixedFunctionSource>();


      public IReadOnlyList<ITexture?> TextureSources { get; }
      public IReadOnlyList<IColor?> ColorSources { get; }
      public IReadOnlyList<float?> AlphaSources { get; }

      public IFixedFunctionMaterial SetTextureSource(
          int textureIndex,
          ITexture texture) {
        if (!this.texturesSources_.Contains(texture)) {
          this.textures_.Add(texture);
        }

        this.texturesSources_[textureIndex] = texture;

        return this;
      }

      public IFixedFunctionMaterial SetColorSource(
          int colorIndex,
          IColor color) {
        this.colors_[colorIndex] = color;
        return this;
      }

      public IFixedFunctionMaterial SetAlphaSource(
          int alphaIndex,
          float alpha) {
        this.alphas_[alphaIndex] = alpha;
        return this;
      }
    }
  }

  public class ColorImpl : IColor {
    private static Random RANDOM_ = new();

    public ColorSourceType Type => ColorSourceType.COLOR;

    private ColorImpl(byte rb, byte gb, byte bb, byte ab) {
      this.Rb = rb;
      this.Gb = gb;
      this.Bb = bb;
      this.Ab = ab;
    }

    public static IColor FromRgbaBytes(byte rb, byte gb, byte bb, byte ab)
      => new ColorImpl(rb, gb, bb, ab);

    public static IColor FromHsv(
        double hDegrees,
        double sFraction,
        double vFraction) {
      var sharpColor = ColorImpl.ColorFromHSV(hDegrees, sFraction, vFraction);
      return ColorImpl.FromRgbaBytes(sharpColor.R,
                                     sharpColor.G,
                                     sharpColor.B,
                                     sharpColor.A);
    }

    public static IColor Random() {
      return ColorImpl.FromHsv(360 * ColorImpl.RANDOM_.NextDouble(),
                               1,
                               1);
    }

    public float Rf => this.Rb / 255f;
    public float Gf => this.Gb / 255f;
    public float Bf => this.Bb / 255f;
    public float Af => this.Ab / 255f;

    public byte Rb { get; }
    public byte Gb { get; }
    public byte Bb { get; }
    public byte Ab { get; }

    public static void ColorToHSV(
        Color color,
        out double hue,
        out double saturation,
        out double value) {
      int max = Math.Max(color.R, Math.Max(color.G, color.B));
      int min = Math.Min(color.R, Math.Min(color.G, color.B));

      hue = color.GetHue();
      saturation = (max == 0) ? 0 : 1d - (1d * min / max);
      value = max / 255d;
    }

    public static Color ColorFromHSV(
        double hue,
        double saturation,
        double value) {
      int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
      double f = hue / 60 - Math.Floor(hue / 60);

      value = value * 255;
      int v = Convert.ToInt32(value);
      int p = Convert.ToInt32(value * (1 - saturation));
      int q = Convert.ToInt32(value * (1 - f * saturation));
      int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

      if (hi == 0)
        return Color.FromArgb(255, v, t, p);
      else if (hi == 1)
        return Color.FromArgb(255, q, v, p);
      else if (hi == 2)
        return Color.FromArgb(255, p, v, t);
      else if (hi == 3)
        return Color.FromArgb(255, p, q, v);
      else if (hi == 4)
        return Color.FromArgb(255, t, p, v);
      else
        return Color.FromArgb(255, v, p, q);
    }
  }
}