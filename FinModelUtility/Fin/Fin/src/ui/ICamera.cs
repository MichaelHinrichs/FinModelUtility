namespace fin.ui {
  public interface ICamera {
    float X { get; } 
    float Y { get; }
    float Z { get; }

    float XNormal { get; }
    float YNormal { get; }
    float ZNormal { get; }

    float YawDegrees { get; }
  }
}
