using System.Collections;

namespace UoT {
  /* TODO ERROR: Skipped RegionDirectiveTrivia */
  public struct Color3UByte {
    public byte r;
    public byte g;
    public byte b;
  }

  public struct Color4UByte {
    public byte r;
    public byte g;
    public byte b;
    public byte a;
  }

  public struct ZSegment {
    public byte Bank;
    public uint Offset;
  }

  public struct ShaderCache {
    public uint MUXS0;
    public uint MUXS1;
    public string[] Equation;
    public uint FragShader;
  }
  

  public struct PickableItems {
    public byte r;
    public byte g;
    public byte b;
    public byte a;
  }
  
  /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
}