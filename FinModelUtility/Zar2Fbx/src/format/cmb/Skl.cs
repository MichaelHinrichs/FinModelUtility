using System.IO;

using fin.io;
using fin.util.strings;

namespace zar.format.cmb {
  public class Skl : IDeserializable {
    public uint chunkSize;
    public uint unkFlags;
    public Bone[] bones;

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("skl" + AsciiUtil.GetChar(0x20));

      this.chunkSize = r.ReadUInt32();
      this.bones = new Bone[r.ReadUInt32()];
      // M-1: Only value found is "2", possibly "IsTranslateAnimationEnabled"
      // flag (I can't find a change in-game)
      this.unkFlags = r.ReadUInt32();

      for (var i = 0; i < this.bones.Length; ++i) {
        var bone = new Bone();
        bone.Read(r);
        this.bones[i] = bone;
      }

      foreach (var bone in this.bones) {
        var parentId = bone.parentId;
        if (parentId != -1) {
          var parent = this.bones[parentId];
          bone.parent = parent;
          parent.children.Add(bone);
        }
      }
    }
  }
}
