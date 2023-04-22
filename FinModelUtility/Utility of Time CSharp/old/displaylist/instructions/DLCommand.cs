using System.Collections.Generic;

namespace UoT {
  public class DLCommand : IDisplayListInstruction {
    public byte Opcode { get; private set; }
    public string Name => DLCommand.GetNameOfOpcode_(this.Opcode);


    public uint Low { get; private set; }
    public uint High { get; private set; }
    public byte[] CMDParams { get; } = new byte[8];


    public DLCommand() {}
    public DLCommand(byte opcode) => this.Update(opcode, 0, 0);
    public DLCommand(IList<byte> src, uint offset) => this.Update(src, offset);

    public DLCommand(byte opcode, uint low, uint high)
      => this.Update(opcode, low, high);

    public DLCommand(uint low, uint high) => this.Update(low, high);


    public void Update(IList<byte> src, uint offset)
      => this.Update(IoUtil.ReadUInt32(src, offset),
                     IoUtil.ReadUInt32(src, offset + 4));

    public void Update(byte opcode, uint low, uint high)
      => this.Update((uint) ((opcode << 24) | (low & 0x00ffffff)),
                     high);

    public void Update(uint low, uint high) {
      this.Low = low & 0x00ffffff;
      for (var i = 0; i < 4; ++i) {
        this.CMDParams[i] = (byte) IoUtil.ShiftR(low, (3 - i) * 8, 8);
      }

      this.High = high;
      for (var i = 0; i < 4; ++i) {
        this.CMDParams[4 + i] = (byte) IoUtil.ShiftR(high, (3 - i) * 8, 8);
      }

      this.Opcode = this.CMDParams[0];
    }

    private static string GetNameOfOpcode_(byte opcode) => opcode switch {
          (byte) RDP.G_RDPPIPESYNC => "G_RDPPIPESYNC (unemulated)",
          (byte) RDP.G_RDPLOADSYNC => "G_RDPLOADSYNC (unemulated)",
          (byte) RDP.G_SETENVCOLOR => "G_SETENVCOLOR",
          (byte) RDP.G_SETPRIMCOLOR => "G_SETPRIMCOLOR",
          (byte) RDP.G_SETTIMG => "G_SETTIMG",
          (byte) RDP.G_LOADTLUT => "G_LOADTLUT",
          (byte) RDP.G_LOADBLOCK => "G_LOADBLOCK",
          (byte) RDP.G_SETTILESIZE => "G_SETTILESIZE",
          (byte) RDP.G_SETTILE => "G_SETTILE",
          (byte) RDP.G_SETCOMBINE => "G_SETCOMBINE",
          (byte) F3DZEX.TEXTURE => "F3DEX2_TEXTURE",
          (byte) F3DZEX.GEOMETRYMODE => "F3DEX2_GEOMETRYMODE",
          (byte) F3DZEX.SETOTHERMODE_H => "F3DEX2_SETOTHERMODE_H (partial)",
          (byte) F3DZEX.SETOTHERMODE_L => "F3DEX2_SETOTHERMODE_L",
          (byte) F3DZEX.MTX => "F3DEX2_MTX (unemulated)",
          (byte) F3DZEX.VTX => "F3DEX2_VTX",
          (byte) F3DZEX.MODIFYVTX => "F3DEX2_MODIFYVTX",
          (byte) F3DZEX.TRI1 => "F3DEX2_TRI1",
          (byte) F3DZEX.TRI2 => "F3DEX2_TRI2",
          (byte) F3DZEX.CULLDL => "F3DEX2_CULLDL (unemulated)",
          (byte) F3DZEX.DL => "F3DEX2_DL (unemulated)",
          (byte) F3DZEX.ENDDL => "F3DEX2_ENDDL",
          _ => "Unrecognized (0x" + opcode.ToString("X2") + ")"
      };
  }
}