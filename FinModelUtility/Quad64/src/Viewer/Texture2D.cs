using fin.image;

namespace Quad64 {
  public class Texture2D {
    public IImage Bmp { get; set; }

    public int Id { get; }

    public int Width => Bmp.Width;
    public int Height => Bmp.Height;

    public int TextureParamS { get; set; }
    public int TextureParamT { get; set; }

    public Texture2D(IImage bmp) {
      this.Bmp = bmp;
    }

    public override int GetHashCode() {
      var hash = 17;
      hash = hash * 23 + this.Id.GetHashCode();
      hash = hash * 23 + this.Bmp.GetHashCode();
      hash = hash * 23 + this.Width.GetHashCode();
      hash = hash * 23 + this.Height.GetHashCode();
      hash = hash * 23 + this.TextureParamS.GetHashCode();
      hash = hash * 23 + this.TextureParamT.GetHashCode();
      return hash;
    }

    public override bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is Texture2D otherTexture) {
        return this.Equals(otherTexture);
      }

      return false;
    }

    public bool Equals(Texture2D other)
      => this.Id == other.Id &&
         this.Bmp == other.Bmp &&
         this.Width == other.Width &&
         this.Height == other.Height &&
         this.TextureParamS == other.TextureParamS &&
         this.TextureParamT == other.TextureParamT;
  }
}