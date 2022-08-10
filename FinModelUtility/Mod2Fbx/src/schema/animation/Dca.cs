using System.IO;
using System.Text;
using System.Threading.Tasks;

using schema;
using schema.attributes.ignore;
using schema.util;


namespace mod.schema.animation {
  public class Dca : IDcx {
    public string Name { get; set; }
    public IDcxAnimationData AnimationData { get; } = new DcaAnimationData();

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
              length => (uint) length.Result));

      ew.WriteInt32(this.Name.Length);
      ew.WriteString(this.Name);

      var actualLengthTask = ew.EnterBlockAndGetDelayedLength(
          (_, _) => { this.AnimationData.Write(ew); });
      actualLengthTask.ContinueWith(
          length =>
              beforeLengthTask.SetResult(length.Result));
    }
  }


  [Schema]
  public partial class DcaAnimationData : IDcxAnimationData {
    public uint JointCount { get; set; }
    public uint FrameCount { get; set; }

    [ArrayLengthSource(SchemaIntType.INT32)]
    public float[] ScaleValues { get; set; }

    [ArrayLengthSource(SchemaIntType.INT32)]
    public float[] RotationValues { get; set; }

    [ArrayLengthSource(SchemaIntType.INT32)]
    public float[] PositionValues { get; set; }

    [ArrayLengthSource(nameof(JointCount))]
    public DcaJointData[] JointDataList { get; set; }

    [Ignore]
    IDcxJointData[] IDcxAnimationData.JointDataList {
      get => this.JointDataList;
      set => this.JointDataList = value as DcaJointData[];
    }
  }

  [Schema]
  public partial class DcaJointData : IDcxJointData {
    public int JointIndex { get; set; }
    public int ParentIndex { get; set; }

    public IDcxAxes ScaleAxes { get; } = new DcaAxes();
    public IDcxAxes RotationAxes { get; } = new DcaAxes();
    public IDcxAxes PositionAxes { get; } = new DcaAxes();
  }

  [Schema]
  public partial class DcaAxes : IDcxAxes {
    public IDcxAxis[] Axes { get; } = {
        new DcaAxis(),
        new DcaAxis(),
        new DcaAxis(),
    };
  }

  [Schema]
  public partial class DcaAxis : IDcxAxis {
    public int FrameCount { get; set; }
    public int FrameOffset { get; set; }
  }
}