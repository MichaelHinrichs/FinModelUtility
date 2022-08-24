namespace modl.schema.anim {
  public class AnimBoneFrames {
    public List<(float, float, float)> PositionFrames { get; } = new();
    public List<(float, float, float, float)> RotationFrames { get; } = new();
  }
}
