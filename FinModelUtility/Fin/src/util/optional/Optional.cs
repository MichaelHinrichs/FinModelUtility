using asserts;

namespace fin.util.optional {
  public interface IOptional<T> {
    bool Try(out T value);

    IOptional<T> Or(IOptional<T> other);
    T Or(T defaultValue);
    T Assert(string? message = null);
  }

  public struct Optional<T> : IOptional<T> {
    private readonly T value_;

    public static Optional<T> Of(T value) => new(value);
    public static Optional<T> None() => new();

    private Optional(T value) {
      this.HasValue = true;
      this.value_ = value;
    }

    public Optional() {
      this.HasValue = false;
      this.value_ = default;
    }

    public bool HasValue { get; }

    public bool Try(out T value) {
      if (this.HasValue) {
        value = this.value_;
        return true;
      }

      value = default;
      return false;
    }


    public IOptional<T> Or(IOptional<T> other) => this.HasValue ? this : other;
    public T Or(T defaultValue) => this.HasValue ? this.value_ : defaultValue;

    public T Assert(string? message = null) {
      Asserts.True(this.HasValue, message);
      return this.value_;
    }
  }
}