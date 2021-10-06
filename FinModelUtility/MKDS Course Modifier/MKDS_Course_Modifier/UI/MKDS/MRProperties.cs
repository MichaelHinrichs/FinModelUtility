// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.MRProperties
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.MKDS;
using System;
using System.ComponentModel;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class MRProperties
  {
    public class MREntry
    {
      private string name;

      [MLCategory("mr.time")]
      [MLDisplayName("mr.timelimit")]
      public TimeSpan TimeLimit { get; set; }

      [MLDisplayName("mr.ranktime")]
      [MLCategory("mr.time")]
      public TimeSpan RankTime { get; set; }

      [MLDisplayName("mr.ranktolerance")]
      [MLCategory("mr.time")]
      public ushort RankTolerance { get; set; }

      [MLDisplayName("mr.missionid")]
      [MLCategory("mr.mission")]
      public byte MissionID { get; set; }

      [MLDisplayName("mr.missiontask")]
      [MLCategory("mr.missiongoal")]
      public MKDS_Course_Modifier.MKDS.MR.MREntry.MissionTasks MissionTask { get; set; }

      [MLDisplayName("mr.track")]
      [MLCategory("mr.mission")]
      public MKDS_Const.Tracks Track { get; set; }

      [MLDisplayName("mr.nrlaps")]
      [MLCategory("mr.missiongoal")]
      public byte NrLaps { get; set; }

      [MLDisplayName("mr.character")]
      [MLCategory("mr.mission")]
      public MKDS_Const.Characters Character { get; set; }

      [MLCategory("mr.mission")]
      [MLDisplayName("mr.kart")]
      public MKDS_Const.Karts Kart { get; set; }

      [MLDisplayName("mr.missionmenuid")]
      [MLCategory("mr.mission")]
      public byte MissionRunMenuID { get; set; }

      [MLCategory("nkm.unknown")]
      [TypeConverter(typeof (UInt16HexTypeConverter))]
      [MLDisplayName("nkm.unknown1")]
      public ushort Unknown1 { get; set; }

      [MLDisplayName("mr.targetvalue")]
      [MLCategory("mr.missiongoal")]
      public byte TargetValue { get; set; }

      [MLDisplayName("nkm.unknown2")]
      [MLCategory("nkm.unknown")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint Unknown2 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("mr.missiongoal")]
      [MLDisplayName("nkm.obji.objectid")]
      public uint ObjectID { get; set; }

      [MLCategory("nkm.unknown")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLDisplayName("nkm.unknown3")]
      public uint Unknown3 { get; set; }

      [MLDisplayName("mr.condition")]
      [MLCategory("mr.missiongoal")]
      public ushort Condition { get; set; }

      [MLDisplayName("nkm.unknown4")]
      [MLCategory("nkm.unknown")]
      public short Unknown4 { get; set; }

      [MLCategory("mr.mission")]
      [MLDisplayName("mr.name")]
      public string NameOfMission
      {
        get
        {
          return this.name;
        }
        set
        {
          this.name = value.Length > 12 ? value.Remove(12) : value;
        }
      }
    }
  }
}
