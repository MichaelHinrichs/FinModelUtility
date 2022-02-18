using System.Collections;
using System.Collections.Generic;

#nullable enable


namespace texture {
  public interface IDxt<TImage> : IEnumerable<IMipMap<TImage>>
      where TImage : notnull {
    ICubeMap<TImage>? CubeMap { get; }
    IMipMap<TImage>? MipMap { get; }
  }

  public interface ICubeMap<TImage> : IEnumerable<IMipMap<TImage>>
      where TImage : notnull {
    IMipMap<TImage>? PositiveX { get; }
    IMipMap<TImage>? NegativeX { get; }

    IMipMap<TImage>? PositiveY { get; }
    IMipMap<TImage>? NegativeY { get; }

    IMipMap<TImage>? PositiveZ { get; }
    IMipMap<TImage>? NegativeZ { get; }
  }

  public interface IMipMap<TImage> : IEnumerable<IMipMapLevel<TImage>>
      where TImage : notnull {
    IList<IMipMapLevel<TImage>> Levels { get; }
  }

  public interface IMipMapLevel<out TImage> where TImage : notnull {
    TImage Impl { get; }
    int Width { get; }
    int Height { get; }
  }


  public class DxtImpl<TImage> : IDxt<TImage> where TImage : notnull {
    public DxtImpl(ICubeMap<TImage> cubeMap) => this.CubeMap = cubeMap;
    public DxtImpl(IMipMap<TImage> mipmaps) => this.MipMap = mipmaps;

    public ICubeMap<TImage>? CubeMap { get; }
    public IMipMap<TImage>? MipMap { get; }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<IMipMap<TImage>> GetEnumerator()
      => this.CubeMap != null
             ? this.CubeMap.GetEnumerator()
             : this.GetMipMapEnumerator();

    public IEnumerator<IMipMap<TImage>> GetMipMapEnumerator() {
      if (this.MipMap != null) {
        yield return this.MipMap;
      }
    }
  }

  public class CubeMapImpl<TImage> : ICubeMap<TImage> where TImage : notnull {
    public IMipMap<TImage>? PositiveX { get; set; }
    public IMipMap<TImage>? NegativeX { get; set; }

    public IMipMap<TImage>? PositiveY { get; set; }
    public IMipMap<TImage>? NegativeY { get; set; }

    public IMipMap<TImage>? PositiveZ { get; set; }
    public IMipMap<TImage>? NegativeZ { get; set; }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<IMipMap<TImage>> GetEnumerator() {
      if (this.PositiveX != null) {
        yield return this.PositiveX;
      }
      if (this.NegativeX != null) {
        yield return this.NegativeX;
      }

      if (this.PositiveY != null) {
        yield return this.PositiveY;
      }
      if (this.NegativeY != null) {
        yield return this.NegativeY;
      }

      if (this.PositiveZ != null) {
        yield return this.PositiveZ;
      }
      if (this.NegativeZ != null) {
        yield return this.NegativeZ;
      }
    }
  }

  public class MipMapImpl<TImage> : IMipMap<TImage> where TImage : notnull {
    public MipMapImpl() {
      this.Levels = new List<IMipMapLevel<TImage>>();
    }

    public MipMapImpl(IList<IMipMapLevel<TImage>> levels) {
      this.Levels = levels;
    }

    public IList<IMipMapLevel<TImage>> Levels { get; }

    public void AddLevel(IMipMapLevel<TImage> level) {
      this.Levels.Add(level);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<IMipMapLevel<TImage>> GetEnumerator()
      => this.Levels.GetEnumerator();
  }

  public class MipMapLevelImpl<TImage> : IMipMapLevel<TImage>
      where TImage : notnull {
    public MipMapLevelImpl(TImage impl, int width, int height) {
      this.Impl = impl;
      this.Width = width;
      this.Height = height;
    }

    public TImage Impl { get; }
    public int Width { get; }
    public int Height { get; }
  }


  public class A16b16g16r16fReader { }
}