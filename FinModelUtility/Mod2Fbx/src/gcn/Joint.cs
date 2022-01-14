using System.Collections.Generic;
using System.IO;

using schema;

namespace mod.gcn {
  [Schema]
  public partial class JointMatPoly : IGcnSerializable {
    public ushort matIdx = 0;
    public ushort meshIdx = 0;

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.matIdx);
      writer.Write(this.meshIdx);
    }
  }

  [Schema]
  public partial class Joint : IGcnSerializable {
    public uint parentIdx = 0;
    public uint flags = 0;
    public readonly Vector3f boundsMax = new();    
    public readonly Vector3f boundsMin = new();
    public float volumeRadius = 0;
    public readonly Vector3f scale = new();
    public readonly Vector3f rotation = new();
    public readonly Vector3f position = new();

    [ArrayLengthSource(IntType.UINT32)]
    public JointMatPoly[] matpolys;

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.parentIdx);
      writer.Write(this.flags);
      this.boundsMax.Write(writer);
      this.boundsMin.Write(writer);
      writer.Write(this.volumeRadius);
      this.scale.Write(writer);
      this.rotation.Write(writer);
      this.position.Write(writer);

      writer.Write(this.matpolys.Length);
      foreach (var matPoly in this.matpolys) {
        matPoly.Write(writer);
      }
    }
  }
}