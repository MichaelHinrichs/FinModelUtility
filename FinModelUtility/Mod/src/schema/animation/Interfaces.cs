using schema;


namespace mod.schema.animation {
  public interface IDcx : IBiSerializable {
    string Name { get; set; }
    IDcxAnimationData AnimationData { get; }
  }

  public interface IDcxAnimationData : IBiSerializable {
    uint JointCount { get; }
    uint FrameCount { get; set; }

    float[] ScaleValues { get; set; }
    float[] RotationValues { get; set; }
    float[] PositionValues { get; set; }

    IDcxJointData[] JointDataList { get; set; }
  }

  public interface IDcxJointData : IBiSerializable {
    int JointIndex { get; set; }
    int ParentIndex { get; set; }

    IDcxAxes ScaleAxes { get; }
    IDcxAxes RotationAxes { get; }
    IDcxAxes PositionAxes { get; }
  }

  public interface IDcxAxes : IBiSerializable {
    IDcxAxis[] Axes { get; }
  }

  public interface IDcxAxis : IBiSerializable {
    int FrameCount { get; set; }
    int FrameOffset { get; set; }
  }
}