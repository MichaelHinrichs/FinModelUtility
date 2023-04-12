using System.Collections.Generic;
using System.Numerics;


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

  public class PopMtxOpcodeCommand : IOpcodeCommand {
    public uint NumberOfMatrices { get; set; }
  }


  public interface IVtx {
    short X { get; }
    short Y { get; }
    short Z { get; }
    Vector3 GetPosition();

    short Flag { get; }
    short U { get; }
    short V { get; }
    Vector2 GetUv(float scaleX, float scaleY);

    byte NormalXOrR { get; }
    byte NormalYOrG { get; }
    byte NormalZOrB { get; }
    byte A { get; }
    Vector3 GetNormal();
    Vector4 GetColor();
  }


  public class VtxOpcodeCommand : IOpcodeCommand {
    public IReadOnlyList<IVtx> Vertices { get; set; }
    public byte IndexToBeginStoringVertices { get; set; }
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

  public class SetEnvColorOpcodeCommand : IOpcodeCommand {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }

  public class SetFogColorOpcodeCommand : IOpcodeCommand {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }
}