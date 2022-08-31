using fin.color;


namespace fin.model {
  public interface ILight {
    string Name { get; }
    
    IPosition Position { get; }
    INormal Normal { get; }

    IColor Color { get; }
  }
}
