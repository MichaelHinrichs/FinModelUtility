using System.Collections.Generic;
using System.IO;

namespace mod.gcn {
  public class JointMatPoly : IGcnSerializable {
    public ushort matIdx = 0;
    public ushort meshIdx = 0;

    public void Read(EndianBinaryReader reader) {
      this.matIdx = reader.ReadUInt16();
      this.meshIdx = reader.ReadUInt16();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.matIdx);
      writer.Write(this.meshIdx);
    }
  }

  public class Joint : IGcnSerializable {
    public uint parentIdx = 0;
    public uint flags = 0;
    public readonly Vector3f boundsMin = new();
    public readonly Vector3f boundsMax = new();
    public float volumeRadius = 0;
    public readonly Vector3f scale = new();
    public readonly Vector3f rotation = new();
    public readonly Vector3f position = new();
    public readonly List<JointMatPoly> matpolys = new();

    public void Read(EndianBinaryReader reader) {
      this.parentIdx = reader.ReadUInt32();
      this.flags = reader.ReadUInt32();
      this.boundsMax.Read(reader);
      this.boundsMin.Read(reader);
      this.volumeRadius = reader.ReadSingle();
      this.scale.Read(reader);
      this.rotation.Read(reader);
      this.position.Read(reader);

      this.matpolys.Clear();
      var numMatPolys = reader.ReadUInt32();
      for (var i = 0; i < numMatPolys; ++i) {
        var matPoly = new JointMatPoly();
        matPoly.Read(reader);
        this.matpolys.Add(matPoly);
      }
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.parentIdx);
      writer.Write(this.flags);
      this.boundsMax.Write(writer);
      this.boundsMin.Write(writer);
      writer.Write(this.volumeRadius);
      this.scale.Write(writer);
      this.rotation.Write(writer);
      this.position.Write(writer);

      writer.Write(this.matpolys.Count);
      foreach (var matPoly in this.matpolys) {
        matPoly.Write(writer);
      }
    }
  }
}