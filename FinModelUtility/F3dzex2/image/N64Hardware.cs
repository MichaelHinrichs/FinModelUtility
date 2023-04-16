using f3dzex2.io;

namespace f3dzex2.image {
  public interface IN64Hardware {
    IN64Memory Memory { get; }
    IRsp Rsp { get; }
    IRdp Rdp { get; }
  }

  public class N64Hardware : IN64Hardware {
    public IN64Memory Memory { get; set; }
    public IRsp Rsp { get; set; }
    public IRdp Rdp { get; set; }
  }
}
