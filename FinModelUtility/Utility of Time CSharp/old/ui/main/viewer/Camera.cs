using System;

namespace UoT.ui.main.viewer {
  public class Camera {
    // TODO: Add x/y/z locking.
    
    // TODO: Remove static instance.
    public static Camera? Instance { get; private set; }

    public Camera() {
      Camera.Instance = this;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }


    /// <summary>
    ///   The left-right angle of the camera, in degrees.
    /// </summary>
    public double Yaw { get; set; }

    /// <summary>
    ///   The up-down angle of the camera, in degrees.
    /// </summary>
    public double Pitch { get; set; }


    public double HorizontalNormal => Math.Cos(this.Pitch / 180 * Math.PI);
    public double VerticalNormal => Math.Sin(this.Pitch / 180 * Math.PI);


    public double XNormal
      => this.HorizontalNormal * Math.Cos(this.Yaw / 180 * Math.PI);

    public double YNormal => this.VerticalNormal;

    public double ZNormal
      => this.HorizontalNormal * Math.Sin(this.Yaw / 180 * Math.PI);


    public void Reset() => this.X = this.Y = this.Z = this.Yaw = this.Pitch = 0;

    // TODO: These negative signs and flipped cos/sin don't look right but they
    // work???
    public void Move(double forwardVector, double rightVector, double speed) {
      this.Y += -speed * this.VerticalNormal * forwardVector;

      var forwardYawRads = this.Yaw / 180 * Math.PI;
      var rightYawRads = (this.Yaw - 90) / 180 * Math.PI;

      this.X += speed *
                this.HorizontalNormal *
                (forwardVector * Math.Sin(forwardYawRads) +
                 -rightVector * Math.Sin(rightYawRads));

      this.Z += speed *
                -this.HorizontalNormal *
                (forwardVector * Math.Cos(forwardYawRads) +
                 -rightVector * Math.Cos(rightYawRads));
    }
  }
}