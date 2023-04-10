namespace f3dzex2.displaylist {
  public interface IDisplayList {
    public IDisplayListInstruction Root { get; set; }
  }

  public interface IDisplayListInstruction {
    long Address { get; }

    uint Low { get; }
    uint High { get; }

    F3dzexOpcode Opcode => (F3dzexOpcode)(this.Low >> 24);

    IDisplayListInstruction? FirstChild { get; set; }
    IDisplayListInstruction? NextSibling { get; set; }
  }
}