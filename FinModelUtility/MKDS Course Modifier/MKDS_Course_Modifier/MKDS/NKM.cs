// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MKDS.NKM
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Flobbster.Windows.Forms;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.UI.MKDS;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.MKDS
{
  public class NKM
  {
    public const string Signature = "NKMD";
    public NKM.NKMHeader Header;
    public NKM.NKMHeader<NKM.OBJIEntry> OBJI;
    public NKM.NKMHeader<NKM.PATHEntry> PATH;
    public NKM.NKMHeader<NKM.POITEntry> POIT;
    public NKM.STAGEntry STAG;
    public NKM.NKMHeader<NKM.KTPSEntry> KTPS;
    public NKM.NKMHeader<NKM.KTPJEntry> KTPJ;
    public NKM.NKMHeader<NKM.KTP2Entry> KTP2;
    public NKM.NKMHeader<NKM.KTPCEntry> KTPC;
    public NKM.NKMHeader<NKM.KTPMEntry> KTPM;
    public NKM.NKMHeader<NKM.CPOIEntry> CPOI;
    public NKM.NKMHeader<NKM.CPATEntry> CPAT;
    public NKM.NKMHeader<NKM.IPOIEntry> IPOI;
    public NKM.NKMHeader<NKM.IPATEntry> IPAT;
    public NKM.NKMHeader<NKM.EPOIEntry> EPOI;
    public NKM.NKMHeader<NKM.EPATEntry> EPAT;
    public NKM.NKMHeader<NKM.MEPOEntry> MEPO;
    public NKM.NKMHeader<NKM.MEPAEntry> MEPA;
    public NKM.NKMHeader<NKM.AREAEntry> AREA;
    public NKM.NKMHeader<NKM.CAMEEntry> CAME;
    public NKM.NKMIEntry NKMI;

    public NKM(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new NKM.NKMHeader(er, "NKMD", out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
      {
        for (int index = 0; index < ((int) this.Header.HeaderLength - 8) / 4; ++index)
        {
          er.BaseStream.Position = (long) (this.Header[index] + (uint) this.Header.HeaderLength);
          string Signature;
          switch (Signature = er.ReadString(Encoding.ASCII, 4))
          {
            case nameof (OBJI):
              this.OBJI = new NKM.NKMHeader<NKM.OBJIEntry>(er, Signature);
              break;
            case nameof (PATH):
              this.PATH = new NKM.NKMHeader<NKM.PATHEntry>(er, Signature);
              break;
            case nameof (POIT):
              this.POIT = new NKM.NKMHeader<NKM.POITEntry>(er, Signature);
              break;
            case nameof (STAG):
              this.STAG = new NKM.STAGEntry(er, Signature);
              break;
            case nameof (KTPS):
              this.KTPS = new NKM.NKMHeader<NKM.KTPSEntry>(er, Signature);
              break;
            case nameof (KTPJ):
              this.KTPJ = new NKM.NKMHeader<NKM.KTPJEntry>(er, Signature, ((int) this.Header[index + 1] - (int) this.Header[index] - 8) / 32);
              break;
            case nameof (KTP2):
              this.KTP2 = new NKM.NKMHeader<NKM.KTP2Entry>(er, Signature);
              break;
            case nameof (KTPC):
              this.KTPC = new NKM.NKMHeader<NKM.KTPCEntry>(er, Signature);
              break;
            case nameof (KTPM):
              this.KTPM = new NKM.NKMHeader<NKM.KTPMEntry>(er, Signature);
              break;
            case nameof (CPOI):
              this.CPOI = new NKM.NKMHeader<NKM.CPOIEntry>(er, Signature);
              break;
            case nameof (CPAT):
              this.CPAT = new NKM.NKMHeader<NKM.CPATEntry>(er, Signature);
              break;
            case nameof (IPOI):
              this.IPOI = new NKM.NKMHeader<NKM.IPOIEntry>(er, Signature, ((int) this.Header[index + 1] - (int) this.Header[index] - 8) / 20);
              break;
            case nameof (IPAT):
              this.IPAT = new NKM.NKMHeader<NKM.IPATEntry>(er, Signature);
              break;
            case nameof (EPOI):
              this.EPOI = new NKM.NKMHeader<NKM.EPOIEntry>(er, Signature);
              break;
            case nameof (EPAT):
              this.EPAT = new NKM.NKMHeader<NKM.EPATEntry>(er, Signature);
              break;
            case nameof (MEPO):
              this.MEPO = new NKM.NKMHeader<NKM.MEPOEntry>(er, Signature);
              break;
            case nameof (MEPA):
              this.MEPA = new NKM.NKMHeader<NKM.MEPAEntry>(er, Signature);
              break;
            case nameof (AREA):
              this.AREA = new NKM.NKMHeader<NKM.AREAEntry>(er, Signature);
              break;
            case nameof (CAME):
              this.CAME = new NKM.NKMHeader<NKM.CAMEEntry>(er, Signature);
              break;
          }
        }
        if (er.BaseStream.Position + 4L != er.BaseStream.Length && er.ReadString(Encoding.ASCII, 4) == nameof (NKMI))
          this.NKMI = new NKM.NKMIEntry(er, nameof (NKMI));
        if (this.KTPC == null)
          this.KTPC = new NKM.NKMHeader<NKM.KTPCEntry>(nameof (KTPC));
        if (this.KTPM == null)
          this.KTPM = new NKM.NKMHeader<NKM.KTPMEntry>(nameof (KTPM));
        if (this.NKMI == null)
          this.NKMI = new NKM.NKMIEntry();
      }
      er.Close();
    }

    public NKM(bool BattleTrack)
    {
      this.Header = new NKM.NKMHeader();
      this.OBJI = new NKM.NKMHeader<NKM.OBJIEntry>(nameof (OBJI));
      this.PATH = new NKM.NKMHeader<NKM.PATHEntry>(nameof (PATH));
      this.POIT = new NKM.NKMHeader<NKM.POITEntry>(nameof (POIT));
      this.STAG = new NKM.STAGEntry(BattleTrack);
      this.KTPS = new NKM.NKMHeader<NKM.KTPSEntry>(nameof (KTPS));
      this.KTPJ = new NKM.NKMHeader<NKM.KTPJEntry>(nameof (KTPJ));
      this.KTP2 = new NKM.NKMHeader<NKM.KTP2Entry>(nameof (KTP2));
      this.KTPC = new NKM.NKMHeader<NKM.KTPCEntry>(nameof (KTPC));
      this.KTPM = new NKM.NKMHeader<NKM.KTPMEntry>(nameof (KTPM));
      this.CPOI = new NKM.NKMHeader<NKM.CPOIEntry>(nameof (CPOI));
      this.CPAT = new NKM.NKMHeader<NKM.CPATEntry>(nameof (CPAT));
      this.IPOI = new NKM.NKMHeader<NKM.IPOIEntry>(nameof (IPOI));
      this.IPAT = new NKM.NKMHeader<NKM.IPATEntry>(nameof (IPAT));
      if (BattleTrack)
      {
        this.MEPO = new NKM.NKMHeader<NKM.MEPOEntry>(nameof (MEPO));
        this.MEPA = new NKM.NKMHeader<NKM.MEPAEntry>(nameof (MEPA));
      }
      else
      {
        this.EPOI = new NKM.NKMHeader<NKM.EPOIEntry>(nameof (EPOI));
        this.EPAT = new NKM.NKMHeader<NKM.EPATEntry>(nameof (EPAT));
      }
      this.AREA = new NKM.NKMHeader<NKM.AREAEntry>(nameof (AREA));
      this.CAME = new NKM.NKMHeader<NKM.CAMEEntry>(nameof (CAME));
      this.NKMI = new NKM.NKMIEntry();
    }

    public byte[] Save()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      int num1 = 8;
      long position1 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num1;
      er.Write((uint) ((ulong) position1 - 76UL));
      int num2 = num1 + 4;
      er.BaseStream.Position = position1;
      this.OBJI.Write(er);
      long position2 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num2;
      er.Write((uint) ((ulong) position2 - 76UL));
      int num3 = num2 + 4;
      er.BaseStream.Position = position2;
      this.PATH.Write(er);
      long position3 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num3;
      er.Write((uint) ((ulong) position3 - 76UL));
      int num4 = num3 + 4;
      er.BaseStream.Position = position3;
      this.POIT.Write(er);
      long position4 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num4;
      er.Write((uint) ((ulong) position4 - 76UL));
      int num5 = num4 + 4;
      er.BaseStream.Position = position4;
      this.STAG.Write(er);
      long position5 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num5;
      er.Write((uint) ((ulong) position5 - 76UL));
      int num6 = num5 + 4;
      er.BaseStream.Position = position5;
      this.KTPS.Write(er);
      long position6 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num6;
      er.Write((uint) ((ulong) position6 - 76UL));
      int num7 = num6 + 4;
      er.BaseStream.Position = position6;
      this.KTPJ.Write(er);
      long position7 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num7;
      er.Write((uint) ((ulong) position7 - 76UL));
      int num8 = num7 + 4;
      er.BaseStream.Position = position7;
      this.KTP2.Write(er);
      long position8 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num8;
      er.Write((uint) ((ulong) position8 - 76UL));
      int num9 = num8 + 4;
      er.BaseStream.Position = position8;
      this.KTPC.Write(er);
      long position9 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num9;
      er.Write((uint) ((ulong) position9 - 76UL));
      int num10 = num9 + 4;
      er.BaseStream.Position = position9;
      this.KTPM.Write(er);
      long position10 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num10;
      er.Write((uint) ((ulong) position10 - 76UL));
      int num11 = num10 + 4;
      er.BaseStream.Position = position10;
      this.CPOI.Write(er);
      long position11 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num11;
      er.Write((uint) ((ulong) position11 - 76UL));
      int num12 = num11 + 4;
      er.BaseStream.Position = position11;
      this.CPAT.Write(er);
      long position12 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num12;
      er.Write((uint) ((ulong) position12 - 76UL));
      int num13 = num12 + 4;
      er.BaseStream.Position = position12;
      this.IPOI.Write(er);
      long position13 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num13;
      er.Write((uint) ((ulong) position13 - 76UL));
      int num14 = num13 + 4;
      er.BaseStream.Position = position13;
      this.IPAT.Write(er);
      int num15;
      if (this.MEPO == null)
      {
        long position14 = er.BaseStream.Position;
        er.BaseStream.Position = (long) num14;
        er.Write((uint) ((ulong) position14 - 76UL));
        int num16 = num14 + 4;
        er.BaseStream.Position = position14;
        this.EPOI.Write(er);
        long position15 = er.BaseStream.Position;
        er.BaseStream.Position = (long) num16;
        er.Write((uint) ((ulong) position15 - 76UL));
        num15 = num16 + 4;
        er.BaseStream.Position = position15;
        this.EPAT.Write(er);
      }
      else
      {
        long position14 = er.BaseStream.Position;
        er.BaseStream.Position = (long) num14;
        er.Write((uint) ((ulong) position14 - 76UL));
        int num16 = num14 + 4;
        er.BaseStream.Position = position14;
        this.MEPO.Write(er);
        long position15 = er.BaseStream.Position;
        er.BaseStream.Position = (long) num16;
        er.Write((uint) ((ulong) position15 - 76UL));
        num15 = num16 + 4;
        er.BaseStream.Position = position15;
        this.MEPA.Write(er);
      }
      long position16 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num15;
      er.Write((uint) ((ulong) position16 - 76UL));
      int num17 = num15 + 4;
      er.BaseStream.Position = position16;
      this.AREA.Write(er);
      long position17 = er.BaseStream.Position;
      er.BaseStream.Position = (long) num17;
      er.Write((uint) ((ulong) position17 - 76UL));
      int num18 = num17 + 4;
      er.BaseStream.Position = position17;
      this.CAME.Write(er);
      this.NKMI.Write(er);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class NKMHeader
    {
      private const ushort NKMVersion = 40;
      public string Type;
      public ushort Version;
      public ushort HeaderLength;
      public uint[] SectionOffsets;

      public NKMHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.Version = er.ReadUInt16();
          this.HeaderLength = er.ReadUInt16();
          this.SectionOffsets = er.ReadUInt32s(((int) this.HeaderLength - 8) / 4);
          OK = true;
        }
      }

      public NKMHeader()
      {
        this.Type = "NKMD";
        this.Version = (ushort) 40;
        this.HeaderLength = (ushort) 76;
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Type, Encoding.ASCII, false);
        er.Write((ushort) 40);
        er.Write((ushort) 76);
        er.Write(new uint[17], 0, 17);
      }

      public uint this[int i]
      {
        get
        {
          return this.SectionOffsets[i];
        }
      }
    }

    public class NKMHeader<T> where T : NKM.NKMEntry, new()
    {
      public string Type;
      public uint NrEntries;
      public List<T> Entries;

      public NKMHeader(EndianBinaryReader er, string Signature)
      {
        this.Type = Signature;
        this.NrEntries = er.ReadUInt32();
        this.Entries = new List<T>();
        for (int index = 0; (long) index < (long) this.NrEntries; ++index)
        {
          this.Entries.Add(new T());
          this.Entries[index].Read(er);
        }
      }

      public NKMHeader(EndianBinaryReader er, string Signature, int CompensateCheck)
      {
        this.Type = Signature;
        this.NrEntries = er.ReadUInt32();
        this.Entries = new List<T>();
        bool Compensate = (long) CompensateCheck != (long) this.NrEntries;
        for (int Idx = 0; (long) Idx < (long) this.NrEntries; ++Idx)
        {
          this.Entries.Add(new T());
          this.Entries[Idx].Read(er, Compensate, Idx);
        }
      }

      public NKMHeader(string Signature)
      {
        this.Type = Signature;
        this.NrEntries = 0U;
        this.Entries = new List<T>();
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Type, Encoding.ASCII, false);
        er.Write((uint) this.Entries.Count);
        for (int index = 0; index < this.Entries.Count; ++index)
          this.Entries[index].Write(er);
      }

      public T this[int i]
      {
        get
        {
          return this.Entries[i];
        }
        set
        {
          this.Entries[i] = value;
        }
      }

      public void Add(T Obj)
      {
        this.Entries.Add(Obj);
      }

      public void Insert(int Idx, T Obj)
      {
        this.Entries.Insert(Idx, Obj);
      }

      public void Remove(T Obj)
      {
        this.Entries.Remove(Obj);
      }

      public void RemoveAt(int Idx)
      {
        this.Entries.RemoveAt(Idx);
      }

      public IEnumerator<T> GetEnumerator()
      {
        return (IEnumerator<T>) this.Entries.GetEnumerator();
      }
    }

    public abstract class NKMEntry
    {
      public virtual void Read(EndianBinaryReader er)
      {
      }

      public virtual void Read(EndianBinaryReader er, bool Compensate, int Idx)
      {
      }

      public virtual void Write(EndianBinaryWriter er)
      {
      }

      public virtual ListViewItem ToListViewItem()
      {
        return (ListViewItem) null;
      }
    }

    public class OBJIEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public Vector3 Scale;
      public ushort ObjectID;
      public short RouteID;
      public uint Setting1;
      public uint Setting2;
      public uint Setting3;
      public uint Setting4;
      public bool TimeTrails;

      public OBJIEntry()
      {
        this.Scale = new Vector3(1f, 1f, 1f);
        this.TimeTrails = true;
        this.RouteID = (short) -1;
      }

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Scale = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.ObjectID = er.ReadUInt16();
        this.RouteID = er.ReadInt16();
        this.Setting1 = er.ReadUInt32();
        this.Setting2 = er.ReadUInt32();
        this.Setting3 = er.ReadUInt32();
        this.Setting4 = er.ReadUInt32();
        this.TimeTrails = er.ReadUInt32() == 1U;
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Scale.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Scale.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Scale.Z), 0, 4);
        er.Write(this.ObjectID);
        er.Write(this.RouteID);
        er.Write(this.Setting1);
        er.Write(this.Setting2);
        er.Write(this.Setting3);
        er.Write(this.Setting4);
        er.Write(this.TimeTrails ? 1 : 0);
      }

      public static implicit operator NKMProperties.OBJI(NKM.OBJIEntry e)
      {
        NKMProperties.OBJI Bag = new NKMProperties.OBJI();
        Bag.Tx = e.Position.X;
        Bag.Ty = e.Position.Y;
        Bag.Tz = e.Position.Z;
        Bag.Rx = e.Rotation.X;
        Bag.Ry = e.Rotation.Y;
        Bag.Rz = e.Rotation.Z;
        Bag.Sx = e.Scale.X;
        Bag.Sy = e.Scale.Y;
        Bag.Sz = e.Scale.Z;
        Bag.OldObjectID = e.ObjectID;
        Bag.ObjectID = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(e.ObjectID)).Reverse<byte>().ToArray<byte>(), 0);
        Bag.RouteID = e.RouteID;
        Bag.ShowTT = e.TimeTrails;
        ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(e.ObjectID);
        if (@object != null)
        {
          int PropertyIndex1 = 0;
          int TotalIndex = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting1)
          {
            NKM.OBJIEntry.SetProperties(Bag, Data, e.Setting1, TotalIndex, PropertyIndex1);
            ++TotalIndex;
            ++PropertyIndex1;
          }
          int PropertyIndex2 = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting2)
          {
            NKM.OBJIEntry.SetProperties(Bag, Data, e.Setting2, TotalIndex, PropertyIndex2);
            ++TotalIndex;
            ++PropertyIndex2;
          }
          int PropertyIndex3 = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting3)
          {
            NKM.OBJIEntry.SetProperties(Bag, Data, e.Setting3, TotalIndex, PropertyIndex3);
            ++TotalIndex;
            ++PropertyIndex3;
          }
          int PropertyIndex4 = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting4)
          {
            NKM.OBJIEntry.SetProperties(Bag, Data, e.Setting4, TotalIndex, PropertyIndex4);
            ++TotalIndex;
            ++PropertyIndex4;
          }
        }
        else
        {
          Bag.Properties.Add(new PropertySpec("Unknown1", typeof (uint), "Settings", "", (object) "", "", typeof (UInt32HexTypeConverter)));
          Bag["Unknown1"] = (object) BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Setting1)).Reverse<byte>().ToArray<byte>(), 0);
          Bag.Properties.Add(new PropertySpec("Unknown2", typeof (uint), "Settings", "", (object) "", "", typeof (UInt32HexTypeConverter)));
          Bag["Unknown2"] = (object) BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Setting2)).Reverse<byte>().ToArray<byte>(), 0);
          Bag.Properties.Add(new PropertySpec("Unknown3", typeof (uint), "Settings", "", (object) "", "", typeof (UInt32HexTypeConverter)));
          Bag["Unknown3"] = (object) BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Setting3)).Reverse<byte>().ToArray<byte>(), 0);
          Bag.Properties.Add(new PropertySpec("Unknown4", typeof (uint), "Settings", "", (object) "", "", typeof (UInt32HexTypeConverter)));
          Bag["Unknown4"] = (object) BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(e.Setting4)).Reverse<byte>().ToArray<byte>(), 0);
        }
        return Bag;
      }

      public static void SetProperties(
        NKMProperties.OBJI Bag,
        ObjectDb.Object.Setting.SettingData Data,
        uint SourceData,
        int TotalIndex,
        int PropertyIndex)
      {
        switch (Data.GetType().Name)
        {
          case "U32":
            if (Data.Hex)
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (uint), "Settings", "", (object) "", "", typeof (UInt32HexTypeConverter)));
            else
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (uint), "Settings"));
            if (Data.Hex)
              Bag[TotalIndex.ToString()] = (object) BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(SourceData)).Reverse<byte>().ToArray<byte>(), 0);
            else
              Bag[TotalIndex.ToString()] = (object) SourceData;
            Bag.Properties[TotalIndex].Attributes = new Attribute[1]
            {
              (Attribute) new DisplayNameAttribute(Data.Name)
            };
            break;
          case "S32":
            if (Data.Hex)
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (uint), "Settings", "", (object) "", "", typeof (UInt32HexTypeConverter)));
            else
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (int), "Settings"));
            if (Data.Hex)
              Bag[TotalIndex.ToString()] = (object) BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(SourceData)).Reverse<byte>().ToArray<byte>(), 0);
            else
              Bag[TotalIndex.ToString()] = (object) (int) SourceData;
            Bag.Properties[TotalIndex].Attributes = new Attribute[1]
            {
              (Attribute) new DisplayNameAttribute(Data.Name)
            };
            break;
          case "U16":
            if (Data.Hex)
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (ushort), "Settings", "", (object) "", "", typeof (UInt16HexTypeConverter)));
            else
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (ushort), "Settings"));
            if (Data.Hex)
              Bag[TotalIndex.ToString()] = (object) BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes((ushort) (SourceData >> (PropertyIndex == 0 ? 0 : 16) & (uint) ushort.MaxValue))).Reverse<byte>().ToArray<byte>(), 0);
            else
              Bag[TotalIndex.ToString()] = (object) (ushort) (SourceData >> (PropertyIndex == 0 ? 0 : 16) & (uint) ushort.MaxValue);
            Bag.Properties[TotalIndex].Attributes = new Attribute[1]
            {
              (Attribute) new DisplayNameAttribute(Data.Name)
            };
            break;
          case "S16":
            if (Data.Hex)
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (ushort), "Settings", "", (object) "", "", typeof (UInt16HexTypeConverter)));
            else
              Bag.Properties.Add(new PropertySpec(TotalIndex.ToString(), typeof (short), "Settings"));
            if (Data.Hex)
              Bag[TotalIndex.ToString()] = (object) BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes((ushort) (SourceData >> (PropertyIndex == 0 ? 0 : 16) & (uint) ushort.MaxValue))).Reverse<byte>().ToArray<byte>(), 0);
            else
              Bag[TotalIndex.ToString()] = (object) (short) ((int) (SourceData >> (PropertyIndex == 0 ? 0 : 16)) & (int) ushort.MaxValue);
            Bag.Properties[TotalIndex].Attributes = new Attribute[1]
            {
              (Attribute) new DisplayNameAttribute(Data.Name)
            };
            break;
        }
      }

      public static void GetProperties(
        NKMProperties.OBJI Bag,
        ObjectDb.Object.Setting.SettingData Data,
        ref uint DestData,
        int TotalIndex,
        int PropertyIndex)
      {
        switch (Data.GetType().Name)
        {
          case "U32":
            if (Data.Hex)
            {
              DestData = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes((uint) Bag[TotalIndex.ToString()])).Reverse<byte>().ToArray<byte>(), 0);
              break;
            }
            DestData = (uint) Bag[TotalIndex.ToString()];
            break;
          case "S32":
            if (Data.Hex)
            {
              DestData = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes((uint) Bag[TotalIndex.ToString()])).Reverse<byte>().ToArray<byte>(), 0);
              break;
            }
            DestData = (uint) (int) Bag[TotalIndex.ToString()];
            break;
          case "U16":
            if (Data.Hex)
            {
              DestData |= (uint) BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes((ushort) Bag[TotalIndex.ToString()])).Reverse<byte>().ToArray<byte>(), 0) << (PropertyIndex == 0 ? 0 : 16);
              break;
            }
            DestData |= (uint) (ushort) Bag[TotalIndex.ToString()] << (PropertyIndex == 0 ? 0 : 16);
            break;
          case "S16":
            if (Data.Hex)
            {
              DestData |= (uint) BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes((ushort) Bag[TotalIndex.ToString()])).Reverse<byte>().ToArray<byte>(), 0) << (PropertyIndex == 0 ? 0 : 16);
              break;
            }
            DestData |= (uint) (short) Bag[TotalIndex.ToString()] << (PropertyIndex == 0 ? 0 : 16);
            break;
        }
      }

      public static implicit operator NKM.OBJIEntry(NKMProperties.OBJI o)
      {
        NKM.OBJIEntry objiEntry = new NKM.OBJIEntry();
        objiEntry.Position.X = o.Tx;
        objiEntry.Position.Y = o.Ty;
        objiEntry.Position.Z = o.Tz;
        objiEntry.Rotation.X = o.Rx;
        objiEntry.Rotation.Y = o.Ry;
        objiEntry.Rotation.Z = o.Rz;
        objiEntry.Scale.X = o.Sx;
        objiEntry.Scale.Y = o.Sy;
        objiEntry.Scale.Z = o.Sz;
        objiEntry.ObjectID = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.ObjectID)).Reverse<byte>().ToArray<byte>(), 0);
        objiEntry.RouteID = o.RouteID;
        objiEntry.TimeTrails = o.ShowTT;
        ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(o.OldObjectID);
        if (@object != null)
        {
          int PropertyIndex1 = 0;
          int TotalIndex = 0;
          objiEntry.Setting1 = 0U;
          objiEntry.Setting2 = 0U;
          objiEntry.Setting3 = 0U;
          objiEntry.Setting4 = 0U;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting1)
          {
            NKM.OBJIEntry.GetProperties(o, Data, ref objiEntry.Setting1, TotalIndex, PropertyIndex1);
            ++TotalIndex;
            ++PropertyIndex1;
          }
          int PropertyIndex2 = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting2)
          {
            NKM.OBJIEntry.GetProperties(o, Data, ref objiEntry.Setting2, TotalIndex, PropertyIndex2);
            ++TotalIndex;
            ++PropertyIndex2;
          }
          int PropertyIndex3 = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting3)
          {
            NKM.OBJIEntry.GetProperties(o, Data, ref objiEntry.Setting3, TotalIndex, PropertyIndex3);
            ++TotalIndex;
            ++PropertyIndex3;
          }
          int PropertyIndex4 = 0;
          foreach (ObjectDb.Object.Setting.SettingData Data in @object.Setting4)
          {
            NKM.OBJIEntry.GetProperties(o, Data, ref objiEntry.Setting4, TotalIndex, PropertyIndex4);
            ++TotalIndex;
            ++PropertyIndex4;
          }
        }
        else
        {
          objiEntry.Setting1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes((uint) o["Unknown1"])).Reverse<byte>().ToArray<byte>(), 0);
          objiEntry.Setting2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes((uint) o["Unknown2"])).Reverse<byte>().ToArray<byte>(), 0);
          objiEntry.Setting3 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes((uint) o["Unknown3"])).Reverse<byte>().ToArray<byte>(), 0);
          objiEntry.Setting4 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes((uint) o["Unknown4"])).Reverse<byte>().ToArray<byte>(), 0);
        }
        return objiEntry;
      }

      public static implicit operator ListViewItem(NKM.OBJIEntry o)
      {
        ListViewItem listViewItem = new ListViewItem();
        listViewItem.SubItems.Add(o.Position.X.ToString());
        listViewItem.SubItems.Add(o.Position.Y.ToString());
        listViewItem.SubItems.Add(o.Position.Z.ToString());
        listViewItem.SubItems.Add(o.Rotation.X.ToString());
        listViewItem.SubItems.Add(o.Rotation.Y.ToString());
        listViewItem.SubItems.Add(o.Rotation.Z.ToString());
        listViewItem.SubItems.Add(o.Scale.X.ToString());
        listViewItem.SubItems.Add(o.Scale.Y.ToString());
        listViewItem.SubItems.Add(o.Scale.Z.ToString());
        ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(o.ObjectID);
        if (@object != null)
          listViewItem.SubItems.Add(@object.ToString());
        else
          listViewItem.SubItems.Add(BitConverter.ToString(BitConverter.GetBytes(o.ObjectID)).Replace("-", ""));
        listViewItem.SubItems.Add(o.RouteID.ToString());
        listViewItem.SubItems.Add(BitConverter.ToString(BitConverter.GetBytes(o.Setting1)).Replace("-", ""));
        listViewItem.SubItems.Add(BitConverter.ToString(BitConverter.GetBytes(o.Setting2)).Replace("-", ""));
        listViewItem.SubItems.Add(BitConverter.ToString(BitConverter.GetBytes(o.Setting3)).Replace("-", ""));
        listViewItem.SubItems.Add(BitConverter.ToString(BitConverter.GetBytes(o.Setting4)).Replace("-", ""));
        listViewItem.SubItems.Add(o.TimeTrails.ToString());
        return listViewItem;
      }
    }

    public class PATHEntry : NKM.NKMEntry
    {
      public byte Index;
      public bool Loop;
      public short NrPoit;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Index = er.ReadByte();
        this.Loop = er.ReadByte() == (byte) 1;
        this.NrPoit = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(this.Index);
        er.Write(this.Loop ? (byte) 1 : (byte) 0);
        er.Write(this.NrPoit);
      }

      public static implicit operator NKMProperties.PATH(NKM.PATHEntry o)
      {
        return new NKMProperties.PATH()
        {
          Index = o.Index,
          Loop = o.Loop,
          NrPoit = o.NrPoit
        };
      }

      public static implicit operator NKM.PATHEntry(NKMProperties.PATH o)
      {
        return new NKM.PATHEntry()
        {
          Index = o.Index,
          Loop = o.Loop,
          NrPoit = o.NrPoit
        };
      }

      public static implicit operator ListViewItem(NKM.PATHEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Index.ToString(),
            o.Loop.ToString(),
            o.NrPoit.ToString()
          }
        };
      }
    }

    public class POITEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public short Index;
      public short Duration;
      public uint Unknown;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Index = er.ReadInt16();
        this.Duration = er.ReadInt16();
        this.Unknown = er.ReadUInt32();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(this.Index);
        er.Write(this.Duration);
        er.Write(this.Unknown);
      }

      public static implicit operator NKMProperties.POIT(NKM.POITEntry o)
      {
        return new NKMProperties.POIT()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Index = o.Index,
          Duration = o.Duration,
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.POITEntry(NKMProperties.POIT o)
      {
        return new NKM.POITEntry()
        {
          Index = o.Index,
          Unknown = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Duration = o.Duration
        };
      }

      public static implicit operator ListViewItem(NKM.POITEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Index.ToString(),
            o.Duration.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown)).Replace("-", "")
          }
        };
      }
    }

    public class STAGEntry
    {
      public string Type;
      public ushort Unknown1;
      public short NrLaps;
      public byte Unknown2;
      public bool FogEnabled;
      public byte FogTableGenMode;
      public byte FogSlope;
      public byte[] UnknownData1;
      public float FogDensity;
      public Color FogColor;
      public ushort FogAlpha;
      public Color KclColor1;
      public Color KclColor2;
      public Color KclColor3;
      public Color KclColor4;
      public byte[] UnknownData2;

      public STAGEntry(EndianBinaryReader er, string Signature)
      {
        this.Type = Signature;
        this.Unknown1 = er.ReadUInt16();
        this.NrLaps = er.ReadInt16();
        this.Unknown2 = er.ReadByte();
        this.FogEnabled = er.ReadByte() == (byte) 1;
        this.FogTableGenMode = er.ReadByte();
        this.FogSlope = er.ReadByte();
        this.UnknownData1 = er.ReadBytes(8);
        this.FogDensity = er.ReadSingleInt32Exp12();
        this.FogColor = Graphic.ConvertABGR1555(er.ReadInt16());
        this.FogAlpha = er.ReadUInt16();
        this.KclColor1 = Graphic.ConvertABGR1555(er.ReadInt16());
        this.KclColor2 = Graphic.ConvertABGR1555(er.ReadInt16());
        this.KclColor3 = Graphic.ConvertABGR1555(er.ReadInt16());
        this.KclColor4 = Graphic.ConvertABGR1555(er.ReadInt16());
        this.UnknownData2 = er.ReadBytes(8);
      }

      public STAGEntry(bool BattleTrack)
      {
        this.Type = "STAG";
        this.Unknown1 = (ushort) 0;
        this.NrLaps = BattleTrack ? (short) 60 : (short) 3;
        this.Unknown2 = (byte) 0;
        this.FogEnabled = false;
        this.FogTableGenMode = (byte) 0;
        this.FogSlope = (byte) 0;
        this.UnknownData1 = new byte[8];
        this.FogDensity = 0.0f;
        this.FogColor = Color.White;
        this.FogAlpha = (ushort) 0;
        this.KclColor1 = Color.White;
        this.KclColor2 = Color.White;
        this.KclColor3 = Color.White;
        this.KclColor4 = Color.White;
        this.UnknownData2 = new byte[8];
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Type, Encoding.ASCII, false);
        er.Write(this.Unknown1);
        er.Write(this.NrLaps);
        er.Write(this.Unknown2);
        er.Write(this.FogEnabled ? (byte) 1 : (byte) 0);
        er.Write(this.FogTableGenMode);
        er.Write(this.FogSlope);
        er.Write(this.UnknownData1, 0, 8);
        er.Write((int) ((double) this.FogDensity * 4096.0));
        er.Write((short) Graphic.ToABGR1555(this.FogColor));
        er.Write(this.FogAlpha);
        er.Write((short) Graphic.ToABGR1555(this.KclColor1));
        er.Write((short) Graphic.ToABGR1555(this.KclColor2));
        er.Write((short) Graphic.ToABGR1555(this.KclColor3));
        er.Write((short) Graphic.ToABGR1555(this.KclColor4));
        er.Write(this.UnknownData2, 0, 8);
      }
    }

    public class KTPSEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public ushort Padding;
      public short Index;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Padding = er.ReadUInt16();
        this.Index = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(this.Padding);
        er.Write(this.Index);
      }

      public static implicit operator NKMProperties.KTPS(NKM.KTPSEntry o)
      {
        return new NKMProperties.KTPS()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Rx = o.Rotation.X,
          Ry = o.Rotation.Y,
          Rz = o.Rotation.Z,
          Index = o.Index,
          Padding = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Padding)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.KTPSEntry(NKMProperties.KTPS o)
      {
        return new NKM.KTPSEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Rotation = {
            X = o.Rx,
            Y = o.Ry,
            Z = o.Rz
          },
          Index = o.Index,
          Padding = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Padding)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator ListViewItem(NKM.KTPSEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Rotation.X.ToString(),
            o.Rotation.Y.ToString(),
            o.Rotation.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Padding)).Replace("-", ""),
            o.Index.ToString()
          }
        };
      }
    }

    public class KTPJEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public short EnemyPositionID;
      public short ItemPositionID;
      public int Index;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er, bool Compensate, int Idx)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.EnemyPositionID = er.ReadInt16();
        this.ItemPositionID = er.ReadInt16();
        if (Compensate)
          this.Index = Idx;
        else
          this.Index = er.ReadInt32();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(this.EnemyPositionID);
        er.Write(this.ItemPositionID);
        er.Write(this.Index);
      }

      public static implicit operator NKMProperties.KTPJ(NKM.KTPJEntry o)
      {
        return new NKMProperties.KTPJ()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Rx = o.Rotation.X,
          Ry = o.Rotation.Y,
          Rz = o.Rotation.Z,
          Index = o.Index,
          ItemPositionID = o.ItemPositionID,
          EnemyPositionID = o.EnemyPositionID
        };
      }

      public static implicit operator NKM.KTPJEntry(NKMProperties.KTPJ o)
      {
        return new NKM.KTPJEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Rotation = {
            X = o.Rx,
            Y = o.Ry,
            Z = o.Rz
          },
          Index = o.Index,
          ItemPositionID = o.ItemPositionID,
          EnemyPositionID = o.EnemyPositionID
        };
      }

      public static implicit operator ListViewItem(NKM.KTPJEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Rotation.X.ToString(),
            o.Rotation.Y.ToString(),
            o.Rotation.Z.ToString(),
            o.EnemyPositionID.ToString(),
            o.ItemPositionID.ToString(),
            o.Index.ToString()
          }
        };
      }
    }

    public class KTP2Entry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public ushort Padding;
      public short Index;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Padding = er.ReadUInt16();
        this.Index = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(this.Padding);
        er.Write(this.Index);
      }

      public static implicit operator NKMProperties.KTP2(NKM.KTP2Entry o)
      {
        return new NKMProperties.KTP2()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Rx = o.Rotation.X,
          Ry = o.Rotation.Y,
          Rz = o.Rotation.Z,
          Index = o.Index,
          Padding = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Padding)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.KTP2Entry(NKMProperties.KTP2 o)
      {
        return new NKM.KTP2Entry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Rotation = {
            X = o.Rx,
            Y = o.Ry,
            Z = o.Rz
          },
          Index = o.Index,
          Padding = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Padding)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator ListViewItem(NKM.KTP2Entry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Rotation.X.ToString(),
            o.Rotation.Y.ToString(),
            o.Rotation.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Padding)).Replace("-", ""),
            o.Index.ToString()
          }
        };
      }
    }

    public class KTPCEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public ushort Unknown1;
      public short Index;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Unknown1 = er.ReadUInt16();
        this.Index = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(this.Unknown1);
        er.Write(this.Index);
      }

      public static implicit operator NKMProperties.KTPC(NKM.KTPCEntry o)
      {
        return new NKMProperties.KTPC()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Rx = o.Rotation.X,
          Ry = o.Rotation.Y,
          Rz = o.Rotation.Z,
          Index = o.Index,
          Unknown1 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.KTPCEntry(NKMProperties.KTPC o)
      {
        return new NKM.KTPCEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Rotation = {
            X = o.Rx,
            Y = o.Ry,
            Z = o.Rz
          },
          Index = o.Index,
          Unknown1 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator ListViewItem(NKM.KTPCEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Rotation.X.ToString(),
            o.Rotation.Y.ToString(),
            o.Rotation.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown1)).Replace("-", ""),
            o.Index.ToString()
          }
        };
      }
    }

    public class KTPMEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public ushort Padding;
      public short Index;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Padding = er.ReadUInt16();
        this.Index = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(this.Padding);
        er.Write(this.Index);
      }

      public static implicit operator NKMProperties.KTPM(NKM.KTPMEntry o)
      {
        return new NKMProperties.KTPM()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Rx = o.Rotation.X,
          Ry = o.Rotation.Y,
          Rz = o.Rotation.Z,
          Index = o.Index,
          Padding = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Padding)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.KTPMEntry(NKMProperties.KTPM o)
      {
        return new NKM.KTPMEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Rotation = {
            X = o.Rx,
            Y = o.Ry,
            Z = o.Rz
          },
          Index = o.Index,
          Padding = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Padding)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator ListViewItem(NKM.KTPMEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Rotation.X.ToString(),
            o.Rotation.Y.ToString(),
            o.Rotation.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Padding)).Replace("-", ""),
            o.Index.ToString()
          }
        };
      }
    }

    public class CPOIEntry : NKM.NKMEntry
    {
      public Vector2 Position1;
      public Vector2 Position2;
      public float Sinus;
      public float Cosinus;
      public float Distance;
      public uint SectionData;
      public short KeyPoint;
      public short RespawnID;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position1 = new Vector2(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Position2 = new Vector2(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Sinus = er.ReadSingleInt32Exp12();
        this.Cosinus = er.ReadSingleInt32Exp12();
        this.Distance = er.ReadSingleInt32Exp12();
        this.SectionData = er.ReadUInt32();
        this.KeyPoint = er.ReadInt16();
        this.RespawnID = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position1.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position1.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position2.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position2.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Sinus), 0, 4);
        er.Write(Bytes.HexConverter(this.Cosinus), 0, 4);
        er.Write(Bytes.HexConverter(this.Distance), 0, 4);
        er.Write(this.SectionData);
        er.Write(this.KeyPoint);
        er.Write(this.RespawnID);
      }

      public static implicit operator NKMProperties.CPOI(NKM.CPOIEntry o)
      {
        return new NKMProperties.CPOI()
        {
          Tx1 = o.Position1.X,
          Tz1 = o.Position1.Y,
          Tx2 = o.Position2.X,
          Tz2 = o.Position2.Y,
          KeyPoint = o.KeyPoint,
          RespawnID = o.RespawnID,
          SectionData = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.SectionData)).Reverse<byte>().ToArray<byte>(), 0),
          Distance = o.Distance
        };
      }

      public static implicit operator NKM.CPOIEntry(NKMProperties.CPOI o)
      {
        return new NKM.CPOIEntry()
        {
          Position1 = {
            X = o.Tx1,
            Y = o.Tz1
          },
          Position2 = {
            X = o.Tx2,
            Y = o.Tz2
          },
          KeyPoint = o.KeyPoint,
          RespawnID = o.RespawnID,
          SectionData = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.SectionData)).Reverse<byte>().ToArray<byte>(), 0),
          Distance = o.Distance,
          Sinus = o.Sinus,
          Cosinus = o.Cosinus
        };
      }

      public static implicit operator ListViewItem(NKM.CPOIEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position1.X.ToString(),
            o.Position1.Y.ToString(),
            o.Position2.X.ToString(),
            o.Position2.Y.ToString(),
            o.Sinus.ToString(),
            o.Cosinus.ToString(),
            o.Distance.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.SectionData)).Replace("-", ""),
            o.KeyPoint.ToString(),
            o.RespawnID.ToString()
          }
        };
      }
    }

    public class CPATEntry : NKM.NKMEntry
    {
      public short StartIdx;
      public short Length;
      public sbyte GoesTo1;
      public sbyte GoesTo2;
      public sbyte GoesTo3;
      public sbyte ComesFrom1;
      public sbyte ComesFrom2;
      public sbyte ComesFrom3;
      public short SectionOrder;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.StartIdx = er.ReadInt16();
        this.Length = er.ReadInt16();
        this.GoesTo1 = er.ReadSByte();
        this.GoesTo2 = er.ReadSByte();
        this.GoesTo3 = er.ReadSByte();
        this.ComesFrom1 = er.ReadSByte();
        this.ComesFrom2 = er.ReadSByte();
        this.ComesFrom3 = er.ReadSByte();
        this.SectionOrder = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(this.StartIdx);
        er.Write(this.Length);
        er.Write(this.GoesTo1);
        er.Write(this.GoesTo2);
        er.Write(this.GoesTo3);
        er.Write(this.ComesFrom1);
        er.Write(this.ComesFrom2);
        er.Write(this.ComesFrom3);
        er.Write(this.SectionOrder);
      }

      public static implicit operator NKMProperties.CPAT(NKM.CPATEntry o)
      {
        return new NKMProperties.CPAT()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          Length = o.Length,
          SectionOrder = o.SectionOrder,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator NKM.CPATEntry(NKMProperties.CPAT o)
      {
        return new NKM.CPATEntry()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          Length = o.Length,
          SectionOrder = o.SectionOrder,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator ListViewItem(NKM.CPATEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.StartIdx.ToString(),
            o.Length.ToString(),
            o.GoesTo1.ToString(),
            o.GoesTo2.ToString(),
            o.GoesTo3.ToString(),
            o.ComesFrom1.ToString(),
            o.ComesFrom2.ToString(),
            o.ComesFrom3.ToString(),
            o.SectionOrder.ToString()
          }
        };
      }
    }

    public class IPOIEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public uint Unknown1;
      public uint Unknown2;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er, bool Compensate, int Idx)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Unknown1 = er.ReadUInt32();
        if (Compensate)
          this.Unknown2 = 0U;
        else
          this.Unknown2 = er.ReadUInt32();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(this.Unknown1);
        er.Write(this.Unknown2);
      }

      public static implicit operator NKMProperties.IPOI(NKM.IPOIEntry o)
      {
        return new NKMProperties.IPOI()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.IPOIEntry(NKMProperties.IPOI o)
      {
        return new NKM.IPOIEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator ListViewItem(NKM.IPOIEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown1)).Replace("-", ""),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown2)).Replace("-", "")
          }
        };
      }
    }

    public class IPATEntry : NKM.NKMEntry
    {
      public short StartIdx;
      public short Length;
      public sbyte GoesTo1;
      public sbyte GoesTo2;
      public sbyte GoesTo3;
      public sbyte ComesFrom1;
      public sbyte ComesFrom2;
      public sbyte ComesFrom3;
      public short SectionOrder;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.StartIdx = er.ReadInt16();
        this.Length = er.ReadInt16();
        this.GoesTo1 = er.ReadSByte();
        this.GoesTo2 = er.ReadSByte();
        this.GoesTo3 = er.ReadSByte();
        this.ComesFrom1 = er.ReadSByte();
        this.ComesFrom2 = er.ReadSByte();
        this.ComesFrom3 = er.ReadSByte();
        this.SectionOrder = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(this.StartIdx);
        er.Write(this.Length);
        er.Write(this.GoesTo1);
        er.Write(this.GoesTo2);
        er.Write(this.GoesTo3);
        er.Write(this.ComesFrom1);
        er.Write(this.ComesFrom2);
        er.Write(this.ComesFrom3);
        er.Write(this.SectionOrder);
      }

      public static implicit operator NKMProperties.IPAT(NKM.IPATEntry o)
      {
        return new NKMProperties.IPAT()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          Length = o.Length,
          SectionOrder = o.SectionOrder,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator NKM.IPATEntry(NKMProperties.IPAT o)
      {
        return new NKM.IPATEntry()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          Length = o.Length,
          SectionOrder = o.SectionOrder,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator ListViewItem(NKM.IPATEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.StartIdx.ToString(),
            o.Length.ToString(),
            o.GoesTo1.ToString(),
            o.GoesTo2.ToString(),
            o.GoesTo3.ToString(),
            o.ComesFrom1.ToString(),
            o.ComesFrom2.ToString(),
            o.ComesFrom3.ToString(),
            o.SectionOrder.ToString()
          }
        };
      }
    }

    public class EPOIEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public float PointSize;
      public short Drifting;
      public ushort Unknown1;
      public uint Unknown2;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.PointSize = er.ReadSingleInt32Exp12();
        this.Drifting = er.ReadInt16();
        this.Unknown1 = er.ReadUInt16();
        this.Unknown2 = er.ReadUInt32();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.PointSize), 0, 4);
        er.Write(this.Drifting);
        er.Write(this.Unknown1);
        er.Write(this.Unknown2);
      }

      public static implicit operator NKMProperties.EPOI(NKM.EPOIEntry o)
      {
        return new NKMProperties.EPOI()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          PointSize = o.PointSize,
          Drifting = o.Drifting,
          Unknown1 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.EPOIEntry(NKMProperties.EPOI o)
      {
        return new NKM.EPOIEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          PointSize = o.PointSize,
          Unknown1 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0),
          Drifting = o.Drifting
        };
      }

      public static implicit operator ListViewItem(NKM.EPOIEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.PointSize.ToString(),
            o.Drifting.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown1)).Replace("-", ""),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown2)).Replace("-", "")
          }
        };
      }
    }

    public class EPATEntry : NKM.NKMEntry
    {
      public short StartIdx;
      public short Length;
      public sbyte GoesTo1;
      public sbyte GoesTo2;
      public sbyte GoesTo3;
      public sbyte ComesFrom1;
      public sbyte ComesFrom2;
      public sbyte ComesFrom3;
      public short SectionOrder;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.StartIdx = er.ReadInt16();
        this.Length = er.ReadInt16();
        this.GoesTo1 = er.ReadSByte();
        this.GoesTo2 = er.ReadSByte();
        this.GoesTo3 = er.ReadSByte();
        this.ComesFrom1 = er.ReadSByte();
        this.ComesFrom2 = er.ReadSByte();
        this.ComesFrom3 = er.ReadSByte();
        this.SectionOrder = er.ReadInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(this.StartIdx);
        er.Write(this.Length);
        er.Write(this.GoesTo1);
        er.Write(this.GoesTo2);
        er.Write(this.GoesTo3);
        er.Write(this.ComesFrom1);
        er.Write(this.ComesFrom2);
        er.Write(this.ComesFrom3);
        er.Write(this.SectionOrder);
      }

      public static implicit operator NKMProperties.EPAT(NKM.EPATEntry o)
      {
        return new NKMProperties.EPAT()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          Length = o.Length,
          SectionOrder = o.SectionOrder,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator NKM.EPATEntry(NKMProperties.EPAT o)
      {
        return new NKM.EPATEntry()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          Length = o.Length,
          SectionOrder = o.SectionOrder,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator ListViewItem(NKM.EPATEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.StartIdx.ToString(),
            o.Length.ToString(),
            o.GoesTo1.ToString(),
            o.GoesTo2.ToString(),
            o.GoesTo3.ToString(),
            o.ComesFrom1.ToString(),
            o.ComesFrom2.ToString(),
            o.ComesFrom3.ToString(),
            o.SectionOrder.ToString()
          }
        };
      }
    }

    public class MEPOEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public float PointSize;
      public int Drifting;
      public uint Unknown1;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.PointSize = er.ReadSingleInt32Exp12();
        this.Drifting = er.ReadInt32();
        this.Unknown1 = er.ReadUInt32();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.PointSize), 0, 4);
        er.Write(this.Drifting);
        er.Write(this.Unknown1);
      }

      public static implicit operator NKMProperties.MEPO(NKM.MEPOEntry o)
      {
        return new NKMProperties.MEPO()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          PointSize = o.PointSize,
          Drifting = o.Drifting,
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0)
        };
      }

      public static implicit operator NKM.MEPOEntry(NKMProperties.MEPO o)
      {
        return new NKM.MEPOEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          PointSize = o.PointSize,
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Drifting = o.Drifting
        };
      }

      public static implicit operator ListViewItem(NKM.MEPOEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.PointSize.ToString(),
            o.Drifting.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown1)).Replace("-", "")
          }
        };
      }
    }

    public class MEPAEntry : NKM.NKMEntry
    {
      public short StartIdx;
      public short Length;
      public sbyte GoesTo1;
      public sbyte GoesTo2;
      public sbyte GoesTo3;
      public sbyte GoesTo4;
      public sbyte GoesTo5;
      public sbyte GoesTo6;
      public sbyte GoesTo7;
      public sbyte GoesTo8;
      public sbyte ComesFrom1;
      public sbyte ComesFrom2;
      public sbyte ComesFrom3;
      public sbyte ComesFrom4;
      public sbyte ComesFrom5;
      public sbyte ComesFrom6;
      public sbyte ComesFrom7;
      public sbyte ComesFrom8;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.StartIdx = er.ReadInt16();
        this.Length = er.ReadInt16();
        this.GoesTo1 = er.ReadSByte();
        this.GoesTo2 = er.ReadSByte();
        this.GoesTo3 = er.ReadSByte();
        this.GoesTo4 = er.ReadSByte();
        this.GoesTo5 = er.ReadSByte();
        this.GoesTo6 = er.ReadSByte();
        this.GoesTo7 = er.ReadSByte();
        this.GoesTo8 = er.ReadSByte();
        this.ComesFrom1 = er.ReadSByte();
        this.ComesFrom2 = er.ReadSByte();
        this.ComesFrom3 = er.ReadSByte();
        this.ComesFrom4 = er.ReadSByte();
        this.ComesFrom5 = er.ReadSByte();
        this.ComesFrom6 = er.ReadSByte();
        this.ComesFrom7 = er.ReadSByte();
        this.ComesFrom8 = er.ReadSByte();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(this.StartIdx);
        er.Write(this.Length);
        er.Write(this.GoesTo1);
        er.Write(this.GoesTo2);
        er.Write(this.GoesTo3);
        er.Write(this.GoesTo4);
        er.Write(this.GoesTo5);
        er.Write(this.GoesTo6);
        er.Write(this.GoesTo7);
        er.Write(this.GoesTo8);
        er.Write(this.ComesFrom1);
        er.Write(this.ComesFrom2);
        er.Write(this.ComesFrom3);
        er.Write(this.ComesFrom4);
        er.Write(this.ComesFrom5);
        er.Write(this.ComesFrom6);
        er.Write(this.ComesFrom7);
        er.Write(this.ComesFrom8);
      }

      public static implicit operator NKMProperties.MEPA(NKM.MEPAEntry o)
      {
        return new NKMProperties.MEPA()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          ComesFrom4 = o.ComesFrom4,
          ComesFrom5 = o.ComesFrom5,
          ComesFrom6 = o.ComesFrom6,
          ComesFrom7 = o.ComesFrom7,
          ComesFrom8 = o.ComesFrom8,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          GoesTo4 = o.GoesTo4,
          GoesTo5 = o.GoesTo5,
          GoesTo6 = o.GoesTo6,
          GoesTo7 = o.GoesTo7,
          GoesTo8 = o.GoesTo8,
          Length = o.Length,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator NKM.MEPAEntry(NKMProperties.MEPA o)
      {
        return new NKM.MEPAEntry()
        {
          ComesFrom1 = o.ComesFrom1,
          ComesFrom2 = o.ComesFrom2,
          ComesFrom3 = o.ComesFrom3,
          ComesFrom4 = o.ComesFrom4,
          ComesFrom5 = o.ComesFrom5,
          ComesFrom6 = o.ComesFrom6,
          ComesFrom7 = o.ComesFrom7,
          ComesFrom8 = o.ComesFrom8,
          GoesTo1 = o.GoesTo1,
          GoesTo2 = o.GoesTo2,
          GoesTo3 = o.GoesTo3,
          GoesTo4 = o.GoesTo4,
          GoesTo5 = o.GoesTo5,
          GoesTo6 = o.GoesTo6,
          GoesTo7 = o.GoesTo7,
          GoesTo8 = o.GoesTo8,
          Length = o.Length,
          StartIdx = o.StartIdx
        };
      }

      public static implicit operator ListViewItem(NKM.MEPAEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.StartIdx.ToString(),
            o.Length.ToString(),
            o.GoesTo1.ToString(),
            o.GoesTo2.ToString(),
            o.GoesTo3.ToString(),
            o.GoesTo4.ToString(),
            o.GoesTo5.ToString(),
            o.GoesTo6.ToString(),
            o.GoesTo7.ToString(),
            o.GoesTo8.ToString(),
            o.ComesFrom1.ToString(),
            o.ComesFrom2.ToString(),
            o.ComesFrom3.ToString(),
            o.ComesFrom4.ToString(),
            o.ComesFrom5.ToString(),
            o.ComesFrom6.ToString(),
            o.ComesFrom7.ToString(),
            o.ComesFrom8.ToString()
          }
        };
      }
    }

    public class AREAEntry : NKM.NKMEntry
    {
      public Vector3 Position;
      public Vector3 Unknown1;
      public uint Unknown2;
      public uint Unknown3;
      public uint Unknown4;
      public uint Unknown5;
      public uint Unknown6;
      public uint Unknown7;
      public uint Unknown8;
      public uint Unknown9;
      public uint Unknown10;
      public uint Unknown11;
      public short Unknown12;
      public byte Unknown13;
      public sbyte LinkedCame;
      public int Unknown14;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Unknown1 = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Unknown2 = er.ReadUInt32();
        this.Unknown3 = er.ReadUInt32();
        this.Unknown4 = er.ReadUInt32();
        this.Unknown5 = er.ReadUInt32();
        this.Unknown6 = er.ReadUInt32();
        this.Unknown7 = er.ReadUInt32();
        this.Unknown8 = er.ReadUInt32();
        this.Unknown9 = er.ReadUInt32();
        this.Unknown10 = er.ReadUInt32();
        this.Unknown11 = er.ReadUInt32();
        this.Unknown12 = er.ReadInt16();
        this.Unknown13 = er.ReadByte();
        this.LinkedCame = er.ReadSByte();
        this.Unknown14 = er.ReadInt32();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Unknown1.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Unknown1.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Unknown1.Z), 0, 4);
        er.Write(this.Unknown2);
        er.Write(this.Unknown3);
        er.Write(this.Unknown4);
        er.Write(this.Unknown5);
        er.Write(this.Unknown6);
        er.Write(this.Unknown7);
        er.Write(this.Unknown8);
        er.Write(this.Unknown9);
        er.Write(this.Unknown10);
        er.Write(this.Unknown11);
        er.Write(this.Unknown12);
        er.Write(this.Unknown13);
        er.Write(this.LinkedCame);
        er.Write(this.Unknown14);
      }

      public static implicit operator NKMProperties.AREA(NKM.AREAEntry o)
      {
        return new NKMProperties.AREA()
        {
          Tx = o.Position.X,
          Ty = o.Position.Y,
          Tz = o.Position.Z,
          Unknown1 = o.Unknown1.X,
          Unknown2 = o.Unknown1.Y,
          Unknown3 = o.Unknown1.Z,
          Unknown4 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown5 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown3)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown6 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown4)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown7 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown5)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown8 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown6)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown9 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown7)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown10 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown8)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown11 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown9)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown12 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown10)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown13 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown11)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown14 = o.Unknown12,
          Unknown15 = o.Unknown13,
          Unknown16 = o.Unknown14,
          LinkedCame = o.LinkedCame
        };
      }

      public static implicit operator NKM.AREAEntry(NKMProperties.AREA o)
      {
        return new NKM.AREAEntry()
        {
          Position = {
            X = o.Tx,
            Y = o.Ty,
            Z = o.Tz
          },
          Unknown1 = {
            X = o.Unknown1,
            Y = o.Unknown2,
            Z = o.Unknown3
          },
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown4)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown3 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown5)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown4 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown6)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown5 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown7)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown6 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown8)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown7 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown9)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown8 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown10)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown9 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown11)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown10 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown12)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown11 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown13)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown12 = o.Unknown14,
          Unknown13 = o.Unknown15,
          Unknown14 = o.Unknown16,
          LinkedCame = o.LinkedCame
        };
      }

      public static implicit operator ListViewItem(NKM.AREAEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position.X.ToString(),
            o.Position.Y.ToString(),
            o.Position.Z.ToString(),
            o.Unknown1.X.ToString(),
            o.Unknown1.Y.ToString(),
            o.Unknown1.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown2)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown3)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown4)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown5)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown6)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown7)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown8)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown9)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown10)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown11)).Replace("-", "").ToString(),
            o.Unknown12.ToString(),
            o.Unknown13.ToString(),
            o.LinkedCame.ToString(),
            o.Unknown14.ToString()
          }
        };
      }
    }

    public class CAMEEntry : NKM.NKMEntry
    {
      public Vector3 Position1;
      public Vector3 Rotation;
      public Vector3 Position2;
      public Vector3 Position3;
      public uint Unknown1;
      public uint Unknown2;
      public uint Unknown3;
      public short CameraZoom;
      public short CameraType;
      public short LinkedRoute;
      public short RouteSpeed;
      public short PointSpeed;
      public TimeSpan TotalLength;
      public short NextCame;
      public ushort Unknown4;

      public override ListViewItem ToListViewItem()
      {
        return (ListViewItem) this;
      }

      public override void Read(EndianBinaryReader er)
      {
        this.Position1 = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Rotation = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Position2 = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Position3 = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Unknown1 = er.ReadUInt32();
        this.Unknown2 = er.ReadUInt32();
        this.Unknown3 = er.ReadUInt32();
        this.CameraZoom = er.ReadInt16();
        this.CameraType = er.ReadInt16();
        this.LinkedRoute = er.ReadInt16();
        this.RouteSpeed = er.ReadInt16();
        this.PointSpeed = er.ReadInt16();
        this.TotalLength = TimeSpan.FromSeconds((double) er.ReadInt16() / 60.0);
        this.NextCame = er.ReadInt16();
        this.Unknown4 = er.ReadUInt16();
      }

      public override void Write(EndianBinaryWriter er)
      {
        er.Write(Bytes.HexConverter(this.Position1.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position1.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position1.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Rotation.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Position2.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position2.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position2.Z), 0, 4);
        er.Write(Bytes.HexConverter(this.Position3.X), 0, 4);
        er.Write(Bytes.HexConverter(this.Position3.Y), 0, 4);
        er.Write(Bytes.HexConverter(this.Position3.Z), 0, 4);
        er.Write(this.Unknown1);
        er.Write(this.Unknown2);
        er.Write(this.Unknown3);
        er.Write(this.CameraZoom);
        er.Write(this.CameraType);
        er.Write(this.LinkedRoute);
        er.Write(this.RouteSpeed);
        er.Write(this.PointSpeed);
        er.Write((short) (this.TotalLength.TotalSeconds * 60.0));
        er.Write(this.NextCame);
        er.Write(this.Unknown4);
      }

      public static implicit operator NKMProperties.CAME(NKM.CAMEEntry o)
      {
        return new NKMProperties.CAME()
        {
          Tx1 = o.Position1.X,
          Ty1 = o.Position1.Y,
          Tz1 = o.Position1.Z,
          Tx2 = o.Position2.X,
          Ty2 = o.Position2.Y,
          Tz2 = o.Position2.Z,
          Tx3 = o.Position3.X,
          Ty3 = o.Position3.Y,
          Tz3 = o.Position3.Z,
          Rx = o.Rotation.X,
          Ry = o.Rotation.Y,
          Rz = o.Rotation.Z,
          CameraZoom = o.CameraZoom,
          CameraType = o.CameraType,
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown3 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown3)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown4 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown4)).Reverse<byte>().ToArray<byte>(), 0),
          LinkedRoute = o.LinkedRoute,
          NextCame = o.NextCame,
          TotalLength = o.TotalLength,
          PointSpeed = o.PointSpeed,
          RouteSpeed = o.RouteSpeed
        };
      }

      public static implicit operator NKM.CAMEEntry(NKMProperties.CAME o)
      {
        return new NKM.CAMEEntry()
        {
          Position1 = {
            X = o.Tx1,
            Y = o.Ty1,
            Z = o.Tz1
          },
          Position2 = {
            X = o.Tx2,
            Y = o.Ty2,
            Z = o.Tz2
          },
          Position3 = {
            X = o.Tx3,
            Y = o.Ty3,
            Z = o.Tz3
          },
          Rotation = {
            X = o.Rx,
            Y = o.Ry,
            Z = o.Rz
          },
          CameraZoom = o.CameraZoom,
          CameraType = o.CameraType,
          Unknown1 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown1)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown2 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown2)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown3 = BitConverter.ToUInt32(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown3)).Reverse<byte>().ToArray<byte>(), 0),
          Unknown4 = BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(o.Unknown4)).Reverse<byte>().ToArray<byte>(), 0),
          LinkedRoute = o.LinkedRoute,
          NextCame = o.NextCame,
          TotalLength = o.TotalLength,
          PointSpeed = o.PointSpeed,
          RouteSpeed = o.RouteSpeed
        };
      }

      public static implicit operator ListViewItem(NKM.CAMEEntry o)
      {
        return new ListViewItem()
        {
          SubItems = {
            o.Position1.X.ToString(),
            o.Position1.Y.ToString(),
            o.Position1.Z.ToString(),
            o.Rotation.X.ToString(),
            o.Rotation.Y.ToString(),
            o.Rotation.Z.ToString(),
            o.Position2.X.ToString(),
            o.Position2.Y.ToString(),
            o.Position2.Z.ToString(),
            o.Position3.X.ToString(),
            o.Position3.Y.ToString(),
            o.Position3.Z.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown1)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown2)).Replace("-", "").ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown3)).Replace("-", "").ToString(),
            o.CameraZoom.ToString(),
            o.CameraType.ToString(),
            o.LinkedRoute.ToString(),
            o.RouteSpeed.ToString(),
            o.PointSpeed.ToString(),
            o.TotalLength.ToString(),
            o.NextCame.ToString(),
            BitConverter.ToString(BitConverter.GetBytes(o.Unknown4)).Replace("-", "").ToString()
          }
        };
      }
    }

    public class NKMIEntry
    {
      public string Type;
      public uint Length;
      public string TrackName;
      public string Author;
      public string Version;
      public string LastEditDate;

      public NKMIEntry(EndianBinaryReader er, string Signature)
      {
        this.Type = Signature;
        this.Length = er.ReadUInt32();
        this.TrackName = er.ReadStringNT(Encoding.ASCII);
        this.Author = er.ReadStringNT(Encoding.ASCII);
        this.Version = er.ReadStringNT(Encoding.ASCII);
        this.LastEditDate = er.ReadStringNT(Encoding.ASCII);
      }

      public NKMIEntry()
      {
        this.Type = "NKMI";
        this.TrackName = "Undefined";
        this.Author = "Undefined";
        this.Version = "Undefined";
        this.LastEditDate = "Undefined";
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Type, Encoding.ASCII, false);
        this.LastEditDate = DateTime.Now.ToString("dd\\/MM\\/yyyy HH\\:mm\\:ss");
        er.Write((uint) (8 + this.TrackName.Length + 1 + this.Author.Length + 1 + this.Version.Length + 1 + this.LastEditDate.Length + 1));
        er.Write(this.TrackName, Encoding.ASCII, true);
        er.Write(this.Author, Encoding.ASCII, true);
        er.Write(this.Version, Encoding.ASCII, true);
        er.Write(this.LastEditDate, Encoding.ASCII, true);
      }
    }
  }
}
