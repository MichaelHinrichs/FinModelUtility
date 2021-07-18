using System.Collections.Generic;

using UoT.util;

namespace UoT {
  // Based on the structs at:
  // https://wiki.cloudmodding.com/oot/Animation_Format#Normal_Animations

  public class NormalAnimation : IAnimation {
    public ushort[] Angles = new ushort[0];
    public NormalAnimationTrack[] Tracks = new NormalAnimationTrack[0];
    public uint TrackOffset;
    public uint AngleCount;

    public ushort FrameCount { get; set; }

    public Vec3s? Position { get; set; }
    public int PositionCount => 1;
    public Vec3s GetPosition(int _) => Asserts.Assert(this.Position);

    public int TrackCount => this.Tracks.Length;
    public IAnimationTrack GetTrack(int i) => this.Tracks[i];

    public FacialState GetFacialState(int _) => FacialState.DEFAULT;
  }

  public class NormalAnimationTrack : IAnimationTrack {
    public int Type { get; set; } // 0 = constant, 1 = keyframe
    public IList<ushort> Frames { get; set; } = new ushort[0];
  }
}
