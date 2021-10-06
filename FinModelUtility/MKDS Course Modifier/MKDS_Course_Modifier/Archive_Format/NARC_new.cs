// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Archive_Format.NARC_new
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Exceptions;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.IO.NitroFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace MKDS_Course_Modifier.Archive_Format
{
  public class NARC_new
  {
    public NARC_new.ArchiveHeader Header;
    public NARC_new.FileAllocationTableBlock FATB;
    public NARC_new.FilenameTableBlock FNTB;
    public NARC_new.FileImageBlock FIMG;

    private NARC_new()
    {
      this.Header = new NARC_new.ArchiveHeader();
      this.FATB = new NARC_new.FileAllocationTableBlock();
      this.FNTB = new NARC_new.FilenameTableBlock();
      this.FIMG = new NARC_new.FileImageBlock();
    }

    public NARC_new(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      try
      {
        this.Header = new NARC_new.ArchiveHeader(er);
        this.FATB = new NARC_new.FileAllocationTableBlock(er);
        this.FNTB = new NARC_new.FilenameTableBlock(er);
        this.FIMG = new NARC_new.FileImageBlock(er);
      }
      catch (SignatureNotCorrectException ex)
      {
        int num = (int) MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
      }
      finally
      {
        er.Close();
      }
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      this.FATB.Write(er);
      this.FNTB.Write(er);
      this.FIMG.Write(er);
      er.BaseStream.Position = 8L;
      er.Write((uint) er.BaseStream.Length);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public NARCFileSystem ToFileSystem()
    {
      List<FileSystem.Directory> directoryList = new List<FileSystem.Directory>();
      directoryList.Add(new FileSystem.Directory("/", true));
      directoryList[0].DirectoryID = (ushort) 61440;
      uint dirParentId = (uint) this.FNTB.directoryTable[0].dirParentID;
      for (int index = 1; (long) index < (long) dirParentId; ++index)
        directoryList.Add(new FileSystem.Directory((ushort) (61440 + index)));
      for (int index = 1; (long) index < (long) dirParentId; ++index)
        directoryList[index].Parent = directoryList[(int) this.FNTB.directoryTable[index].dirParentID - 61440];
      for (int index1 = 0; (long) index1 < (long) dirParentId; ++index1)
      {
        for (int index2 = 0; (long) index2 < (long) dirParentId; ++index2)
        {
          if (directoryList[index1] == directoryList[index2].Parent)
            directoryList[index1].SubDirectories.Add(directoryList[index2]);
        }
      }
      uint num = dirParentId * 8U;
      ushort dirEntryFileId = this.FNTB.directoryTable[0].dirEntryFileID;
      FileSystem.Directory Parent = (FileSystem.Directory) null;
      foreach (EntryNameTableEntry entryNameTableEntry in this.FNTB.entryNameTable)
      {
        for (int index = 1; (long) index < (long) dirParentId; ++index)
        {
          if ((int) num == (int) this.FNTB.directoryTable[index].dirEntryStart)
          {
            Parent = directoryList[index];
            break;
          }
        }
        switch (entryNameTableEntry)
        {
          case EntryNameTableEndOfDirectoryEntry _:
            Parent = (FileSystem.Directory) null;
            ++num;
            break;
          case EntryNameTableFileEntry _:
            Parent.Files.Add(new FileSystem.File((int) dirEntryFileId++, ((EntryNameTableFileEntry) entryNameTableEntry).entryName, Parent));
            num += 1U + (uint) entryNameTableEntry.entryNameLength;
            break;
          case EntryNameTableDirectoryEntry _:
            directoryList[(int) ((EntryNameTableDirectoryEntry) entryNameTableEntry).directoryID - 61440].DirectoryName = ((EntryNameTableDirectoryEntry) entryNameTableEntry).entryName;
            num += (uint) (3 + ((int) entryNameTableEntry.entryNameLength & (int) sbyte.MaxValue));
            break;
        }
      }
      for (int index = 0; index < (int) this.FATB.numFiles; ++index)
      {
        byte[] numArray = new byte[(IntPtr) this.FATB.allocationTable[index].fileSize];
        Array.Copy((Array) this.FIMG.fileImage, (long) this.FATB.allocationTable[index].fileTop, (Array) numArray, 0L, (long) numArray.Length);
        directoryList[0].GetFileByID((int) (ushort) index).Data = numArray;
      }
      return new NARCFileSystem(directoryList[0]);
    }

    public static NARC_new FromFileSystem(NARCFileSystem fileSystem)
    {
      NARC_new narcNew = new NARC_new();
      narcNew.FATB.numFiles = (ushort) fileSystem.Root.TotalNrSubFiles;
      List<byte> byteList = new List<byte>();
      for (ushort index = 0; (int) index < (int) narcNew.FATB.numFiles; ++index)
      {
        FileSystem.File fileById = fileSystem.Root.GetFileByID((int) index);
        narcNew.FATB.allocationTable.Add(new FileAllocationEntry((uint) byteList.Count, (uint) fileById.Data.Length));
        byteList.AddRange((IEnumerable<byte>) fileById.Data);
        while (byteList.Count % 4 != 0)
          byteList.Add(byte.MaxValue);
      }
      narcNew.FIMG.fileImage = byteList.ToArray();
      NARC_new.GenerateDirectoryTable(narcNew.FNTB.directoryTable, fileSystem.Root);
      uint dirEntryStart = narcNew.FNTB.directoryTable[0].dirEntryStart;
      ushort FileId = 0;
      NARC_new.GenerateEntryNameTable(narcNew.FNTB.directoryTable, narcNew.FNTB.entryNameTable, fileSystem.Root, ref dirEntryStart, ref FileId);
      return narcNew;
    }

    private static void GenerateDirectoryTable(
      List<DirectoryTableEntry> directoryTable,
      FileSystem.Directory dir)
    {
      DirectoryTableEntry directoryTableEntry = new DirectoryTableEntry();
      if (dir.IsRoot)
      {
        directoryTableEntry.dirParentID = (ushort) (dir.TotalNrSubDirectories + 1U);
        directoryTableEntry.dirEntryStart = (uint) directoryTableEntry.dirParentID * 8U;
      }
      else
        directoryTableEntry.dirParentID = dir.Parent.DirectoryID;
      dir.DirectoryID = (ushort) (61440 + directoryTable.Count);
      directoryTable.Add(directoryTableEntry);
      foreach (FileSystem.Directory subDirectory in dir.SubDirectories)
        NARC_new.GenerateDirectoryTable(directoryTable, subDirectory);
    }

    private static void GenerateEntryNameTable(
      List<DirectoryTableEntry> directoryTable,
      List<EntryNameTableEntry> entryNameTable,
      FileSystem.Directory dir,
      ref uint Offset,
      ref ushort FileId)
    {
      directoryTable[(int) dir.DirectoryID - 61440].dirEntryStart = Offset;
      directoryTable[(int) dir.DirectoryID - 61440].dirEntryFileID = FileId;
      foreach (FileSystem.Directory subDirectory in dir.SubDirectories)
      {
        entryNameTable.Add((EntryNameTableEntry) new EntryNameTableDirectoryEntry(subDirectory.DirectoryName, subDirectory.DirectoryID));
        Offset += (uint) (subDirectory.DirectoryName.Length + 3);
      }
      foreach (FileSystem.File file in dir.Files)
      {
        file.FileID = (int) FileId;
        entryNameTable.Add((EntryNameTableEntry) new EntryNameTableFileEntry(file.FileName));
        Offset += (uint) (file.FileName.Length + 1);
        ++FileId;
      }
      entryNameTable.Add((EntryNameTableEntry) new EntryNameTableEndOfDirectoryEntry());
      ++Offset;
      foreach (FileSystem.Directory subDirectory in dir.SubDirectories)
        NARC_new.GenerateEntryNameTable(directoryTable, entryNameTable, subDirectory, ref Offset, ref FileId);
    }

    public class ArchiveHeader
    {
      public string signature;
      public ushort byteOrder;
      public ushort version;
      public uint fileSize;
      public ushort headerSize;
      public ushort dataBlocks;

      public ArchiveHeader()
      {
        this.signature = "NARC";
        this.byteOrder = (ushort) 65534;
        this.version = (ushort) 256;
        this.fileSize = 0U;
        this.headerSize = (ushort) 16;
        this.dataBlocks = (ushort) 3;
      }

      public ArchiveHeader(EndianBinaryReader er)
      {
        this.signature = er.ReadString(Encoding.ASCII, 4);
        if (this.signature != "NARC")
          throw new SignatureNotCorrectException(this.signature, "NARC", er.BaseStream.Position);
        this.byteOrder = er.ReadUInt16();
        this.version = er.ReadUInt16();
        this.fileSize = er.ReadUInt32();
        this.headerSize = er.ReadUInt16();
        this.dataBlocks = er.ReadUInt16();
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.signature, Encoding.ASCII, false);
        er.Write(this.byteOrder);
        er.Write(this.version);
        er.Write(this.fileSize);
        er.Write(this.headerSize);
        er.Write(this.dataBlocks);
      }
    }

    public class FileAllocationTableBlock
    {
      public string kind;
      public uint size;
      public ushort numFiles;
      public ushort reserved;
      public List<FileAllocationEntry> allocationTable;

      public FileAllocationTableBlock()
      {
        this.kind = "BTAF";
        this.size = 0U;
        this.numFiles = (ushort) 0;
        this.reserved = (ushort) 0;
        this.allocationTable = new List<FileAllocationEntry>();
      }

      public FileAllocationTableBlock(EndianBinaryReader er)
      {
        this.kind = er.ReadString(Encoding.ASCII, 4);
        if (this.kind != "BTAF")
          throw new SignatureNotCorrectException(this.kind, "BTAF", er.BaseStream.Position);
        this.size = er.ReadUInt32();
        this.numFiles = er.ReadUInt16();
        this.reserved = er.ReadUInt16();
        this.allocationTable = new List<FileAllocationEntry>();
        for (int index = 0; index < (int) this.numFiles; ++index)
          this.allocationTable.Add(new FileAllocationEntry(er));
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.kind, Encoding.ASCII, false);
        er.Write((uint) (12 + this.allocationTable.Count * 8));
        er.Write((ushort) this.allocationTable.Count);
        er.Write(this.reserved);
        foreach (FileAllocationEntry fileAllocationEntry in this.allocationTable)
          fileAllocationEntry.Write(er);
      }
    }

    public class FilenameTableBlock
    {
      public string kind;
      public uint size;
      public List<DirectoryTableEntry> directoryTable;
      public List<EntryNameTableEntry> entryNameTable;

      public FilenameTableBlock()
      {
        this.kind = "BTNF";
        this.size = 0U;
        this.directoryTable = new List<DirectoryTableEntry>();
        this.entryNameTable = new List<EntryNameTableEntry>();
      }

      public FilenameTableBlock(EndianBinaryReader er)
      {
        this.kind = er.ReadString(Encoding.ASCII, 4);
        if (this.kind != "BTNF")
          throw new SignatureNotCorrectException(this.kind, "BTNF", er.BaseStream.Position);
        this.size = er.ReadUInt32();
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
        while (er.BaseStream.Position % 4L != 0L)
        {
          int num3 = (int) er.ReadByte();
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        er.Write(this.kind, Encoding.ASCII, false);
        er.Write(0U);
        foreach (DirectoryTableEntry directoryTableEntry in this.directoryTable)
          directoryTableEntry.Write(er);
        foreach (EntryNameTableEntry entryNameTableEntry in this.entryNameTable)
          entryNameTableEntry.Write(er);
        while (er.BaseStream.Position % 4L != 0L)
          er.Write(byte.MaxValue);
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position2 - position1));
        er.BaseStream.Position = position2;
      }
    }

    public class FileImageBlock
    {
      public string kind;
      public uint size;
      public byte[] fileImage;

      public FileImageBlock()
      {
        this.kind = "GMIF";
        this.size = 0U;
      }

      public FileImageBlock(EndianBinaryReader er)
      {
        this.kind = er.ReadString(Encoding.ASCII, 4);
        if (this.kind != "GMIF")
          throw new SignatureNotCorrectException(this.kind, "GMIF", er.BaseStream.Position);
        this.size = er.ReadUInt32();
        this.fileImage = er.ReadBytes((int) this.size - 8);
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.kind, Encoding.ASCII, false);
        er.Write((uint) (this.fileImage.Length + 8));
        er.Write(this.fileImage, 0, this.fileImage.Length);
      }
    }
  }
}
