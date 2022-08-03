using System.Collections.Generic;
using System.IO;
using System.Numerics;

using fin.math;
using fin.math.matrix;

using schema;


namespace hw.granny3d {
  public class GrannySkeleton : IGrannySkeleton, IDeserializable {
    public string Name { get; private set; }
    public IList<IGrannyBone> Bones { get; } = new List<IGrannyBone>();
    public int LodType { get; private set; }

    public void Read(EndianBinaryReader er) {
      GrannyUtils.SubreadUInt64Pointer(
          er, ser => { this.Name = ser.ReadStringNT(); });

      var boneCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => {
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
      GrannyUtils.SubreadUInt64Pointer(
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

      ;
    }

    public class GrannyTransform : IGrannyTransform, IDeserializable {
      public GrannyTransformFlags Flags { get; private set; }
      public Vector3 Position { get; private set; }
      public Quaternion Orientation { get; private set; }
      public Vector3[] ScaleShear { get; } = new Vector3[3];

      public void Read(EndianBinaryReader er) {
        this.Flags = (GrannyTransformFlags) er.ReadInt32();

        this.Position =
            new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());

        this.Orientation = new Quaternion(er.ReadSingle(), er.ReadSingle(),
                                          er.ReadSingle(), er.ReadSingle());

        for (var i = 0; i < 3; ++i) {
          this.ScaleShear[i] =
              new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        }
      }
    }
  }
}