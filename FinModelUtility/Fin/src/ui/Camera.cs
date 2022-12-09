using System;


namespace fin.ui {
  public class Camera {
    // TODO: Add x/y/z locking.

    public static Camera Instance { get; private set; }

    public Camera() {
      Camera.Instance = this;
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }


    /// <summary>
    ///   The left-right angle of the camera, in degrees.
    /// </summary>
    public float Yaw { get; set; }

    /// <summary>
    ///   The up-down angle of the camera, in degrees.
    /// </summary>
    public float Pitch { get; set; }


    public float HorizontalNormal => MathF.Cos(this.Pitch / 180 * MathF.PI);
    public float VerticalNormal => MathF.Sin(this.Pitch / 180 * MathF.PI);


    public float XNormal
      => this.HorizontalNormal * MathF.Cos(this.Yaw / 180 * MathF.PI);

    public float YNormal
      => this.HorizontalNormal * MathF.Sin(this.Yaw / 180 * MathF.PI);

    public float ZNormal => this.VerticalNormal;


    public void Reset() => this.X = this.Y = this.Z = this.Yaw = this.Pitch = 0;

    // TODO: These negative signs and flipped cos/sin don't look right but they
    // work???
    public void Move(float forwardVector, float rightVector, float speed) {
      this.Z += speed * this.VerticalNormal * forwardVector;

      var forwardYawRads = this.Yaw / 180 * MathF.PI;
      var rightYawRads = (this.Yaw - 90) / 180 * MathF.PI;

      this.X += speed *
                this.HorizontalNormal *
                (forwardVector * MathF.Cos(forwardYawRads) +
                 rightVector * MathF.Cos(rightYawRads));

      this.Y += speed *
                this.HorizontalNormal *
                (forwardVector * MathF.Sin(forwardYawRads) +
                 rightVector * MathF.Sin(rightYawRads));
    }
  }
}