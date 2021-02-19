// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.N64.MK64.MK64
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Exceptions;
using System;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.N64.MK64
{
  public class MK64
  {
    private const uint MK64LevelHeaderLoc = 1188752;
    private const uint MK64SkyColourLoc = 1188064;
    private const uint MK64TextureBaseAddr = 6561648;

    public MK64(byte[] Data)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(Data), Endianness.BigEndian);
      int num = 0;
      er.BaseStream.Position = 1188752L + (long) (48 * num);
      MKDS_Course_Modifier.N64.MK64.MK64.TrackHeader trackHeader = new MKDS_Course_Modifier.N64.MK64.MK64.TrackHeader(er);
      er.Close();
    }

    public class TrackHeader
    {
      public uint PolygonDataOffset;
      public uint PolygonDataEndOffset;
      public uint TrackDataOffset;
      public uint TrackDataEndOffset;
      public uint UnknownOffset1;
      public uint UnknownOffset2;
      public uint Unknown1;
      public uint Unknown2;
      public uint LevelScriptOffset;
      public uint Unknown3;
      public uint Unknown4;
      public uint Unknown5;

      public TrackHeader(EndianBinaryReader er)
      {
        this.PolygonDataOffset = er.ReadUInt32();
        this.PolygonDataEndOffset = er.ReadUInt32();
        this.TrackDataOffset = er.ReadUInt32();
        this.TrackDataEndOffset = er.ReadUInt32();
        this.UnknownOffset1 = er.ReadUInt32();
        this.UnknownOffset2 = er.ReadUInt32();
        this.Unknown1 = er.ReadUInt32();
        this.Unknown2 = er.ReadUInt32();
        this.LevelScriptOffset = er.ReadUInt32() & 16777215U;
        this.Unknown3 = er.ReadUInt32();
        this.Unknown4 = er.ReadUInt32();
        this.Unknown5 = er.ReadUInt32();
      }
    }

    public class MIO0
    {
      public string Signature;
      public uint UncompressedSize;
      public uint CompressedOffset;
      public uint RawOffset;

      private MIO0(byte[] SourceData, long Offset)
      {
        EndianBinaryReader endianBinaryReader = new EndianBinaryReader((Stream) new MemoryStream(SourceData), Endianness.BigEndian);
        endianBinaryReader.BaseStream.Position = Offset;
        this.Signature = endianBinaryReader.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (MIO0))
          throw new SignatureNotCorrectException(this.Signature, nameof (MIO0), endianBinaryReader.BaseStream.Position - 4L);
        this.UncompressedSize = endianBinaryReader.ReadUInt32();
        this.CompressedOffset = endianBinaryReader.ReadUInt32();
        this.RawOffset = endianBinaryReader.ReadUInt32();
        endianBinaryReader.Close();
      }

      public static byte[] Decompress(byte[] Input, uint Offset)
      {
        MKDS_Course_Modifier.N64.MK64.MK64.MIO0 miO0 = new MKDS_Course_Modifier.N64.MK64.MK64.MIO0(Input, (long) Offset);
        byte num1 = 128;
        uint num2 = 0;
        uint num3 = 16U + Offset;
        uint num4 = miO0.CompressedOffset + Offset;
        uint num5 = miO0.RawOffset + Offset;
        uint num6 = 0;
        uint uncompressedSize = miO0.UncompressedSize;
        byte[] numArray = new byte[(IntPtr) miO0.UncompressedSize];
        byte num7 = Input[(IntPtr) num3];
        while (num2 < uncompressedSize)
        {
          if (((int) num7 & (int) num1) != 0)
          {
            numArray[(IntPtr) num6] = Input[(IntPtr) num5];
            ++num6;
            ++num5;
            ++num2;
          }
          else
          {
            uint num8 = (uint) Input[(IntPtr) num4] << 8 | (uint) Input[(IntPtr) (num4 + 1U)];
            uint num9 = (num8 >> 12) + 3U;
            uint num10 = (uint) (((int) num8 & 4095) + 1);
            if ((int) num6 - (int) num10 < 0)
              return (byte[]) null;
            for (int index = 0; (long) index < (long) num9; ++index)
            {
              numArray[(IntPtr) num6] = numArray[(IntPtr) (num6 - num10)];
              ++num6;
              ++num2;
              if (num2 >= uncompressedSize)
                break;
            }
            num4 += 2U;
          }
          num1 >>= 1;
          if (num1 == (byte) 0)
          {
            num1 = (byte) 128;
            ++num3;
            num7 = Input[(IntPtr) num3];
            uint num8 = num4;
            if (num5 < num4)
              num8 = num5;
            if (num3 > num8)
              return (byte[]) null;
          }
        }
        return numArray;
      }
    }
  }
}
