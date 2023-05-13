using System;

namespace fin.math.interpolation {
  public interface IInterpolator<T> : IInterpolator<T, T> { }

  public interface IInterpolator<in TIn, out TOut> {
    public TOut Interpolate(TIn fromValue, TIn toValue, float progress);
  }

  public class WrappedInterpolator<T>
      : WrappedInterpolator<T, T>, IInterpolator<T> {
    public WrappedInterpolator(InterpolateValues impl) : base(impl) { }
  }

  public class WrappedInterpolator<TIn, TOut> : IInterpolator<TIn, TOut> {
    public delegate TOut InterpolateValues(
        TIn fromValue,
        TIn toValue,
        float progress);

    private readonly InterpolateValues impl_;

    public WrappedInterpolator(InterpolateValues impl) {
      this.impl_ = impl;
    }

    public TOut Interpolate(
        TIn fromValue,
        TIn toValue,
        float progress)
      => this.impl_(fromValue, toValue, progress);
  }

  public static class Interpolator {
    public static IInterpolator<float> Float { get; } =
      new WrappedInterpolator<float>(
          (fromValue, toValue, progress)
              => fromValue * (1 - progress) + toValue * progress
      );

    public static IInterpolator<bool> Bool { get; } = StairStep<bool>();

    public static IInterpolator<TEnum> Enum<TEnum>() where TEnum : Enum
      => StairStep<TEnum>();

    public static IInterpolator<T> StairStep<T>()
      => new WrappedInterpolator<T>((fromValue, _, _) => fromValue);
  }
}