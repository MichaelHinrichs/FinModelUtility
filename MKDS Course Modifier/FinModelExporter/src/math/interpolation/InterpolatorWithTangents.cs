namespace fin.math.interpolation {
  public interface IInterpolatorWithTangents<T>
      : IInterpolatorWithTangents<T, T> {}

  public interface IInterpolatorWithTangents<in TIn, out TOut> {
    public TOut Interpolate(
        TIn fromValue,
        float fromTangent,
        TIn toValue,
        float toTangent,
        float progress);
  }

  public class WrappedInterpolatorWithTangents<T>
      : WrappedInterpolatorWithTangents<T, T>, IInterpolatorWithTangents<T> {
    public WrappedInterpolatorWithTangents(
        InterpolateValuesWithTangents impl) :
        base(impl) {}
  }

  public class WrappedInterpolatorWithTangents<TIn, TOut>
      : IInterpolatorWithTangents<TIn, TOut> {
    public delegate TOut InterpolateValuesWithTangents(
        TIn fromValue,
        float fromTangent,
        TIn toValue,
        float toTangent,
        float progress);

    private readonly InterpolateValuesWithTangents impl_;

    public WrappedInterpolatorWithTangents(InterpolateValuesWithTangents impl) {
      this.impl_ = impl;
    }

    public TOut Interpolate(
        TIn fromValue,
        float fromTangent,
        TIn toValue,
        float toTangent,
        float progress)
      => this.impl_(fromValue, fromTangent, toValue, toTangent, progress);
  }

  public static class InterpolatorWithTangents {
    public static IInterpolatorWithTangents<float> Float { get; } =
        new WrappedInterpolatorWithTangents<float>(
            (fromValue, fromTangent, toValue, toTangent, progress) => {
              // TODO: Unfortunately, linear interpolation is way more accurate
              // right now.
              // What's going wrong here??

              return fromValue * (1 - progress) + toValue * progress;

              var v1 = fromValue;
              var v2 = toValue;
              var d1 = fromTangent;
              var d2 = toTangent;
              var t = progress;

              float num1 = (float) (2.0 * ((double) v1 - (double) v2)) +
                           d1 +
                           d2;
              float num2 =
                  (float) (-3.0 * (double) v1 +
                           3.0 * (double) v2 -
                           2.0 * (double) d1) -
                  d2;
              float num3 = d1;
              float num4 = v1;
              return ((num1 * t + num2) * t + num3) * t + num4;
            });
  }
}