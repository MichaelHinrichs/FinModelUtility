using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;

using fin.color;
using fin.image;
using fin.io;
using fin.language.equations.fixedFunction;
using fin.util.image;


namespace fin.model.impl {
  public partial class ModelImpl {
    public IMaterialManager MaterialManager { get; } =
      new MaterialManagerImpl();

    private class MaterialManagerImpl : IMaterialManager {
      private IList<IMaterial> materials_ = new List<IMaterial>();
      private IList<ITexture> textures_ = new List<ITexture>();

      public MaterialManagerImpl() {
        this.All = new ReadOnlyCollection<IMaterial>(this.materials_);
        this.Textures = new ReadOnlyCollection<ITexture>(this.textures_);
      }

      public IReadOnlyList<IMaterial> All { get; }

      public INullMaterial AddNullMaterial() {
        var material = new NullMaterialImpl();
        this.materials_.Add(material);
        return material;
      }

      public ITextureMaterial AddTextureMaterial(ITexture texture) {
        var material = new TextureMaterialImpl(texture);
        this.materials_.Add(material);
        return material;
      }

      public IStandardMaterial AddStandardMaterial() {
        var material = new StandardMaterialImpl();
        this.materials_.Add(material);
        return material;
      }

      public IFixedFunctionMaterial AddFixedFunctionMaterial() {
        var material = new FixedFunctionMaterialImpl();
        this.materials_.Add(material);
        return material;
      }

      public ITexture CreateTexture(IImage imageData) {
        var texture = new TextureImpl(imageData);
        this.textures_.Add(texture);
        return texture;
      }
      public IReadOnlyList<ITexture> Textures { get; }
    }

    private class TextureImpl : ITexture {
      public TextureImpl(IImage image) {
        this.Image = image;
        this.ImageData = image.AsBitmap();
        this.TransparencyType = ImageUtil.GetTransparencyType(this.Image);
      }

      public string Name { get; set; }
      public int UvIndex { get; set; }
      public UvType UvType { get; set; }

      public ColorType ColorType { get; set; }

      public IImage Image { get; }
      public Bitmap ImageData { get; }

      public IFile SaveInDirectory(IDirectory directory) {
        var outFile =
            new FinFile(Path.Combine(directory.FullName, this.Name + ".png"));
        using var writer = outFile.OpenWrite();
        this.Image.ExportToStream(writer, LocalImageFormat.PNG);
        return outFile;
      }

      public ImageTransparencyType TransparencyType { get; }

      public WrapMode WrapModeU { get; set; }
      public WrapMode WrapModeV { get; set; }

      public IColor? BorderColor { get; set; }
    }

    private class NullMaterialImpl : INullMaterial {
      public string? Name { get; set; }
      public IEnumerable<ITexture> Textures { get; } = Array.Empty<ITexture>();
      public CullingMode CullingMode { get; set; }
    }

    private class TextureMaterialImpl : ITextureMaterial {
      public TextureMaterialImpl(ITexture texture) {
        this.Texture = texture;
        this.Textures = new ReadOnlyCollection<ITexture>(new[] {texture});
      }

      public string? Name { get; set; }

      public ITexture Texture { get; }
      public IEnumerable<ITexture> Textures { get; }

      public CullingMode CullingMode { get; set; }

      public bool Unlit { get; set; }
    }

    private class StandardMaterialImpl : IStandardMaterial {
      public string? Name { get; set; }

      public IEnumerable<ITexture> Textures {
        get {
          if (this.DiffuseTexture != null) {
            yield return this.DiffuseTexture;
          }

          if (this.MaskTexture != null) {
            yield return this.MaskTexture;
          }

          if (this.AmbientOcclusionTexture != null) {
            yield return this.AmbientOcclusionTexture;
          }

          if (this.NormalTexture != null) {
            yield return this.NormalTexture;
          }

          if (this.EmissiveTexture != null) {
            yield return this.EmissiveTexture;
          }

          if (this.SpecularTexture != null) {
            yield return this.SpecularTexture;
          }
        }
      }

      public CullingMode CullingMode { get; set; }
      public ITexture? DiffuseTexture { get; set; }
      public ITexture? MaskTexture { get; set; }
      public ITexture? AmbientOcclusionTexture { get; set; }
      public ITexture? NormalTexture { get; set; }
      public ITexture? EmissiveTexture { get; set; }
      public ITexture? SpecularTexture { get; set; }
      public bool Unlit { get; set; }
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

      public string? Name { get; set; }

      public IEnumerable<ITexture> Textures { get; }

      public CullingMode CullingMode { get; set; }

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

      public ITexture? CompiledTexture { get; set; }

      public IFixedFunctionMaterial SetBlending(
          BlendMode blendMode,
          BlendFactor srcFactor,
          BlendFactor dstFactor,
          LogicOp logicOp) {
        this.BlendMode = blendMode;
        this.SrcFactor = srcFactor;
        this.DstFactor = dstFactor;
        this.LogicOp = logicOp;
        return this;
      }

      public BlendMode BlendMode { get; private set; } = BlendMode.ADD;

      public BlendFactor SrcFactor { get; private set; } =
        BlendFactor.SRC_ALPHA;

      public BlendFactor DstFactor { get; private set; } =
        BlendFactor.ONE_MINUS_SRC_ALPHA;

      public LogicOp LogicOp { get; private set; } = LogicOp.COPY;

      public IFixedFunctionMaterial SetAlphaCompare(
          AlphaOp alphaOp,
          AlphaCompareType alphaCompareType0,
          float reference0,
          AlphaCompareType alphaCompareType1,
          float reference1) {
        this.AlphaOp = alphaOp;
        this.AlphaCompareType0 = alphaCompareType0;
        this.AlphaReference0 = reference0;
        this.AlphaCompareType1 = alphaCompareType1;
        this.AlphaReference1 = reference1;
        return this;
      }

      public AlphaOp AlphaOp { get; private set; }

      public AlphaCompareType AlphaCompareType0 { get; private set; } =
        AlphaCompareType.Always;

      public float AlphaReference0 { get; private set; }

      public AlphaCompareType AlphaCompareType1 { get; private set; } =
        AlphaCompareType.Always;

      public float AlphaReference1 { get; private set; }
    }
  }
}