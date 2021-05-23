using System;
using System.Numerics;

using fin.math;

namespace fin.model.impl {
  public partial class ModelImpl {
    public class PositionImpl : IPosition {
      public float X { get; set; }
      public float Y { get; set; }
      public float Z { get; set; }
      public float W => 1;
    }

    public class NormalImpl : INormal {
      public float X { get; set; }
      public float Y { get; set; }
      public float Z { get; set; }
      public float W => 0;
    }

    public class ScaleImpl : IScale {
      public float X { get; set; }
      public float Y { get; set; }
      public float Z { get; set; }
    }

    public class QuaternionImpl : IQuaternion {
      private const float DEG_2_RAD = (float) (Math.PI / 180);
      private const float RAD_2_DEG = 1 / DEG_2_RAD;

      private Quaternion impl_ = Quaternion.Identity;

      /// <summary>
      ///   Length of the (x, y, z, w) quaternion vector.
      /// </summary>
      public float Length => this.impl_.Length();

      public float XDegrees => this.XRadians * QuaternionImpl.RAD_2_DEG;
      public float YDegrees => this.YRadians * QuaternionImpl.RAD_2_DEG;
      public float ZDegrees => this.ZRadians * QuaternionImpl.RAD_2_DEG;

      public IQuaternion SetDegrees(float x, float y, float z)
        => this.SetRadiansImpl_(x * QuaternionImpl.DEG_2_RAD,
                               y * QuaternionImpl.DEG_2_RAD,
                               z * QuaternionImpl.DEG_2_RAD);

      public float XRadians { get; private set; }
      public float YRadians { get; private set; }
      public float ZRadians { get; private set; }

      public IQuaternion SetRadians(float x, float y, float z)
        => this.SetRadiansImpl_(x, y, z);

      private IQuaternion SetRadiansImpl_(float x, float y, float z) {
        this.impl_ = QuaternionUtil.Create(x, y, z);
        this.XRadians = x;
        this.YRadians = y;
        this.ZRadians = z;
        return this;
      }
    }
  }
}