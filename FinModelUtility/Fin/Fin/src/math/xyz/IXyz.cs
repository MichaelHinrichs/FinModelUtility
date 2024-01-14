namespace fin.math.xyz {
  public interface IReadOnlyXyz {
    float X { get; }
    float Y { get; }
    float Z { get; }
  }

  public interface IXyz : IReadOnlyXyz {
    float IReadOnlyXyz.X => this.X;
    new float X { get; set; }

    float IReadOnlyXyz.Y => this.Y;
    new float Y { get; set; }

    float IReadOnlyXyz.Z => this.Z;
    new float Z { get; set; }
  }
}
