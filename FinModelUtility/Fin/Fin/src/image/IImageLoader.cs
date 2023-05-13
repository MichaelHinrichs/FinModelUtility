namespace fin.image {
  public interface IImageLoader<TImageFileBundle>
      where TImageFileBundle : IImageFileBundle {
    IImage LoadImage(TImageFileBundle imageFileBundle);
  }
}