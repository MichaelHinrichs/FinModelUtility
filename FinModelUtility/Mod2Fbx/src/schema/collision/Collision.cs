using System;
using System.Collections.Generic;
using System.IO;

using schema;

namespace mod.schema.collision {
  [Schema]
  public partial class BaseRoomInfo : IBiSerializable {
    public uint index = 0;
  }

  [Schema]
  public partial class BaseCollTriInfo : IBiSerializable {
    public uint mapCode = 0;
    public readonly Vector3i indice = new();
    
    public ushort unknown2 = 0;
    public ushort unknown3 = 0;
    public ushort unknown4 = 0;
    public ushort unknown5 = 0;

    public readonly Plane plane = new();
  }

  public class CollTriInfo : IBiSerializable {
    public readonly List<BaseRoomInfo> roominfo = new();
    public readonly List<BaseCollTriInfo> collinfo = new();

    public void Read(EndianBinaryReader er) {
      var numColInfos = er.ReadUInt32();
      var numRoomInfos = er.ReadUInt32();

      this.collinfo.Clear();
      this.roominfo.Clear();

      er.Align(0x20);
      for (var i = 0; i < numRoomInfos; ++i) {
        this.roominfo.Add(er.ReadNew<BaseRoomInfo>());
      }

      er.Align(0x20);
      for (var i = 0; i < numColInfos; ++i) {
        this.collinfo.Add(er.ReadNew<BaseCollTriInfo>());
      }

      er.Align(0x20);
    }

    public void Write(EndianBinaryWriter ew) {
      var start = ew.StartChunk(0x100);
      ew.WriteInt32(this.collinfo.Count);
      ew.WriteInt32(this.roominfo.Count);

      ew.Align(0x20);
      foreach (var info in this.roominfo) {
        info.Write(ew);
      }

      ew.Align(0x20);
      foreach (var info in this.collinfo) {
        info.Write(ew);
      }

      ew.FinishChunk(start);
    }
  }

  [Schema]
  public partial class CollGroup : IBiSerializable {
    public ushort NumUnknown1 { get; set; }
    public ushort NumUnknown2 { get; set; }

    [ArrayLengthSource(nameof(NumUnknown2))]
    public uint[] unknown2 = Array.Empty<uint>();

    [ArrayLengthSource(nameof(NumUnknown1))]
    public byte[] unknown1 = Array.Empty<byte>();
  }

  public class CollGrid : IBiSerializable {
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
      writer.WriteSingle(this.unknown1);
      writer.WriteUInt32(this.gridX);
      writer.WriteUInt32(this.gridY);

      writer.WriteInt32(this.groups.Count);
      foreach (var group in this.groups) {
        group.Write(writer);
      }

      foreach (var i in this.unknown2) {
        writer.WriteInt32(i);
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