// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MKDS.MKDS_Const
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MKDS_Course_Modifier.MKDS
{
  public class MKDS_Const
  {
    public static MKDS_Const.Table[] Tables = new MKDS_Const.Table[2]
    {
      new MKDS_Const.Table("Global Map Scale Table", 35030436U, 55U, new MKDS_Const.Table.Field[4]
      {
        new MKDS_Const.Table.Field("MinX", typeof (short)),
        new MKDS_Const.Table.Field("MinY", typeof (short)),
        new MKDS_Const.Table.Field("MaxX", typeof (short)),
        new MKDS_Const.Table.Field("MaxY", typeof (short))
      }),
      new MKDS_Const.Table("Local Map Scale Table", 35030876U, 55U, new MKDS_Const.Table.Field[4]
      {
        new MKDS_Const.Table.Field("MinX", typeof (short)),
        new MKDS_Const.Table.Field("MinY", typeof (short)),
        new MKDS_Const.Table.Field("MaxX", typeof (short)),
        new MKDS_Const.Table.Field("MaxY", typeof (short))
      })
    };
    private static uint[] DecryptionTable1 = new uint[29]
    {
      3069360652U,
      445169399U,
      232531819U,
      2952853520U,
      1831160373U,
      1281972012U,
      239718338U,
      2755206548U,
      2564921825U,
      284721993U,
      2815222656U,
      4096293905U,
      2474998151U,
      4059109151U,
      881308154U,
      1706592039U,
      2708544156U,
      590918142U,
      4148344031U,
      2737976812U,
      1397805827U,
      3384917589U,
      3304926749U,
      2151277019U,
      3057151857U,
      885765001U,
      304349210U,
      1506667025U,
      1399976504U
    };
    private static uint[] DecryptionTable2 = new uint[19]
    {
      2213799045U,
      528827665U,
      872081531U,
      2794756638U,
      3676152773U,
      3583535663U,
      2943141474U,
      4211391759U,
      1463758980U,
      3753486159U,
      4127851958U,
      3747080135U,
      292697689U,
      2393806937U,
      3767449188U,
      3541073962U,
      1233858469U,
      1431589617U,
      3847707686U
    };
    private static uint[] DecryptionTable3 = new uint[11]
    {
      3231895481U,
      4238669210U,
      3632145800U,
      3054886192U,
      3939275246U,
      3064527197U,
      2077798528U,
      3468760405U,
      1506736334U,
      4294479283U,
      2825384611U
    };
    private static uint[] DecryptionTable4 = new uint[17]
    {
      1424514722U,
      3732727382U,
      3528614324U,
      1075650244U,
      571337954U,
      520943348U,
      1903604390U,
      1490420460U,
      2241682249U,
      3844442892U,
      3004836112U,
      769524913U,
      4008681521U,
      1535452304U,
      1152215397U,
      2385643173U,
      1579333736U
    };
    public static ObjectDb ObjectDatabase;

    public static byte[] DecryptSave(byte[] Data)
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange((IEnumerable<byte>) ((IEnumerable<byte>) Data).ToList<byte>().GetRange(0, 8).ToArray());
      int StartOffset1 = 8;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset1, 248, 1498630990U));
      int StartOffset2 = StartOffset1 + 248;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset2, 768, 1296386894U));
      int StartOffset3 = StartOffset2 + 768;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset3, 1024, 1346849614U));
      int StartOffset4 = StartOffset3 + 1024;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset4, 5632, 1096043342U));
      int StartOffset5 = StartOffset4 + 5632;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset5, 256, 1380797262U));
      int StartOffset6 = StartOffset5 + 256;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset6, 3072, 1279675214U));
      int StartOffset7 = StartOffset6 + 3072;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset7, 2816, 1162234702U));
      int StartOffset8 = StartOffset7 + 2816;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.DecryptSaveBlock(Data, StartOffset8, 2816, 1162234702U));
      int num = StartOffset8 + 2816;
      return byteList.ToArray();
    }

    public static byte[] EncryptSave(byte[] Data)
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange((IEnumerable<byte>) ((IEnumerable<byte>) Data).ToList<byte>().GetRange(0, 8).ToArray());
      int StartOffset1 = 8;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.EncryptSaveBlock(Data, StartOffset1, 248));
      int StartOffset2 = StartOffset1 + 248;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.EncryptSaveBlock(Data, StartOffset2, 768));
      int StartOffset3 = StartOffset2 + 768;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.EncryptSaveBlock(Data, StartOffset3, 1024));
      int StartOffset4 = StartOffset3 + 1024;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.EncryptSaveBlock(Data, StartOffset4, 5632));
      int StartOffset5 = StartOffset4 + 5632;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.EncryptSaveBlock(Data, StartOffset5, 256));
      int StartOffset6 = StartOffset5 + 256;
      byteList.AddRange((IEnumerable<byte>) MKDS_Const.EncryptSaveBlock(Data, StartOffset6, 3072));
      int num = StartOffset6 + 3072;
      return byteList.ToArray();
    }

    private static byte[] DecryptSaveBlock(
      byte[] Data,
      int StartOffset,
      int Length,
      uint Signature)
    {
      return MKDS_Const.DecryptSaveBlock(Data, StartOffset, Length, MKDS_Const.GetInitValue(Bytes.Read4BytesAsUInt32(Data, StartOffset), Signature));
    }

    private static byte[] DecryptSaveBlock(
      byte[] Data,
      int StartOffset,
      int Length,
      int Table1Index)
    {
      List<byte> byteList = new List<byte>();
      int index1 = Table1Index;
      int index2 = 0;
      int index3 = 0;
      int index4 = 0;
      int offset = StartOffset;
      do
      {
        uint num1 = Bytes.Read4BytesAsUInt32(Data, offset);
        offset += 4;
        uint num2 = MKDS_Const.DecryptionTable1[index1] + MKDS_Const.DecryptionTable2[index2] + MKDS_Const.DecryptionTable3[index3] + MKDS_Const.DecryptionTable4[index4];
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num1 ^ num2));
        ++index3;
        if (index3 >= 11)
        {
          index3 = 0;
          ++index4;
          if (index4 >= 17)
          {
            index4 = 0;
            ++index2;
            if (index2 >= 19)
            {
              index2 = 0;
              ++index1;
              if (index1 >= 29)
                index1 = 0;
            }
          }
        }
      }
      while (offset < StartOffset + Length);
      return byteList.ToArray();
    }

    private static byte[] EncryptSaveBlock(byte[] Data, int StartOffset, int Length)
    {
      List<byte> byteList = new List<byte>();
      int index1 = new Random().Next(0, MKDS_Const.DecryptionTable1.Length - 1);
      int index2 = 0;
      int index3 = 0;
      int index4 = 0;
      int offset = StartOffset;
      do
      {
        uint num1 = Bytes.Read4BytesAsUInt32(Data, offset);
        offset += 4;
        uint num2 = MKDS_Const.DecryptionTable1[index1] + MKDS_Const.DecryptionTable2[index2] + MKDS_Const.DecryptionTable3[index3] + MKDS_Const.DecryptionTable4[index4];
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num1 ^ num2));
        ++index3;
        if (index3 >= 11)
        {
          index3 = 0;
          ++index4;
          if (index4 >= 17)
          {
            index4 = index3;
            ++index2;
            if (index2 >= 19)
            {
              index2 = index3;
              ++index1;
              if (index1 >= 29)
                index1 = index3;
            }
          }
        }
      }
      while (offset < StartOffset + Length);
      return byteList.ToArray();
    }

    private static int GetInitValue(uint In, uint Out)
    {
      int index = 0;
      uint num1 = MKDS_Const.DecryptionTable2[0] + MKDS_Const.DecryptionTable3[0] + MKDS_Const.DecryptionTable4[0];
      do
      {
        uint num2 = MKDS_Const.DecryptionTable1[index];
        if (((int) In ^ (int) num1 + (int) num2) != (int) Out)
          ++index;
        else
          break;
      }
      while (index < 29);
      return index;
    }

    public enum Tracks
    {
      Freeze,
      GCN_Yoshi_Circuit,
      GCN_Mario_Circuit,
      luigi_course,
      dokan_course,
      test1_course,
      donkey_course,
      wario_course,
      nokonoko_course,
      GCN_Baby_Park,
      SNES_Mario_Circuit_1,
      N64_Moo_Moo_Farm,
      GBA_Bowser_Castle_2,
      GBA_Peach_Circuit,
      GCN_Luigi_Circuit,
      SNES_Koopa_Beach_2,
      N64_Frappe_Snowland,
      Tick_Tock_Clock,
      Luigis_Mansion,
      Airship_Fortress,
      Figure_8_Circuit,
      test_circle,
      Yoshi_Falls,
      N64_Banshee_Boardwalk,
      Shroom_Ridge,
      Mario_Circuit,
      Peach_Gardens,
      Desert_Hills,
      Delfino_Square,
      Rainbow_Road,
      DK_Pass,
      Cheep_Cheep_Beach,
      Bowser_Castle,
      Waluigi_Pinball,
      Wario_Stadium,
      SNES_Donut_Plains_1,
      N64_Choco_Mountain,
      GBA_Luigi_Circuit,
      GCN_Mushroom_Bridge,
      SNES_Choco_Island_2,
      GBA_Sky_Garden,
      mini_block_course,
      Block_Fort,
      Pipe_Plaza,
      Nintendo_DS,
      Lighthouse,
      Palm_Shore,
      Shortcake,
      MRStage1,
      MRStage2,
      MRStage3,
      MRStage4,
      Award,
      StaffRoll,
      StaffRollTrue,
      CustomTrack1,
      CustomTrack2,
      CustomTrack3,
      CustomTrack4,
      CustomTrack5,
      CustomTrack6,
      CustomTrack7,
      CustomTrack8,
      CustomTrack9,
      CustomTrack10,
      CustomTrack11,
      CustomTrack12,
      CustomTrack13,
      CustomTrack14,
      CustomTrack15,
      CustomTrack16,
      CustomTrack17,
      CustomTrack18,
      CustomTrack19,
      CustomTrack20,
      CustomTrack21,
      CustomTrack22,
      CustomTrack23,
      CustomTrack24,
      CustomTrack25,
      CustomTrack26,
      CustomTrack27,
      CustomTrack28,
      CustomTrack29,
      CustomTrack30,
      CustomTrack31,
      CustomTrack32,
    }

    public enum Characters
    {
      Mario,
      Donkey_Kong,
      Toad,
      Bowser,
      Peach,
      Wario,
      Yoshi,
      Luigi,
      Dry_Bones,
      Daisy,
      Waluigi,
      ROB,
      Shy_Guy,
    }

    public enum Karts
    {
      Standard_MR,
      Shooting_Star,
      B_Dasher,
      Standard_Dk,
      Whild_Life,
      Rambi_Rider,
      Standard_TD,
      Mushmellow,
      Four_Wheel_Cradle,
      Standard_BW,
      Hurricane,
      Tyrant,
      Standard_PC,
      Light_Tripper,
      Royale,
      Standard_WR,
      Brute,
      Dragonfly,
      Standard_YS,
      Egg_1,
      Cucumber,
      Standard_LG,
      Poltergust_4000,
      Streamliner,
      Standard_DB,
      Dry_Bomber,
      Banisher,
      Standard_DS,
      Light_Dancer,
      Power_Flower,
      Standard_WL,
      Gold_Mantis,
      Zipper,
      Standard_RB,
      ROB_BLS,
      ROB_LGS,
      Standard_HH,
    }

    public class Table
    {
      public Table(
        string Name,
        uint Offset,
        uint NrEntries,
        params MKDS_Const.Table.Field[] Fields)
      {
        this.Name = Name;
        this.Offset = Offset;
        this.NrEntries = NrEntries;
        this.Fields = Fields;
      }

      public MKDS_Const.Table.Field[] Fields { get; private set; }

      public string Name { get; private set; }

      public uint NrEntries { get; private set; }

      public uint Offset { get; private set; }

      public override string ToString()
      {
        return this.Name;
      }

      public class Field
      {
        public Field(string Name, Type DataType)
        {
          this.Name = Name;
          this.DataType = DataType;
        }

        public Type DataType { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
          return this.Name;
        }
      }
    }
  }
}
