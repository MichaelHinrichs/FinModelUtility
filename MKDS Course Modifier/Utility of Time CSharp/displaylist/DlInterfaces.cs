namespace UoT.displaylist {
  public interface IVertex {
    short U { get; }
    short V { get; }

    float NormalX { get; }
    float NormalY { get; }
    float NormalZ { get; }

    byte R { get; }
    byte G { get; }
    byte B { get; }
    byte A { get; }
  }
}
