using System.Collections.Generic;
using System.IO;

namespace mod.gcn.collision {
  public class BaseRoomInfo : IGcnSerializable {
    public uint index = 0;

    public void Read(EndianBinaryReader reader) {
      this.index = reader.ReadUInt32();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.index);
    }
  }

  public class BaseCollTriInfo : IGcnSerializable {
    public uint mapCode = 0;
    public readonly Vector3i indice = new();
    public ushort unknown2 = 0;
    public ushort unknown3 = 0;
    public ushort unknown4 = 0;
    public ushort unknown5 = 0;
    public readonly Plane plane = new();

    public void Read(EndianBinaryReader reader) {
      this.mapCode = reader.ReadUInt32();
      this.indice.Read(reader);

      this.unknown2 = reader.ReadUInt16();
      this.unknown3 = reader.ReadUInt16();
      this.unknown4 = reader.ReadUInt16();
      this.unknown5 = reader.ReadUInt16();

      this.plane.Read(reader);
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.mapCode);
      this.indice.Write(writer);

      writer.Write(this.unknown2);
      writer.Write(this.unknown3);
      writer.Write(this.unknown4);
      writer.Write(this.unknown5);

      this.plane.Write(writer);
    }
  }

  public class CollTriInfo : IGcnSerializable {
    public readonly List<BaseRoomInfo> roominfo = new();
    public readonly List<BaseCollTriInfo> collinfo = new();

    public void Read(EndianBinaryReader reader) {
      var numColInfos = reader.ReadUInt32();
      var numRoomInfos = reader.ReadUInt32();

      this.collinfo.Clear();
      this.roominfo.Clear();

      reader.Align(0x20);
      for (var i = 0; i < numRoomInfos; ++i) {
        var roomInfo = new BaseRoomInfo();
        roomInfo.Read(reader);
        this.roominfo.Add(roomInfo);
      }

      reader.Align(0x20);
      for (var i = 0; i < numColInfos; ++i) {
        var colInfo = new BaseCollTriInfo();
        colInfo.Read(reader);
        this.collinfo.Add(colInfo);
      }

      reader.Align(0x20);
    }

    public void Write(EndianBinaryWriter writer) {
      var start = writer.StartChunk(0x100);
      writer.Write(this.collinfo.Count);
      writer.Write(this.roominfo.Count);

      writer.Align(0x20);
      foreach (var info in this.roominfo) {
        info.Write(writer);
      }

      writer.Align(0x20);
      foreach (var info in this.collinfo) {
        info.Write(writer);
      }

      writer.FinishChunk(start);
    }
  }

  public class CollGroup : IGcnSerializable {
    public readonly List<byte> unknown1 = new();
    public readonly List<uint> unknown2 = new();

    public void Read(EndianBinaryReader reader) {
      var numUnknown1 = reader.ReadUInt16();
      var numUnknown2 = reader.ReadUInt16();

      this.unknown2.Clear();
      for (var i = 0; i < numUnknown2; ++i) {
        this.unknown2.Add(reader.ReadUInt32());
      }

      this.unknown1.Clear();
      for (var i = 0; i < numUnknown1; ++i) {
        this.unknown1.Add(reader.ReadByte());
      }
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write((ushort) this.unknown1.Count);
      writer.Write((ushort) this.unknown2.Count);

      foreach (var i in this.unknown2) {
        writer.Write(i);
      }

      foreach (var i in this.unknown1) {
        writer.Write(i);
      }
    }
  }

  public class CollGrid : IGcnSerializable {
    public readonly Vector3f boundsMin = new();
    public readonly Vector3f boundsMax = new();
    public float unknown1 = 0;
    public uint gridX = 0;
    public uint gridY = 0;
    public readonly List<CollGroup> groups = new();
    public readonly List<int> unknown2 = new();

    public void Read(EndianBinaryReader reader) {
      reader.Align(0x20);
      this.boundsMin.Read(reader);
      this.boundsMax.Read(reader);
      this.unknown1 = reader.ReadSingle();
      this.gridX = reader.ReadUInt32();
      this.gridY = reader.ReadUInt32();

      var numGroups = reader.ReadUInt32();
      this.groups.Clear();
      for (var i = 0; i < numGroups; ++i) {
        var group = new CollGroup();
        group.Read(reader);
        this.groups.Add(group);
      }

      this.unknown2.Clear();
      for (var i = 0; i < this.gridX * this.gridY; ++i) {
        this.unknown2.Add(reader.ReadInt32());
      }
      reader.Align(0x20);
    }

    public void Write(EndianBinaryWriter writer) {
      var start = writer.StartChunk(0x110);
      writer.Align(0x20);
      this.boundsMin.Write(writer);
      this.boundsMax.Write(writer);
      writer.Write(this.unknown1);
      writer.Write(this.gridX);
      writer.Write(this.gridY);

      writer.Write(this.groups.Count);
      foreach (var group in this.groups) {
        group.Write(writer);
      }

      foreach (var i in this.unknown2) {
        writer.Write(i);
      }
      writer.Align(0x20);
      writer.FinishChunk(start);
    }

    public void Clear() {
      this.boundsMin.Reset();
      this.boundsMax.Reset();
      this.unknown1 = 0;
      this.gridX = 0;
      this.gridY = 0;
      this.groups.Clear();
      this.unknown2.Clear();
    }
  }
}