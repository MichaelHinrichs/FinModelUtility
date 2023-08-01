using schema.binary;
using schema.binary.attributes;


namespace mod.schema.animation {
  [BinarySchema]
  public partial class Dck : IDcx {
    [WSizeOfMemberInBytes(nameof(AnimationData))]
    private uint animationLength_;

    [StringLengthSource(SchemaIntegerType.INT32)]
    public string Name { get; set; }

    public IDcxAnimationData AnimationData { get; } = new DckAnimationData();

    public override string ToString() => this.Name;
  }


  [BinarySchema]
  public partial class DckAnimationData : IDcxAnimationData {
    [WLengthOfSequence(nameof(JointDataList))]
    private uint jointCount_ { get; private set; }

    public uint FrameCount { get; set; }

    [SequenceLengthSource(SchemaIntegerType.INT32)]
    public float[] ScaleValues { get; set; }

    [SequenceLengthSource(SchemaIntegerType.INT32)]
    public float[] RotationValues { get; set; }

    [SequenceLengthSource(SchemaIntegerType.INT32)]
    public float[] PositionValues { get; set; }

    [RSequenceLengthSource(nameof(jointCount_))]
    public DckJointData[] JointDataList { get; set; }

    [Ignore]
    IDcxJointData[] IDcxAnimationData.JointDataList {
      get => this.JointDataList;
      set => this.JointDataList = value as DckJointData[];
    }
  }

  [BinarySchema]
  public partial class DckJointData : IDcxJointData {
    public int JointIndex { get; set; }
    public int ParentIndex { get; set; }

    public IDcxAxes ScaleAxes { get; } = new DckAxes();
    public IDcxAxes RotationAxes { get; } = new DckAxes();
    public IDcxAxes PositionAxes { get; } = new DckAxes();
  }

  [BinarySchema]
  public partial class DckAxes : IDcxAxes {
    public IDcxAxis[] Axes { get; } = {
        new DckAxis(), new DckAxis(), new DckAxis(),
    };
  }

  [BinarySchema]
  public partial class DckAxis : IDcxAxis {
    public int FrameCount { get; set; }
    public int FrameOffset { get; set; }
    private readonly int padding_ = 0;
  }
}