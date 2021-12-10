using fin.util.optional;

namespace f3dzex.displaylist {
  public interface IDisplayList {
    public IDisplayListInstruction Root { get; set; }
  }

  public interface IDisplayListInstruction {
    long Address { get; }

    uint Low { get; }
    uint High { get; }

    F3dzexOpcode Opcode => (F3dzexOpcode)(this.Low >> 24);

    Optional<IDisplayListInstruction> FirstChild { get; set; }
    Optional<IDisplayListInstruction> NextSibling { get; set; }
  }
}