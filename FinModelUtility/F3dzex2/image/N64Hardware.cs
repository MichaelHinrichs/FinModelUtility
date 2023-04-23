using f3dzex2.io;

namespace f3dzex2.image {
  public interface IN64Hardware {
    IReadOnlyN64Memory Memory { get; }
    IRsp Rsp { get; }
    IRdp Rdp { get; }
  }

  public interface IN64Hardware<TMemory> : IN64Hardware
      where TMemory : IReadOnlyN64Memory {
    new TMemory Memory { get; }
    IRsp Rsp { get; }
    IRdp Rdp { get; }
  }


  public class N64Hardware<TMemory> : IN64Hardware<TMemory>
      where TMemory : IReadOnlyN64Memory {
    IReadOnlyN64Memory IN64Hardware.Memory => this.Memory;
    public TMemory Memory { get; set; }
    public IRsp Rsp { get; set; }
    public IRdp Rdp { get; set; }
  }
}
