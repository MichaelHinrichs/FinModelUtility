// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NDS
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Archive_Format;
using MKDS_Course_Modifier.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace MKDS_Course_Modifier.IO
{
  public class NDS
  {
    public byte Unitcode;
    public byte EncryptionSeedSelect;
    public byte Devicecapacity;
    public byte ROMVersion;
    public byte InternalFlags;
    public uint ARM9RomOffset;
    public uint ARM9EntryAddress;
    public uint ARM9LoadAddress;
    public uint ARM9Size;
    public uint ARM7RomOffset;
    public uint ARM7EntryAddress;
    public uint ARM7LoadAddress;
    public uint ARM7Size;
    public uint FileNameTableOffset;
    public uint FileNameTableLength;
    public uint FileAllocationTableOffset;
    public uint FileAllocationTableLength;
    public uint ARM9OverlayOffset;
    public uint ARM9OverlayLength;
    public uint ARM7OverlayOffset;
    public uint ARM7OverlayLength;
    public uint NormalCardControlRegisterSettings;
    public uint SecureCardControlRegisterSettings;
    public uint IconBannerOffset;
    public ushort SecureAreaCRC;
    public ushort SecureTransferTimeout;
    public uint ARM9Autoload;
    public uint ARM7Autoload;
    public uint SecureDisable;
    public uint NTRRegionROMSize;
    public uint HeaderSize;
    public byte[] NintendoLogo;
    public ushort NintendoLogoCRC;
    public ushort HeaderCRC;
    public NARC.DirectoryEntry Root;
    public byte[] Arm9;
    public byte[] Arm7;
    public byte[] ARM9Overlay;
    public byte[] ARM7Overlay;

    public NDS(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.GameTitle = er.ReadString(Encoding.ASCII, 12).Replace("\0", "");
      this.GameCode = er.ReadString(Encoding.ASCII, 4).Replace("\0", "");
      this.MakerCode = er.ReadString(Encoding.ASCII, 2).Replace("\0", "");
      this.Unitcode = er.ReadByte();
      this.EncryptionSeedSelect = er.ReadByte();
      this.Devicecapacity = er.ReadByte();
      er.ReadBytes(9);
      this.ROMVersion = er.ReadByte();
      this.InternalFlags = er.ReadByte();
      this.ARM9RomOffset = er.ReadUInt32();
      this.ARM9EntryAddress = er.ReadUInt32();
      this.ARM9LoadAddress = er.ReadUInt32();
      this.ARM9Size = er.ReadUInt32();
      this.ARM7RomOffset = er.ReadUInt32();
      this.ARM7EntryAddress = er.ReadUInt32();
      this.ARM7LoadAddress = er.ReadUInt32();
      this.ARM7Size = er.ReadUInt32();
      this.FileNameTableOffset = er.ReadUInt32();
      this.FileNameTableLength = er.ReadUInt32();
      this.FileAllocationTableOffset = er.ReadUInt32();
      this.FileAllocationTableLength = er.ReadUInt32();
      this.ARM9OverlayOffset = er.ReadUInt32();
      this.ARM9OverlayLength = er.ReadUInt32();
      this.ARM7OverlayOffset = er.ReadUInt32();
      this.ARM7OverlayLength = er.ReadUInt32();
      this.NormalCardControlRegisterSettings = er.ReadUInt32();
      this.SecureCardControlRegisterSettings = er.ReadUInt32();
      this.IconBannerOffset = er.ReadUInt32();
      this.SecureAreaCRC = er.ReadUInt16();
      this.SecureTransferTimeout = er.ReadUInt16();
      this.ARM9Autoload = er.ReadUInt32();
      this.ARM7Autoload = er.ReadUInt32();
      this.SecureDisable = er.ReadUInt32();
      this.NTRRegionROMSize = er.ReadUInt32();
      this.HeaderSize = er.ReadUInt32();
      er.ReadBytes(56);
      this.NintendoLogo = er.ReadBytes(156);
      this.NintendoLogoCRC = er.ReadUInt16();
      this.HeaderCRC = er.ReadUInt16();
      er.ReadBytes(160);
      if (this.IconBannerOffset != 0U)
      {
        er.BaseStream.Position = (long) this.IconBannerOffset;
        this.Banner = new NDS.IconBanner(er);
      }
      this.Root = new NARC.DirectoryEntry("root", 0);
      er.BaseStream.Position = (long) this.FileNameTableOffset;
      uint num1 = er.ReadUInt32();
      ushort num2 = er.ReadUInt16();
      ushort num3 = er.ReadUInt16();
      List<string> source1 = new List<string>();
      List<string> source2 = new List<string>();
      List<int> intList1 = new List<int>();
      List<ushort> source3 = new List<ushort>();
      List<int> intList2 = new List<int>();
      int id1 = 0;
      NARC.DirectoryEntry directoryEntry = this.Root;
      er.BaseStream.Position = (long) (num1 + this.FileNameTableOffset);
      for (int index = 0; (long) index < (long) (this.FileAllocationTableLength / 8U - (uint) num2) + (long) ((int) num3 - 1); ++index)
      {
        byte num4 = er.ReadByte();
        if (num4 < (byte) 128 && num4 > (byte) 0)
        {
          source1.Add(er.ReadString(Encoding.ASCII, (int) num4));
          intList1.Add(id1);
          directoryEntry.Files.Add(new NARC.FileEntry(source1.Last<string>(), intList1.Count - 1));
        }
        else if (num4 > (byte) 128 && num4 < (byte) 176)
        {
          byte num5 = (byte) ((uint) num4 - 128U);
          source2.Add(er.ReadString(Encoding.ASCII, (int) num5));
          source3.Add((ushort) ((uint) er.ReadUInt16() - 61440U));
          intList2.Add(id1);
          directoryEntry.Subdirs.Add(new NARC.DirectoryEntry(source2.Last<string>(), (int) source3.Last<ushort>()));
        }
        else
        {
          ++id1;
          directoryEntry = this.Root.GetDirById(id1);
          if (directoryEntry != null)
            --index;
          else
            break;
        }
      }
      er.BaseStream.Position = (long) this.FileAllocationTableOffset;
      er.ReadBytes((int) num2 * 8);
      for (int id2 = 0; (long) id2 < (long) (this.FileAllocationTableLength / 8U - (uint) num2); ++id2)
      {
        uint num4 = er.ReadUInt32();
        uint num5 = er.ReadUInt32();
        long position = er.BaseStream.Position;
        er.BaseStream.Position = (long) num4;
        int count = (int) num5 - (int) num4;
        this.Root.SetFile(er.ReadBytes(count), id2);
        er.BaseStream.Position = position;
      }
      er.BaseStream.Position = (long) this.ARM9RomOffset;
      this.Arm9 = er.ReadBytes((int) this.ARM9Size);
      er.BaseStream.Position = (long) this.ARM7RomOffset;
      this.Arm7 = er.ReadBytes((int) this.ARM7Size);
      er.BaseStream.Position = (long) this.ARM9OverlayOffset;
      this.ARM9Overlay = er.ReadBytes((int) this.ARM9OverlayLength);
      er.BaseStream.Position = (long) this.ARM7OverlayOffset;
      this.ARM7Overlay = er.ReadBytes((int) this.ARM7OverlayLength);
      er.Close();
    }

    public string GameTitle { get; set; }

    public string GameCode { get; set; }

    public string MakerCode { get; set; }

    [TypeConverter(typeof (ExpandableObjectConverter))]
    public NDS.IconBanner Banner { get; set; }

    public class IconBanner
    {
      public ushort Version;
      public ushort CRC16;
      public byte[] Icon;
      public byte[] Palette;

      public IconBanner(EndianBinaryReader er)
      {
        this.Version = er.ReadUInt16();
        this.CRC16 = er.ReadUInt16();
        er.ReadBytes(28);
        this.Icon = er.ReadBytes(512);
        this.Palette = er.ReadBytes(32);
        this.TitleJP = er.ReadString(Encoding.ASCII, 256).Replace("\0", "");
        this.TitleEN = er.ReadString(Encoding.ASCII, 256).Replace("\0", "");
        this.TitleFR = er.ReadString(Encoding.ASCII, 256).Replace("\0", "");
        this.TitleGE = er.ReadString(Encoding.ASCII, 256).Replace("\0", "");
        this.TitleIT = er.ReadString(Encoding.ASCII, 256).Replace("\0", "");
        this.TitleSP = er.ReadString(Encoding.ASCII, 256).Replace("\0", "");
        er.ReadBytes(448);
      }

      [Category("Title")]
      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      [DisplayName("Japenese")]
      public string TitleJP { get; set; }

      [Category("Title")]
      [DisplayName("English")]
      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      public string TitleEN { get; set; }

      [Category("Title")]
      [DisplayName("French")]
      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      public string TitleFR { get; set; }

      [Category("Title")]
      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      [DisplayName("German")]
      public string TitleGE { get; set; }

      [Category("Title")]
      [DisplayName("Italian")]
      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      public string TitleIT { get; set; }

      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      [Category("Title")]
      [DisplayName("Spanish")]
      public string TitleSP { get; set; }

      [DisplayName("Icon")]
      [Category("Icon")]
      public Bitmap BitmapIcon
      {
        get
        {
          return this.GetIcon();
        }
        set
        {
          if (value.Width != 32 || value.Height != 32)
            throw new Exception("Wrong size! The bitmap needs to be 32x32.");
          Graphic.ConvertBitmap(value, out this.Icon, out this.Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out bool _, true);
        }
      }

      public Bitmap GetIcon()
      {
        return Graphic.ConvertData(this.Icon, this.Palette, (byte[]) null, 0, 32, 32, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, true, true);
      }

      public override string ToString()
      {
        return "Banner";
      }
    }
  }
}
