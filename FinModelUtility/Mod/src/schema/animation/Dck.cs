using System.IO;
using System.Text;
using System.Threading.Tasks;

using schema;
using schema.attributes.ignore;
using schema.util;


namespace mod.schema.animation {
  public class Dck : IDcx {
    public string Name { get; set; }
    public IDcxAnimationData AnimationData { get; } = new DckAnimationData();

    public void Read(EndianBinaryReader er) {
      // TODO: Pull this out as a common "animation header"
      uint animationLength;
      long startPosition;
      {
        animationLength = er.ReadUInt32();

        var nameLength = er.ReadInt32();
        this.Name = er.ReadString(Encoding.ASCII, nameLength);

        startPosition = er.Position;
      }

      this.AnimationData.Read(er);

      var endPosition = er.Position;
      var readLength = endPosition - startPosition;
      Asserts.Equal(animationLength,
                    readLength,
                    "Read unexpected number of bytes in animation!");
    }

    public void Write(EndianBinaryWriter ew) {
      var beforeLengthTask = new TaskCompletionSource<long>();
      ew.WriteUInt32Delayed(
          beforeLengthTask.Task.ContinueWith(
              length => (uint)length.Result));

      ew.WriteInt32(this.Name.Length);
      ew.WriteString(this.Name);

      {
        var sew = ew.EnterBlock(out var actualLengthTask);
        this.AnimationData.Write(sew);
        actualLengthTask.ContinueWith(
            length =>
                beforeLengthTask.SetResult(length.Result));
      }
    }
  }

  [BinarySchema]
  public partial class DckAnimationData : IDcxAnimationData {
    public uint JointCount { get; set; }
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
    public int Unk { get; set; }
  }
}