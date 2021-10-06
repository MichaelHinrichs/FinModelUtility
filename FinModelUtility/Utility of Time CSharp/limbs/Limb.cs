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
    [FieldOffset(0)] public UInt16 x;
    [FieldOffset(2)] public UInt16 y;
    [FieldOffset(4)] public UInt16 z;
    [FieldOffset(6)] public SByte firstChild;
    [FieldOffset(7)] public SByte nextSibling;
    [FieldOffset(8)] public UInt32 displayListAddress;
  }

  public class Limb : IOldLimb {
    public Limb(LimbData data) {
      // TODO: This feels like a bug, what type of data is this?
      this.x = (short) data.x;
      this.y = (short) data.y;
      this.z = (short) data.z;

      this.firstChild = data.firstChild;
      this.nextSibling = data.nextSibling;

      this.DisplayListAddress = data.displayListAddress;
      this.Visible = data.displayListAddress > 0;
    }

    public bool Visible { get; }
    public int VisibleIndex { get; set; }

    // X/Y/Z coordinates of the limb.
    public short x { get; }
    public short y { get; }
    public short z { get; }


    // RGB color of this limb, used as a unique identifier when picking via the
    // mouse.
    public double r { get; set; }
    public double g { get; set; }
    public double b { get; set; }


    public sbyte firstChild { get; }
    public sbyte nextSibling { get; }

    public uint DisplayListAddress { get; }
  }
}