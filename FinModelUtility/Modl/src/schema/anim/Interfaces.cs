namespace modl.schema.anim {
  public interface IBwAnim {
    List<IBwAnimBone> AnimBones { get; }
    List<AnimBoneFrames> AnimBoneFrames { get; }
  }

  public interface IBwAnimBone {
    uint PositionKeyframeCount { get; }
    uint RotationKeyframeCount { get; }

    float XPosDelta { get; }
    float YPosDelta { get; }
    float ZPosDelta { get; }
    float XPosMin { get; }
    float YPosMin { get; }
    float ZPosMin { get; }

    uint WeirdId { get; }
  }
}
