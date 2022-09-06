using fin.math;
using fin.math.matrix;

using schema;


namespace granny3d {
  public class GrannySkeleton : IGrannySkeleton, IDeserializable {
    public string Name { get; private set; }
    public IList<IGrannyBone> Bones { get; } = new List<IGrannyBone>();
    public int LodType { get; private set; }

    public void Read(EndianBinaryReader er) {
      GrannyUtils.SubreadRef(
          er, ser => { this.Name = ser.ReadStringNT(); });

      GrannyUtils.SubreadRefToArray(er, (ser, boneCount) => {
        for (var i = 0; i < boneCount; ++i) {
          Bones.Add(ser.ReadNew<GrannyBone>());
        }
      });
    }
  }

  public class GrannyBone : IGrannyBone, IDeserializable {
    public string Name { get; private set; }
    public int ParentIndex { get; private set; }
    public IGrannyTransform LocalTransform { get; private set; }
    public IFinMatrix4x4 InverseWorld4x4 { get; } = new FinMatrix4x4();
    public float LodError { get; private set; }

    public void Read(EndianBinaryReader er) {
      GrannyUtils.SubreadRef(
          er, ser => { this.Name = ser.ReadStringNT(); });

      this.ParentIndex = er.ReadInt32();

      // granny_transform
      var transform = new GrannyTransform();
      transform.Read(er);
      this.LocalTransform = transform;

      // inverse_world_4x4
      for (var y = 0; y < 4; ++y) {
        for (var x = 0; x < 4; x++) {
          this.InverseWorld4x4[x, y] = er.ReadSingle();
        }
      }

      // lod_error
      er.Position += 4;

      // extended_data
      er.Position += 2 * 8;
    }
  }
}