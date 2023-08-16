using fin.data;

namespace fin.image {
  public interface IImageChannels<T> {
    int ChannelCount { get; }
    
    int Width { get; }
    int Height { get; }

    T this[int x, int y, int channel] { get; set; }
  }

  public class ImageChannels<T> : IImageChannels<T> {
    private readonly Grid<T>[] impl_;

    public ImageChannels(int width, int height, int channelCount) {
      this.impl_ = new Grid<T>[channelCount];
      for (var i = 0; i < channelCount; ++i) {
        this.impl_[i] = new Grid<T>(width, height);
      }
    }

    public int ChannelCount => this.impl_.Length;

    public int Width => this.impl_[0].Width;
    public int Height => this.impl_[0].Height;

    public T this[int x, int y, int channel] {
      get => this.impl_[channel][x, y];
      set => this.impl_[channel][x, y] = value;
    }
  }
}
