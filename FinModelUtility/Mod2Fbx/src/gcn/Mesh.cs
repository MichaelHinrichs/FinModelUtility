using System;
using System.Collections.Generic;
using System.IO;

using fin.math;

using schema;

namespace mod.gcn {
  // THANKS:
  // https://github.com/KillzXGaming/010-Templates/blob/816cfc57e2ee998b953cf488e4fed25c54e7861a/Pikmin/MOD.bt#L312
  public class DisplayListFlagsByteView {
    public byte b1;
    public byte b2;
    public byte b3;
    public byte cullMode;
  }

  public class DisplayListFlags {
    public DisplayListFlagsByteView byteView = new();

    public uint intView {
      get => BitLogic.ToUint32(this.byteView.b1,
                               this.byteView.b2,
                               this.byteView.b3,
                               this.byteView.cullMode);
      set => (this.byteView.b1,
              this.byteView.b2,
              this.byteView.b3,
              this.byteView.cullMode) = BitLogic.FromUint32(value);
    }
  }

  public class DisplayList : IBiSerializable {
    public DisplayListFlags flags = new();

    // THANKS: Yoshi2's mod2obj
    public uint cmdCount = 0;
    public readonly List<byte> dlistData = new();

    public void Read(EndianBinaryReader reader) {
      this.flags.intView = reader.ReadUInt32();
      this.cmdCount = reader.ReadUInt32();

      var numDlists = reader.ReadUInt32();
      reader.Align(0x20);
      for (var i = 0; i < numDlists; ++i) {
        this.dlistData.Add(reader.ReadByte());
      }
    }

    public void Write(EndianBinaryWriter writer) {
      writer.WriteUInt32(this.flags.intView);
      writer.WriteUInt32(this.cmdCount);

      writer.WriteInt32(this.dlistData.Count);
      writer.Align(0x20);
      foreach (var b in this.dlistData) {
        writer.WriteByte(b);
      }
    }
  }

  [Schema]
  public partial class MeshPacket : IBiSerializable {
    [ArrayLengthSource(IntType.UINT32)]
    public short[] indices;
    [ArrayLengthSource(IntType.UINT32)]
    public DisplayList[] displaylists;
  }

  [Schema]
  public partial class Mesh : IBiSerializable {
    public uint boneIndex = 0;
    public uint vtxDescriptor = 0;

    [ArrayLengthSource(IntType.UINT32)]
    public MeshPacket[] packets;
  }
}