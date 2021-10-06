// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NDS_new
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.IO.NitroFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.IO
{
  public class NDS_new
  {
    public NDS_new.ROM_Header header;
    public byte[] main_rom;
    public byte[] sub_rom;
    public NDS_new.ROM_FNT fnt;
    public NDS_new.ROM_OVT[] main_ovt;
    public NDS_new.ROM_OVT[] sub_ovt;
    public FileAllocationEntry[] fat;
    public NDS_new.BannerFile banner;
    public byte[][] FileData;

    public NDS_new(byte[] data)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(data), Endianness.LittleEndian);
      this.header = new NDS_new.ROM_Header(er);
      er.BaseStream.Position = (long) this.header.main_rom_offset;
      this.main_rom = er.ReadBytes((int) this.header.main_size);
      er.BaseStream.Position = (long) this.header.sub_rom_offset;
      this.sub_rom = er.ReadBytes((int) this.header.sub_size);
      er.BaseStream.Position = (long) this.header.fnt_offset;
      this.fnt = new NDS_new.ROM_FNT(er);
      er.BaseStream.Position = (long) this.header.main_ovt_offset;
      this.main_ovt = new NDS_new.ROM_OVT[(IntPtr) (this.header.main_ovt_size / 32U)];
      for (int index = 0; (long) index < (long) (this.header.main_ovt_size / 32U); ++index)
        this.main_ovt[index] = new NDS_new.ROM_OVT(er);
      er.BaseStream.Position = (long) this.header.sub_ovt_offset;
      this.sub_ovt = new NDS_new.ROM_OVT[(IntPtr) (this.header.sub_ovt_size / 32U)];
      for (int index = 0; (long) index < (long) (this.header.sub_ovt_size / 32U); ++index)
        this.sub_ovt[index] = new NDS_new.ROM_OVT(er);
      er.BaseStream.Position = (long) this.header.fat_offset;
      this.fat = new FileAllocationEntry[(IntPtr) (this.header.fat_size / 8U)];
      for (int index = 0; (long) index < (long) (this.header.fat_size / 8U); ++index)
        this.fat[index] = new FileAllocationEntry(er);
      er.BaseStream.Position = (long) this.header.banner_offset;
      this.banner = new NDS_new.BannerFile(er);
      this.FileData = new byte[(IntPtr) (this.header.fat_size / 8U)][];
      for (int index = 0; (long) index < (long) (this.header.fat_size / 8U); ++index)
      {
        er.BaseStream.Position = (long) this.fat[index].fileTop;
        this.FileData[index] = er.ReadBytes((int) this.fat[index].fileSize);
      }
      er.Close();
    }

    public void ToFileSystem()
    {
      List<FileSystem.Directory> directoryList = new List<FileSystem.Directory>();
      directoryList.Add(new FileSystem.Directory("/", true));
      directoryList[0].DirectoryID = (ushort) 61440;
      uint dirParentId = (uint) this.fnt.directoryTable[0].dirParentID;
      for (int index = 1; (long) index < (long) dirParentId; ++index)
        directoryList.Add(new FileSystem.Directory((ushort) (61440 + index)));
      for (int index = 1; (long) index < (long) dirParentId; ++index)
        directoryList[index].Parent = directoryList[(int) this.fnt.directoryTable[index].dirParentID - 61440];
      for (int index1 = 0; (long) index1 < (long) dirParentId; ++index1)
      {
        for (int index2 = 0; (long) index2 < (long) dirParentId; ++index2)
        {
          if (directoryList[index1] == directoryList[index2].Parent)
            directoryList[index1].SubDirectories.Add(directoryList[index2]);
        }
      }
      uint num = dirParentId * 8U;
      ushort dirEntryFileId = this.fnt.directoryTable[0].dirEntryFileID;
      FileSystem.Directory Parent1 = (FileSystem.Directory) null;
      foreach (EntryNameTableEntry entryNameTableEntry in this.fnt.entryNameTable)
      {
        for (int index = 1; (long) index < (long) dirParentId; ++index)
        {
          if ((int) num == (int) this.fnt.directoryTable[index].dirEntryStart)
          {
            Parent1 = directoryList[index];
            break;
          }
        }
        switch (entryNameTableEntry)
        {
          case EntryNameTableEndOfDirectoryEntry _:
            Parent1 = (FileSystem.Directory) null;
            ++num;
            break;
          case EntryNameTableFileEntry _:
            Parent1.Files.Add(new FileSystem.File((int) dirEntryFileId++, ((EntryNameTableFileEntry) entryNameTableEntry).entryName, Parent1));
            num += 1U + (uint) entryNameTableEntry.entryNameLength;
            break;
          case EntryNameTableDirectoryEntry _:
            directoryList[(int) ((EntryNameTableDirectoryEntry) entryNameTableEntry).directoryID - 61440].DirectoryName = ((EntryNameTableDirectoryEntry) entryNameTableEntry).entryName;
            num += (uint) (3 + ((int) entryNameTableEntry.entryNameLength & (int) sbyte.MaxValue));
            break;
        }
      }
      FileSystem.Directory Parent2 = new FileSystem.Directory("", true);
      directoryList[0].IsRoot = false;
      directoryList[0].DirectoryName = "data";
      directoryList[0].Parent = Parent2;
      Parent2.SubDirectories.Add(directoryList[0]);
      Parent2.IsVirtualDirectory = true;
      for (int index = 0; index < this.main_ovt.Length; ++index)
        Parent2.Files.Add(new FileSystem.File((int) (ushort) this.main_ovt[index].file_id, "overlay9_" + this.main_ovt[index].id.ToString("d4") + ".bin", Parent2));
      for (int index = 0; index < this.sub_ovt.Length; ++index)
        Parent2.Files.Add(new FileSystem.File((int) (ushort) this.sub_ovt[index].file_id, "overlay7_" + this.sub_ovt[index].id.ToString("d4") + ".bin", Parent2));
      Parent2.Files.Add(new FileSystem.File(-1, "arm9.bin", Parent2));
      Parent2.Files.Add(new FileSystem.File(-2, "arm7.bin", Parent2));
      Parent2.GetFileByID(-1).Data = this.main_rom;
      Parent2.GetFileByID(-2).Data = this.sub_rom;
      for (int Id = 0; Id < this.fat.Length; ++Id)
        Parent2.GetFileByID(Id).Data = this.FileData[Id];
    }

    public class ROM_Header
    {
      public string game_name;
      public string game_code;
      public string maker_code;
      public byte product_id;
      public byte device_type;
      public byte device_size;
      public byte[] reserved_A;
      public byte game_version;
      public byte property;
      public uint main_rom_offset;
      public uint main_entry_address;
      public uint main_ram_address;
      public uint main_size;
      public uint sub_rom_offset;
      public uint sub_entry_address;
      public uint sub_ram_address;
      public uint sub_size;
      public uint fnt_offset;
      public uint fnt_size;
      public uint fat_offset;
      public uint fat_size;
      public uint main_ovt_offset;
      public uint main_ovt_size;
      public uint sub_ovt_offset;
      public uint sub_ovt_size;
      public byte[] rom_param_A;
      public uint banner_offset;
      public ushort secure_crc;
      public byte[] rom_param_B;
      public uint main_autoload_done;
      public uint sub_autoload_done;
      public byte[] rom_param_C;
      public uint rom_size;
      public uint header_size;
      public byte[] reserved_B;
      public byte[] logo_data;
      public ushort logo_crc;
      public ushort header_crc;

      public ROM_Header(EndianBinaryReader er)
      {
        this.game_name = er.ReadString(Encoding.ASCII, 12).Replace("\0", "");
        this.game_code = er.ReadString(Encoding.ASCII, 4).Replace("\0", "");
        this.maker_code = er.ReadString(Encoding.ASCII, 2).Replace("\0", "");
        this.product_id = er.ReadByte();
        this.device_type = er.ReadByte();
        this.device_size = er.ReadByte();
        this.reserved_A = er.ReadBytes(9);
        this.game_version = er.ReadByte();
        this.property = er.ReadByte();
        this.main_rom_offset = er.ReadUInt32();
        this.main_entry_address = er.ReadUInt32();
        this.main_ram_address = er.ReadUInt32();
        this.main_size = er.ReadUInt32();
        this.sub_rom_offset = er.ReadUInt32();
        this.sub_entry_address = er.ReadUInt32();
        this.sub_ram_address = er.ReadUInt32();
        this.sub_size = er.ReadUInt32();
        this.fnt_offset = er.ReadUInt32();
        this.fnt_size = er.ReadUInt32();
        this.fat_offset = er.ReadUInt32();
        this.fat_size = er.ReadUInt32();
        this.main_ovt_offset = er.ReadUInt32();
        this.main_ovt_size = er.ReadUInt32();
        this.sub_ovt_offset = er.ReadUInt32();
        this.sub_ovt_size = er.ReadUInt32();
        this.rom_param_A = er.ReadBytes(8);
        this.banner_offset = er.ReadUInt32();
        this.secure_crc = er.ReadUInt16();
        this.rom_param_B = er.ReadBytes(2);
        this.main_autoload_done = er.ReadUInt32();
        this.sub_autoload_done = er.ReadUInt32();
        this.rom_param_C = er.ReadBytes(8);
        this.rom_size = er.ReadUInt32();
        this.header_size = er.ReadUInt32();
        this.reserved_B = er.ReadBytes(56);
        this.logo_data = er.ReadBytes(156);
        this.logo_crc = er.ReadUInt16();
        this.header_crc = er.ReadUInt16();
      }
    }

    public class ROM_FNT
    {
      public List<DirectoryTableEntry> directoryTable;
      public List<EntryNameTableEntry> entryNameTable;

      public ROM_FNT(EndianBinaryReader er)
      {
        this.directoryTable = new List<DirectoryTableEntry>();
        this.directoryTable.Add(new DirectoryTableEntry(er));
        for (int index = 0; index < (int) this.directoryTable[0].dirParentID - 1; ++index)
          this.directoryTable.Add(new DirectoryTableEntry(er));
        this.entryNameTable = new List<EntryNameTableEntry>();
        int num1 = 0;
        while (num1 < (int) this.directoryTable[0].dirParentID)
        {
          byte num2 = er.ReadByte();
          --er.BaseStream.Position;
          if (num2 == (byte) 0)
          {
            this.entryNameTable.Add((EntryNameTableEntry) new EntryNameTableEndOfDirectoryEntry(er));
            ++num1;
          }
          else if (num2 < (byte) 128)
            this.entryNameTable.Add((EntryNameTableEntry) new EntryNameTableFileEntry(er));
          else
            this.entryNameTable.Add((EntryNameTableEntry) new EntryNameTableDirectoryEntry(er));
        }
      }
    }

    public class ROM_OVT
    {
      public uint id;
      public uint ram_address;
      public uint ram_size;
      public uint bss_size;
      public uint sinit_init;
      public uint sinit_init_end;
      public uint file_id;
      public uint compressed;
      public NDS_new.ROM_OVT.OVT_Flag flag;

      public ROM_OVT(EndianBinaryReader er)
      {
        this.id = er.ReadUInt32();
        this.ram_address = er.ReadUInt32();
        this.ram_size = er.ReadUInt32();
        this.bss_size = er.ReadUInt32();
        this.sinit_init = er.ReadUInt32();
        this.sinit_init_end = er.ReadUInt32();
        this.file_id = er.ReadUInt32();
        uint num = er.ReadUInt32();
        this.compressed = num & 16777215U;
        this.flag = (NDS_new.ROM_OVT.OVT_Flag) (num >> 24);
      }

      [Flags]
      public enum OVT_Flag : byte
      {
        Compressed = 1,
        Authentication_Code = 2,
      }
    }

    public class BannerFile
    {
      private NDS_new.BannerFile.BannerHeader h;
      private NDS_new.BannerFile.BannerFileV1 v1;

      public BannerFile(EndianBinaryReader er)
      {
        this.h = new NDS_new.BannerFile.BannerHeader(er);
        this.v1 = new NDS_new.BannerFile.BannerFileV1(er);
      }

      public class BannerHeader
      {
        public byte version;
        public byte reserved_A;
        public ushort crc16_v1;
        public byte[] reserved_B;

        public BannerHeader(EndianBinaryReader er)
        {
          this.version = er.ReadByte();
          this.reserved_A = er.ReadByte();
          this.crc16_v1 = er.ReadUInt16();
          this.reserved_B = er.ReadBytes(28);
        }
      }

      public class BannerFileV1
      {
        public byte[] image;
        public byte[] pltt;
        public string[] gameName;

        public BannerFileV1(EndianBinaryReader er)
        {
          this.image = er.ReadBytes(512);
          this.pltt = er.ReadBytes(32);
          this.gameName = new string[6];
          this.gameName[0] = er.ReadString(Encoding.Unicode, 128).Replace("\0", "");
          this.gameName[1] = er.ReadString(Encoding.Unicode, 128).Replace("\0", "");
          this.gameName[2] = er.ReadString(Encoding.Unicode, 128).Replace("\0", "");
          this.gameName[3] = er.ReadString(Encoding.Unicode, 128).Replace("\0", "");
          this.gameName[4] = er.ReadString(Encoding.Unicode, 128).Replace("\0", "");
          this.gameName[5] = er.ReadString(Encoding.Unicode, 128).Replace("\0", "");
        }
      }
    }
  }
}
