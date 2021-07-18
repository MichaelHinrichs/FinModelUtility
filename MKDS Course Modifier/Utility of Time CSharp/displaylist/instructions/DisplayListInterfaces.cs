using System.Collections.Generic;

namespace UoT {
  public interface IDisplayListManager : IEnumerable<N64DisplayList> {
    int Count { get; }

    void Add(N64DisplayList displayList);
    N64DisplayList GetDisplayListByIndex(int index);
    int GetIndexByAddress(uint address);
  }

  public interface IDisplayList {
    IDisplayListInstruction[] Commands { get; set; }
  }

  public interface IDisplayListInstruction {
    byte Opcode { get; }
    string Name { get; }

    uint Low { get; }
    uint High { get; }
    byte[] CMDParams { get; }


    void Update(IList<byte> src, uint offset);
    void Update(byte opcode, uint low, uint high);
    void Update(uint low, uint high);
  }
}
