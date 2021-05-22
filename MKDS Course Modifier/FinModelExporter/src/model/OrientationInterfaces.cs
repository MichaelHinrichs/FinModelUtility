using System.Collections.Generic;

namespace fin.model {
  public interface IVector4 {
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
    float W { get; }
  }

  public interface IPosition : IVector4 {}

  public interface INormal : IVector4 {}

  public interface IVector3 {
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
  }

  public interface IScale : IVector3 {}

  public interface IQuaternion {
    float Length { get; }

    float XDegrees { get; }
    float YDegrees { get; }
    float ZDegrees { get; }
    IQuaternion SetDegrees(float x, float y, float z);

    float XRadians { get; }
    float YRadians { get; }
    float ZRadians { get; }
    IQuaternion SetRadians(float x, float y, float z);
  }
}