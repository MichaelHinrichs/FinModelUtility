using f3dzex2.model;

namespace f3dzex2.image {
  /// <summary>
  ///   https://www.retroreversing.com/n64rdp
  /// </summary>
  public interface IRdp {
    ITmem Tmem { get; }
    JankTmem? JankTmem => this.Tmem as JankTmem;

    IF3dVertices F3dVertices { get; }
  }

  public class Rdp : IRdp {
    public ITmem Tmem { get; set; }
    public IF3dVertices F3dVertices { get; set; }
  }
}