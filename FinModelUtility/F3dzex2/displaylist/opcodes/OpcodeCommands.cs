using System.Collections.Generic;


namespace f3dzex2.displaylist.opcodes {
  public class NoopOpcodeCommand : IOpcodeCommand { }

  public class DlOpcodeCommand : IOpcodeCommand {
    public IReadOnlyList<IDisplayList> PossibleBranches { get; set; }
    public bool PushCurrentDlToStack { get; set; }
  }

  /// <summary>
  ///   Stops executing current DL and returns to one at top of stack.
  /// </summary>
  public class EndDlOpcodeCommand : IOpcodeCommand { }



  public class MtxOpcodeCommand : IOpcodeCommand {
    public uint RamAddress { get; set; }

    public byte Params { get; set; }

    public bool Push => (Params & 1) != 0;
    public bool NoPush => !Push;

    public bool Load => (Params & 2) != 0;
    public bool Mul => !Load;

    public bool Projection => (Params & 4) != 0;
    public bool ModelView => !Projection;
  }


  public class VtxOpcodeCommand : IOpcodeCommand {
    public byte NumVertices { get; set; }
    public byte IndexToBeginStoringVertices { get; set; }
    public uint AddressOfVertices { get; set; }
  }


  public enum TriVertexOrder {
    ABC = 0,
    CAB = 1,
    BCA = 2
  }

  public class Tri1OpcodeCommand : IOpcodeCommand {
    public TriVertexOrder VertexOrder { get; set; }
    public byte VertexIndexA { get; set; }
    public byte VertexIndexB { get; set; }
    public byte VertexIndexC { get; set; }
  }

  public class Tri2OpcodeCommand : IOpcodeCommand {
    public TriVertexOrder VertexOrder0 { get; set; }
    public byte VertexIndexA0 { get; set; }
    public byte VertexIndexB0 { get; set; }
    public byte VertexIndexC0 { get; set; }

    public TriVertexOrder VertexOrder1 { get; set; }
    public byte VertexIndexA1 { get; set; }
    public byte VertexIndexB1 { get; set; }
    public byte VertexIndexC1 { get; set; }

  }
}