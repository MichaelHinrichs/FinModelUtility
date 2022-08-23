using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using fin.schema.vector;

using schema;
using schema.attributes.align;
using schema.attributes.ignore;


namespace mod.schema.collision {
  [BinarySchema]
  public partial class BaseRoomInfo : IBiSerializable {
    public uint index = 0;
  }

  [BinarySchema]
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
      ew.WriteUInt32(0x100);

      var beforeLengthTask = new TaskCompletionSource<long>();
      ew.WriteUInt32Delayed(
          beforeLengthTask.Task.ContinueWith(
              length => (uint)length.Result));

      var actualLengthTask = ew.EnterBlockAndGetDelayedLength((_, _) => {
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

        ew.Align(0x20);
      });
      actualLengthTask.ContinueWith(
          length =>
              beforeLengthTask.SetResult(length.Result));
    }
  }

  [BinarySchema]
  public partial class CollGroup : IBiSerializable {
    public ushort NumUnknown1 { get; set; }
    public ushort NumUnknown2 { get; set; }

    [ArrayLengthSource(nameof(NumUnknown2))]
    public uint[] unknown2 = Array.Empty<uint>();

    [ArrayLengthSource(nameof(NumUnknown1))]
    public byte[] unknown1 = Array.Empty<byte>();
  }

  public class CollGrid : IBiSerializable {
    [Align(0x20)]
    public readonly Vector3f boundsMin = new();
    public readonly Vector3f boundsMax = new();

    public float unknown1 = 0;
    
    public uint gridX = 0;
    public uint gridY = 0;

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public readonly List<CollGroup> groups = new();

    [Ignore]
    private uint gridSize_ => this.gridX * this.gridY;

    [ArrayLengthSource(nameof(gridSize_))]
    public readonly List<int> unknown2 = new();

    [Align(0x20)]
    private readonly byte[] empty_ = new byte[0];

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
        this.groups.Add(reader.ReadNew<CollGroup>());
      }

      this.unknown2.Clear();
      for (var i = 0; i < this.gridX * this.gridY; ++i) {
        this.unknown2.Add(reader.ReadInt32());
      }
      reader.Align(0x20);
    }

    public void Write(EndianBinaryWriter ew) {
      ew.WriteUInt32(0x110);

      var beforeLengthTask = new TaskCompletionSource<long>();
      ew.WriteUInt32Delayed(
          beforeLengthTask.Task.ContinueWith(
              length => (uint) length.Result));

      var actualLengthTask = ew.EnterBlockAndGetDelayedLength(
          (_, _) => {
            ew.Align(0x20);
            this.boundsMin.Write(ew);
            this.boundsMax.Write(ew);
            ew.WriteSingle(this.unknown1);
            ew.WriteUInt32(this.gridX);
            ew.WriteUInt32(this.gridY);

            ew.WriteInt32(this.groups.Count);
            foreach (var group in this.groups) {
              group.Write(ew);
            }

            foreach (var i in this.unknown2) {
              ew.WriteInt32(i);
            }
            ew.Align(0x20);
          });
      actualLengthTask.ContinueWith(
          length =>
              beforeLengthTask.SetResult(length.Result));
    }
  }
}