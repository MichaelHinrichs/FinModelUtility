using System.Runtime.InteropServices.ComTypes;

using MathNet.Numerics.Interpolation;

namespace fin.math.interpolation {
  public interface IInterpolatorWithTangents<T>
      : IInterpolatorWithTangents<T, T> {}

  public interface IInterpolatorWithTangents<in TIn, out TOut> {
    public TOut Interpolate(
        float fromTime,
        TIn fromValue,
        float fromTangent,
        float toTime,
        TIn toValue,
        float toTangent,
        float time);
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
        float fromTime,
        TIn fromValue,
        float fromTangent,
        float toTime,
        TIn toValue,
        float toTangent,
        float time);

    private readonly InterpolateValuesWithTangents impl_;

    public WrappedInterpolatorWithTangents(InterpolateValuesWithTangents impl) {
      this.impl_ = impl;
    }

    public TOut Interpolate(
        float fromTime,
        TIn fromValue,
        float fromTangent,
        float toTime,
        TIn toValue,
        float toTangent,
        float time)
      => this.impl_(fromTime,
                    fromValue,
                    fromTangent,
                    toTime,
                    toValue,
                    toTangent,
                    time);
  }

  public static class InterpolatorWithTangents {
    public static IInterpolatorWithTangents<float> Float { get; } =
      new WrappedInterpolatorWithTangents<float>(
          (fromTime, fromValue, fromTangent, toTime, toValue, toTangent, time)
              => {
            return InterpolatorWithTangents.InterpolateFloatsWithMkds(
                fromTime,
                fromValue,
                fromTangent,
                toTime,
                toValue,
                toTangent,
                time);
          });

    public static IInterpolatorWithTangents<float> Radians { get; } =
      new WrappedInterpolatorWithTangents<float>(
          (fromTime, fromValue, fromTangent, toTime, toValue, toTangent, time)
              => {
            toValue = fromValue +
                      RadiansUtil.angleDifference(toValue, fromValue);

            return InterpolatorWithTangents.InterpolateFloatsWithMkds(
                fromTime,
                fromValue,
                fromTangent,
                toTime,
                toValue,
                toTangent,
                time);
          });

    public static float InterpolateFloatsWithMathNet(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var spline = CubicSpline
          .InterpolateHermiteSorted(new double[] {fromTime, toTime},
                                    new double[] {fromValue, toValue},
                                    new double[] {fromTangent, toTangent});

      return (float) spline.Interpolate(time);
    }

    public static float InterpolateFloatsWithMkds(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var progress = (time - fromTime) / (toTime - fromTime);

      var v1 = fromValue;
      var v2 = toValue;
      var d1 = fromTangent;
      var d2 = toTangent;
      var t = progress;

      var num1 = 2.0 * (v1 - v2) + d1 + d2;
      var num2 = -3.0 * v1 +
                   3.0 * v2 -
                   2.0 * d1 -
                   d2;
      var num3 = d1;
      var num4 = v1;
      return (float) (((num1 * t + num2) * t + num3) * t + num4);
    }

    public static float InterpolateFloatsWithRandom(float fromTime,
                                                    float fromValue,
                                                    float fromTangent,
                                                    float toTime,
                                                    float toValue,
                                                    float toTangent,
                                                    float time) {
      var progress = (time - fromTime) / (toTime - fromTime);

      var s = 1 / 30f;

      var p0 = fromValue;
      var p1 = toValue;
      var m0 = fromTangent * s;
      var m1 = toTangent * s;
      var t = progress;

      float tt = t * t;
      float ttt = t * t * t;

      return
          (2.0f * ttt - 3.0f * tt + 1.0f) * p0 +
          (ttt - 2.0f * tt + t) * m0 +
          (-2.0f * ttt + 3.0f * tt) * p1 +
          (ttt - tt) * m1;

    }

    public static float InterpolateFloatsWithNoclipHermite(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var length = toTime - fromTime;

      var p0 = fromValue;
      var p1 = toValue;
      var s0 = fromTangent * length;
      var s1 = toTangent * length;

      var t = (time - fromTime) / length;

      var cf0 = (p0 * 2) + (p1 * -2) + (s0 * 1) + (s1 * 1);
      var cf1 = (p0 * -3) + (p1 * 3) + (s0 * -2) + (s1 * -1);
      var cf2 = (p0 * 0) + (p1 * 0) + (s0 * 1) + (s1 * 0);
      var cf3 = (p0 * 1) + (p1 * 0) + (s0 * 0) + (s1 * 0);

      return (((cf0 * t + cf1) * t + cf2) * t + cf3);
    }

    public static float InterpolateFloatsWithNoclipBezier(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var length = toTime - fromTime;

      var p0 = fromValue;
      var p1 = toValue;
      var p2 = fromTangent;
      var p3 = toTangent;

      var t = (time - fromTime) / length;

      var cf0 = (p0 * -1) + (p1 * 3) + (p2 * -3) + (p3 * 1);
      var cf1 = (p0 * 3) + (p1 * -6) + (p2 * 3) + (p3 * 0);
      var cf2 = (p0 * -3) + (p1 * 3) + (p2 * 0) + (p3 * 0);
      var cf3 = (p0 * 1) + (p1 * 0) + (p2 * 0) + (p3 * 0);

      return (((cf0 * t + cf1) * t + cf2) * t + cf3);
    }
  }
}