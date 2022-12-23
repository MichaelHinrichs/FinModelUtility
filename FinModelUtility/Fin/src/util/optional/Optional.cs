using System;

using fin.util.asserts;

namespace fin.util.optional {
  public interface IOptional<T> {
    bool HasValue { get; }

    bool Try(out T value);

    IOptional<TSub> Pluck<TSub>(Func<T, TSub> pluck);
    IOptional<TSub> Pluck<TSub>(Func<T, IOptional<TSub>> pluck);

    IOptional<T> Or(IOptional<T> other);
    T Or(T defaultValue);
    T Assert(string? message = null);
  }

  public static class Optional {
    public static Optional<T> Of<T>(T value) => Optional<T>.Of(value);
    public static Optional<T> None<T>() => Optional<T>.None();
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


    public IOptional<TSub> Pluck<TSub>(Func<T, TSub> pluck)
      => this.HasValue
             ? Optional.Of(pluck(this.value_))
             : Optional.None<TSub>();

    public IOptional<TSub> Pluck<TSub>(Func<T, IOptional<TSub>> pluck)
      => this.HasValue
             ? pluck(this.value_)
             : Optional.None<TSub>();


    public IOptional<T> Or(IOptional<T> other) => this.HasValue ? this : other;
    public T Or(T defaultValue) => this.HasValue ? this.value_ : defaultValue;

    public T Assert(string? message = null) {
      Asserts.True(this.HasValue, message);
      return this.value_;
    }
  }
}