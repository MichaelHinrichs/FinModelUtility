using fin.image;


namespace SuperMario64 {
  class ContentPipe {
    public static Texture2D LoadTexture(ref IImage bitmap) {
      return new Texture2D(bitmap);
    }
  }
}