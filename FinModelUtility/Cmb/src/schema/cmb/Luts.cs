﻿using schema;

namespace cmb.schema.cmb {
  /// <summary>
  ///   "LUT" stands for "lookup table". (But where is this actually used...?)
  /// </summary>
  [BinarySchema]
  public partial class Luts : IBiSerializable {
    private uint LutSetCount;
    public uint unk;

    [ArrayLengthSource(nameof(LutSetCount))]
    public uint[] Offset;

    [ArrayLengthSource(nameof(LutSetCount))]
    public LutSet[] luts;
  }

  [BinarySchema]
  public partial class LutSet : IBiSerializable {
    public ushort BitFlags; //Not sure
    private ushort KeyCount; //Keyframes
    public short Start;
    public short End;
    [ArrayLengthSource(nameof(KeyCount))]
    public LutKeyframe[] Frame;
    public float unk1;
    public float unk2;
  }

  [BinarySchema]
  public partial class LutKeyframe : IBiSerializable {
    public float InSlope;
    public float OutSlope;
    public int Frame;
    public float Value;
  }
}