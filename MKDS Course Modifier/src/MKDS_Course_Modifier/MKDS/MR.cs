// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MKDS.MR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.UI.MKDS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.MKDS
{
  public class MR
  {
    public const string Signature = "NKMR";
    public MR.MRHeader Header;
    public List<MR.MREntry> Entries;

    public MR(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new MR.MRHeader(er, "NKMR", out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Entries = new List<MR.MREntry>();
        for (int index = 0; (long) index < (long) this.Header.NrEntry; ++index)
          this.Entries.Add(new MR.MREntry(er));
      }
      er.Close();
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      foreach (MR.MREntry entry in this.Entries)
        entry.Write(er);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class MRHeader
    {
      public string Type;
      public uint NrEntry;

      public MRHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.NrEntry = er.ReadUInt32();
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Type, Encoding.ASCII, false);
        er.Write(this.NrEntry);
      }
    }

    public class MREntry
    {
      public TimeSpan TimeLimit;
      public TimeSpan RankTime;
      public ushort RankTolerance;
      public byte MissionID;
      public MR.MREntry.MissionTasks MissionTask;
      public MKDS_Const.Tracks Track;
      public byte NrLaps;
      public MKDS_Const.Characters Character;
      public MKDS_Const.Karts Kart;
      public byte MissionRunMenuID;
      public ushort Unknown1;
      public byte TargetValue;
      public uint Unknown2;
      public uint ObjectID;
      public uint Unknown3;
      public ushort Condition;
      public short Unknown4;
      public string NameOfMission;

      public MREntry()
      {
        this.Unknown4 = (short) -1;
      }

      public MREntry(EndianBinaryReader er)
      {
        this.TimeLimit = TimeSpan.FromMilliseconds((double) ((int) er.ReadInt16() * 1000));
        this.RankTime = TimeSpan.FromMilliseconds((double) ((int) er.ReadInt16() * 1000));
        this.RankTolerance = er.ReadUInt16();
        this.MissionID = er.ReadByte();
        this.MissionTask = (MR.MREntry.MissionTasks) er.ReadByte();
        this.Track = (MKDS_Const.Tracks) er.ReadByte();
        this.NrLaps = er.ReadByte();
        this.Character = (MKDS_Const.Characters) er.ReadByte();
        this.Kart = (MKDS_Const.Karts) er.ReadByte();
        this.MissionRunMenuID = er.ReadByte();
        this.Unknown1 = er.ReadUInt16();
        this.TargetValue = er.ReadByte();
        this.Unknown2 = er.ReadUInt32();
        this.ObjectID = er.ReadUInt32();
        this.Unknown3 = er.ReadUInt32();
        this.Condition = er.ReadUInt16();
        this.Unknown4 = er.ReadInt16();
        this.NameOfMission = er.ReadString(Encoding.ASCII, 12).Replace("\0", "");
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write((short) (this.TimeLimit.TotalMilliseconds / 1000.0));
        er.Write((short) (this.RankTime.TotalMilliseconds / 1000.0));
        er.Write(this.RankTolerance);
        er.Write(this.MissionID);
        er.Write((byte) this.MissionTask);
        er.Write((byte) this.Track);
        er.Write(this.NrLaps);
        er.Write((byte) this.Character);
        er.Write((byte) this.Kart);
        er.Write(this.MissionRunMenuID);
        er.Write(this.Unknown1);
        er.Write(this.TargetValue);
        er.Write(this.Unknown2);
        er.Write(this.ObjectID);
        er.Write(this.Unknown3);
        er.Write(this.Condition);
        er.Write(this.Unknown4);
        er.Write(this.NameOfMission.PadRight(12, char.MinValue), Encoding.ASCII, false);
      }

      public override string ToString()
      {
        return this.NameOfMission;
      }

      public static implicit operator MRProperties.MREntry(MR.MREntry e)
      {
        return new MRProperties.MREntry()
        {
          Character = e.Character,
          Condition = e.Condition,
          Kart = e.Kart,
          MissionID = e.MissionID,
          MissionRunMenuID = e.MissionRunMenuID,
          MissionTask = e.MissionTask,
          NameOfMission = e.NameOfMission,
          NrLaps = e.NrLaps,
          ObjectID = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.ObjectID)).Reverse<byte>().ToArray<byte>(), 0),
          RankTime = e.RankTime,
          RankTolerance = e.RankTolerance,
          TargetValue = e.TargetValue,
          TimeLimit = e.TimeLimit,
          Track = e.Track,
          Unknown1 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(e.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Unknown2)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown3 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Unknown3)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown4 = e.Unknown4
        };
      }

      public static implicit operator MR.MREntry(MRProperties.MREntry e)
      {
        return new MR.MREntry()
        {
          Character = e.Character,
          Condition = e.Condition,
          Kart = e.Kart,
          MissionID = e.MissionID,
          MissionRunMenuID = e.MissionRunMenuID,
          MissionTask = e.MissionTask,
          NameOfMission = e.NameOfMission,
          NrLaps = e.NrLaps,
          ObjectID = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.ObjectID)).Reverse<byte>().ToArray<byte>(), 0),
          RankTime = e.RankTime,
          RankTolerance = e.RankTolerance,
          TargetValue = e.TargetValue,
          TimeLimit = e.TimeLimit,
          Track = e.Track,
          Unknown1 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(e.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Unknown2)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown3 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Unknown3)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown4 = e.Unknown4
        };
      }

      public enum MissionTasks
      {
        Drive_Through_All_Gates,
        Collect_Coins,
        Drive_N_Laps,
        Destroy_Objects,
        Power_Sliding,
        None,
        Reach_the_Finish_Before,
        Boss_Fight,
      }
    }
  }
}
