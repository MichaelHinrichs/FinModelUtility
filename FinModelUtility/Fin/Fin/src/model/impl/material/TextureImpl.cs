using System.Collections.Generic;
using System.Drawing;
using System.IO;

using fin.color;
using fin.image;
using fin.io;
using fin.schema.vector;
using fin.util.image;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    private partial class MaterialManagerImpl {
      public IReadOnlyList<ITexture> Textures { get; }

      public ITexture CreateTexture(IImage imageData) {
        var texture = new TextureImpl(imageData);
        this.textures_.Add(texture);
        return texture;
      }
    }

    private class TextureImpl : ITexture {
      private ImageTransparencyType? transparencyType_;
      private Bitmap? imageData_;

      public TextureImpl(IImage image) {
        this.Image = image;
      }

      public string Name { get; set; }


      public LocalImageFormat BestImageFormat => LocalImageFormat.PNG;

      public string ValidFileName
        => this.Name.ReplaceInvalidFilenameCharacters() +
           BestImageFormat.GetExtension();

      public int UvIndex { get; set; }
      public UvType UvType { get; set; }

      public ColorType ColorType { get; set; }

      public IImage Image { get; }
      public Bitmap ImageData => this.imageData_ ??= Image.AsBitmap();

      public ISystemFile SaveInDirectory(ISystemDirectory directory) {
        ISystemFile outFile =
            new FinFile(Path.Combine(directory.FullPath, this.ValidFileName));
        using var writer = outFile.OpenWrite();
        this.Image.ExportToStream(writer, BestImageFormat);
        return outFile;
      }

      public ImageTransparencyType TransparencyType
        => this.transparencyType_ ??= ImageUtil.GetTransparencyType(this.Image);

      public WrapMode WrapModeU { get; set; }
      public WrapMode WrapModeV { get; set; }

      public IColor? BorderColor { get; set; }

      public TextureMagFilter MagFilter { get; set; } = TextureMagFilter.LINEAR;

      public TextureMinFilter MinFilter { get; set; } =
        TextureMinFilter.LINEAR_MIPMAP_LINEAR;


      public IReadOnlyVector2 Offset { get; private set; } = new Vector2f();

      public ITexture SetOffset(float x, float y) {
        this.Offset = new Vector2f { X = x, Y = y };
        return this;
      }


      public IReadOnlyVector2 Scale { get; private set; } =
        new Vector2f { X = 1, Y = 1 };

      public ITexture SetScale(float x, float y) {
        this.Scale = new Vector2f { X = x, Y = y };
        return this;
      }


      public float RotationDegrees { get; private set; }

      public ITexture SetRotationDegrees(float rotationDegrees) {
        this.RotationDegrees = rotationDegrees;
        return this;
      }


      public override int GetHashCode() {
        int hash = 216613626;
        var sub = 16780669;
        hash = hash * sub ^ Image.GetHashCode();
        hash = hash * sub ^ WrapModeU.GetHashCode();
        hash = hash * sub ^ WrapModeU.GetHashCode();
        return hash;
      }

      public override bool Equals(object? other) {
        if (ReferenceEquals(null, other)) {
          return false;
        }

        if (ReferenceEquals(this, other)) {
          return true;
        }

        if (other is ITexture otherTexture) {
          return this.Image == otherTexture.Image &&
                 this.WrapModeU == otherTexture.WrapModeU &&
                 this.WrapModeV == otherTexture.WrapModeV;
        }

        return false;
      }
    }
  }
}