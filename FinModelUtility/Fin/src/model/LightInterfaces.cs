using fin.color;


namespace fin.model {
  public interface ILight {
    string Name { get; }
    INormal Normal { get; }
    IColor Color { get; }
  }
}
