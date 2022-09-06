using System;

using fin.util.asserts;


namespace fin.data {
  public interface IReadOnlyGrid<out T> {
    int Width { get; }
    int Height { get; }

    T this[int x, int y] { get; }
  }

  public interface IGrid<T> : IReadOnlyGrid<T> {
    new T this[int x, int y] { get; set; }
  }

  public class Grid<T> : IGrid<T> {
    private T[] impl_;

    public Grid(int width, int height, T defaultValue = default) {
      this.Width = width;
      this.Height = height;
      
      this.impl_ = new T[width * height];
      for (var i = 0; i < this.impl_.Length; ++i) {
        this.impl_[i] = defaultValue;
      }
    }

    public Grid(int width, int height, Func<T> defaultValueHandler) {
      this.Width = width;
      this.Height = height;

      this.impl_ = new T[width * height];
      for (var i = 0; i < this.impl_.Length; ++i) {
        this.impl_[i] = defaultValueHandler();
      }
    }

    public int Width { get; }
    public int Height { get; }

    public T this[int x, int y] {
      get => this.impl_[this.GetIndex_(x, y)];
      set => this.impl_[this.GetIndex_(x, y)] = value;
    }

    private int GetIndex_(int x, int y) {
      if (!(x >= 0 && x < this.Width && y >= 0 && y < this.Height)) {
        Asserts.Fail(
            $"Expected ({x}, {y}) to be a valid index in grid of size ({this.Width}, {this.Height}).");
      }
      return y * this.Width + x;
    }
  }
}