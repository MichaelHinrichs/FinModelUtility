using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

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

      public ITexture CreateTexture(Bitmap imageData)
        => new TextureImpl(imageData);
    }

    private class TextureImpl : ITexture {
      public TextureImpl(Bitmap imageData) {
        this.ImageData = imageData;
      }

      public ColorSourceType Type => ColorSourceType.TEXTURE;

      public string Name { get; set; }
      public int UvIndex { get; }
      public UvType UvType { get; }
      public Bitmap ImageData { get; }
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

        public BlendMode BlendMode { get; set; }
        public BlendFactor SrcFactor { get; set; }
        public BlendFactor DstFactor { get; set; }
        public LogicOp LogicOp { get; set; }
      }
    }
  }
}