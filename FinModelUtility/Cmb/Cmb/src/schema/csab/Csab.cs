using System.Collections.Generic;
using System.IO;

using fin.util.asserts;

using schema.binary;
using schema.binary.attributes;


namespace cmb.schema.csab {
  [Endianness(Endianness.LittleEndian)]
  public class Csab : IBinaryDeserializable {
    public uint Version { get; set; }
    public bool IsPastVersion4 => this.Version > 4;

    public uint Duration { get; set; }

    public Dictionary<int, AnimationNode>
        BoneIndexToAnimationNode { get; set; } = new();

    public void Read(IEndianBinaryReader r) {
      var basePosition = r.Position;

      r.AssertMagicText("csab");
      var size = r.ReadUInt32();

      // Subversion?
      this.Version = r.ReadUInt32();

      r.AssertUInt32(0x00);

      float[] unk;
      if (IsPastVersion4) {
        // TODO: Might have to do with translations?
        unk = r.ReadSingles(3);
      }

      // Num animations?
      r.AssertUInt32(0x01);
      // Location?
      var animationOffset = r.ReadUInt32();

      r.AssertUInt32(0x00);
      r.AssertUInt32(0x00);
      r.AssertUInt32(0x00);
      r.AssertUInt32(0x00);

      this.Duration = r.ReadUInt32();

      // Jasper and M-1 believe this is loop mode, where 0 is a non-looping and
      // 1 is looping. But this doesn't seem to actually correlate with the
      // animations you'd expect to be looping vs. non-looping?
      var loopMode = r.ReadUInt32();

      var anodCount = r.ReadUInt32();
      var boneCount = r.ReadUInt32();
      Asserts.True(anodCount <= boneCount);

      // Jasper: This appears to be an inverse of the bone index in each array,
      // probably for fast binding?
      var boneToAnimationTable = new short[boneCount];
      //var boneTableIdx = basePosition + animationOffset + 0x20;
      //r.Position = boneTableIdx;
      for (var i = 0; i < boneCount; ++i) {
        boneToAnimationTable[i] = r.ReadInt16();
      }

      // TODO(jstpierre): This doesn't seem like a Grezzo thing to do.
      var anodTableIdx = this.Align_(r.Position, 0x04);
      r.Position = anodTableIdx;

      var animationNodes = new AnimationNode[anodCount];
      for (var i = 0; i < anodCount; ++i) {
        var anod = new AnimationNode(this);

        var offset = r.ReadUInt32();
        r.Subread(basePosition + animationOffset + offset, sr => anod.Read(sr));

        animationNodes[i] = anod;
      }

      for (var b = 0; b < boneCount; ++b) {
        var anodIndex = boneToAnimationTable[b];
        if (anodIndex != -1) {
          var anod = animationNodes[anodIndex];
          this.BoneIndexToAnimationNode[b] = anod;
        }
      }
    }

    private long Align_(long n, int multiple) {
      var mask = multiple - 1;
      return (n + mask) & ~mask;
    }
  }
}