using System;
using System.Collections;
using System.Collections.Generic;

using fin.util.asserts;

namespace mod.schema {
  public enum VtxFmt {
    NOT_PRESENT = 0,
    DIRECT = 1,
    INDEX8 = 2,
    INDEX16 = 3,
  }

  public enum Vtx {
    PosMatIdx,
    Tex0MatIdx,
    Tex1MatIdx,
    Tex2MatIdx,
    Tex3MatIdx,
    Tex4MatIdx,
    Tex5MatIdx,
    Tex6MatIdx,
    Tex7MatIdx,
    Position,
    Normal,
    Color0,
    Color1,
    Tex0Coord,
    Tex1Coord,
    Tex2Coord,
    Tex3Coord,
    Tex4Coord,
    Tex5Coord,
    Tex6Coord,
    Tex7Coord
  }

  public class VertexDescriptor : IEnumerable<(Vtx, VtxFmt?)> {
    public bool posMat = false;

    public bool[] texMat = new[] {
        false,
        false,
        false,
        false,

        false,
        false,
        false,
        false,
    };

    public VtxFmt position = VtxFmt.NOT_PRESENT;

    public VtxFmt normal = VtxFmt.NOT_PRESENT;
    public bool useNbt;

    public VtxFmt color0 = VtxFmt.NOT_PRESENT;
    public VtxFmt color1 = VtxFmt.NOT_PRESENT;

    public readonly VtxFmt[] texcoord = new[] {
        VtxFmt.NOT_PRESENT,
        VtxFmt.NOT_PRESENT,
        VtxFmt.NOT_PRESENT,
        VtxFmt.NOT_PRESENT,

        VtxFmt.NOT_PRESENT,
        VtxFmt.NOT_PRESENT,
        VtxFmt.NOT_PRESENT,
        VtxFmt.NOT_PRESENT,
    };

    public IEnumerator<(Vtx, VtxFmt?)> GetEnumerator()
      => this.ActiveAttributes();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<(Vtx, VtxFmt?)> ActiveAttributes() {
      foreach (var attrObj in Enum.GetValues(typeof(Vtx))) {
        var attr = (Vtx) attrObj;
        if (!this.Exists(attr)) {
          continue;
        }

        if (attr is >= Vtx.Position and <= Vtx.Tex7Coord) {
          yield return (attr, this.GetFormat(attr));
        } else {
          yield return (attr, null);
        }
      }
    }

    public bool Exists(Vtx enumval) {
      if (enumval is >= Vtx.Tex0MatIdx and <= Vtx.Tex7MatIdx) {
        var texMatId = enumval - Vtx.Tex0MatIdx;
        return this.texMat[texMatId];
      }

      if (enumval is >= Vtx.Tex0Coord and <= Vtx.Tex7Coord) {
        var texCoordId = enumval - Vtx.Tex0Coord;
        return this.texcoord[texCoordId] != VtxFmt.NOT_PRESENT;
      }

      if (enumval == Vtx.PosMatIdx) {
        return this.posMat;
      }

      if (enumval == Vtx.Position) {
        return this.position != VtxFmt.NOT_PRESENT;
      }

      if (enumval == Vtx.Normal) {
        return this.normal != VtxFmt.NOT_PRESENT;
      }

      if (enumval == Vtx.Color0) {
        return this.color0 != VtxFmt.NOT_PRESENT;
      }

      if (enumval == Vtx.Color1) {
        return this.color1 != VtxFmt.NOT_PRESENT;
      }

      Asserts.Fail($"Unknown enum for exists: {enumval}");
      return false;
    }

    public VtxFmt GetFormat(Vtx enumval) {
      if (enumval == Vtx.Position) {
        return this.position;
      }

      if (enumval == Vtx.Normal) {
        return this.normal;
      }

      if (enumval == Vtx.Color0) {
        return this.color0;
      }

      if (enumval == Vtx.Color1) {
        return this.color1;
      }

      if (enumval is >= Vtx.Tex0Coord and <= Vtx.Tex7Coord) {
        var texcoordid = enumval - Vtx.Tex0Coord;
        return this.texcoord[texcoordid];
      }

      Asserts.Fail($"Unknown enum for format: {enumval}");
      return VtxFmt.NOT_PRESENT;
    }

    /*def from_value(self, val):
    self.posmat = (val & 0b1) == 1
    val = val >> 1
    for i in range(8) :
        self.texmat[i] = (val & 0b1) == 1
    val = val >> 1

    self.position = get_vtxformat(val & 0b11)
    val = val >> 2
    self.normal = get_vtxformat(val & 0b11)
    val = val >> 2
    self.color0 = get_vtxformat(val & 0b11)
    val = val >> 2
    self.color1 = get_vtxformat(val & 0b11)
    val = val >> 2
    for i in range(8) :
        self.texcoord[i] = get_vtxformat(val & 0b11)
    val = val >> 2*/

    public void FromPikmin1(uint val, bool hasNormals = false) {
      this.position =
          VtxFmt.INDEX16; // Position is implied to be always enabled

      this.posMat = (val & 0b1) == 1;
      val = val >> 1;

      this.texMat[1] = (val & 0b1) == 1;
      val = val >> 1;

      if ((val & 0b1) == 1) {
        this.color0 = VtxFmt.INDEX16;
      }
      val = val >> 1;

      for (var i = 0; i < 8; ++i) {
        if ((val & 0b1) == 1) {
          this.texcoord[i] = VtxFmt.INDEX16;
        }
        val = val >> 1;
      }

      this.useNbt = (val & 0x20) != 0;

      if (hasNormals) {
        this.normal = VtxFmt.INDEX16;
      }
    }

    /*def __str__(self) :
        return str([x for x in self.active_attributes()])

    def write_dmd(self, f):
        f.write("vcd ")
        for  attr in self.all_attributes():
            if attr is None:
                f.write("0 ")
            else:
                attr, fmt = attr
                if fmt is None:
                    f.write("1 ")
                elif fmt == VTXFMT.INDEX16:
                    f.write("1 ")
                else:
                    f.write("1 ")
        f.write("\n")*/
  }
}