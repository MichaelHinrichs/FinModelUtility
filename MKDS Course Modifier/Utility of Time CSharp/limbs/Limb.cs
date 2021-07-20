using System;
using System.Runtime.InteropServices;

namespace UoT.limbs {
  public interface IOldLimb {
    /// <summary>
    ///   Whether the limb is "visible", as determined by having a nonzero
    ///   display list address.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    ///   Limb hierarchies include invisible entries, but matrices are indexed
    ///   by *visible* index.
    /// </summary>
    int VisibleIndex { get; set; }

    // X/Y/Z coordinates of the limb.
    short x { get; }
    short y { get; }
    short z { get; }

    sbyte firstChild { get; }
    sbyte nextSibling { get; }
  }

  [StructLayout(LayoutKind.Explicit, Pack = 1)]
  public class LimbData {
    [FieldOffset(0 * 8)] public UInt16 x;
    [FieldOffset(2 * 8)] public UInt16 y;
    [FieldOffset(4 * 8)] public UInt16 z;
    [FieldOffset(6 * 8)] public SByte firstChildIndex;
    [FieldOffset(7 * 8)] public SByte nextSiblingIndex;
    [FieldOffset(8 * 8)] public UInt32 displayListAddress;
  }

  public class Limb : IOldLimb {
    public bool Visible { get; set; }
    public int VisibleIndex { get; set; }

    // X/Y/Z coordinates of the limb.
    public short x { get; set; }
    public short y { get; set; }
    public short z { get; set; }


    // RGB color of this limb, used as a unique identifier when picking via the
    // mouse.
    public double r { get; set; }
    public double g { get; set; }
    public double b { get; set; }


    public sbyte firstChild { get; set; }
    public sbyte nextSibling { get; set; }

    public uint DisplayListAddress { get; set; }
    public uint DisplayListLow { get; set; }
  }
}