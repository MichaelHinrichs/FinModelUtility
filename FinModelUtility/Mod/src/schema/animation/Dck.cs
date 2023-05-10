using schema.binary;
using schema.binary.attributes.ignore;
using schema.binary.attributes.size;


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
    public uint JointCount { get; private set; }
    public uint FrameCount { get; set; }

    [ArrayLengthSource(SchemaIntegerType.INT32)]
    public float[] ScaleValues { get; set; }

    [ArrayLengthSource(SchemaIntegerType.INT32)]
    public float[] RotationValues { get; set; }

    [ArrayLengthSource(SchemaIntegerType.INT32)]
    public float[] PositionValues { get; set; }

    [ArrayLengthSource(nameof(JointCount))]
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
        new DckAxis(),
        new DckAxis(),
        new DckAxis(),
    };
  }

  [BinarySchema]
  public partial class DckAxis : IDcxAxis {
    public int FrameCount { get; set; }
    public int FrameOffset { get; set; }
    private readonly int padding_ = 0;
  }
}