// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Archive_Format.NARC
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Archive_Format
{
  public class NARC
  {
    public static void Unpack(string infile, string outdir)
    {
      NARC.Unpack(infile).Extract(outdir);
    }

    public static NARC.DirectoryEntry Unpack(string infile)
    {
      return NARC.Unpack(System.IO.File.ReadAllBytes(infile));
    }

    public static NARC.DirectoryEntry Unpack(byte[] infile)
    {
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      List<string> source1 = new List<string>();
      List<int> intList3 = new List<int>();
      List<string> source2 = new List<string>();
      List<int> source3 = new List<int>();
      List<int> intList4 = new List<int>();
      NARC.DirectoryEntry directoryEntry1 = new NARC.DirectoryEntry("root", 0);
      NARC.DirectoryEntry directoryEntry2 = directoryEntry1;
      bool flag = true;
      EndianBinaryReader endianBinaryReader = new EndianBinaryReader((Stream) new MemoryStream(infile), Endianness.LittleEndian);
      if (endianBinaryReader.ReadString(Encoding.ASCII, 4) == nameof (NARC))
      {
        endianBinaryReader.ReadBytes(4);
        endianBinaryReader.ReadInt32();
        int num1 = (int) endianBinaryReader.ReadInt16();
        int num2 = (int) endianBinaryReader.ReadInt16();
        if (endianBinaryReader.ReadString(Encoding.ASCII, 4) == "BTAF")
        {
          endianBinaryReader.ReadInt32();
          int num3 = endianBinaryReader.ReadInt32();
          for (int index = 0; index < num3; ++index)
          {
            intList1.Add(endianBinaryReader.ReadInt32());
            intList2.Add(endianBinaryReader.ReadInt32());
          }
          if (endianBinaryReader.ReadString(Encoding.ASCII, 4) == "BTNF")
          {
            int id1;
            if (endianBinaryReader.ReadInt32() == 16)
            {
              for (id1 = 0; id1 < num3; ++id1)
                directoryEntry1.Files.Add(new NARC.FileEntry("Unknown file " + id1.ToString() + ".bin", id1));
              flag = false;
              endianBinaryReader.ReadBytes(8);
            }
            else
            {
              int position = (int) endianBinaryReader.BaseStream.Position;
              int num4 = endianBinaryReader.ReadInt32();
              int num5 = (int) endianBinaryReader.ReadInt16();
              int num6 = (int) endianBinaryReader.ReadInt16();
              endianBinaryReader.ReadInt32();
              int num7 = (int) endianBinaryReader.ReadInt16();
              int num8 = (int) endianBinaryReader.ReadInt16();
              int id2 = 0;
              endianBinaryReader.BaseStream.Position = (long) (position + num4);
              for (int index = 0; index < num3 + (num6 - 1); ++index)
              {
                byte num9 = endianBinaryReader.ReadByte();
                if (num9 < (byte) 128 && num9 > (byte) 0)
                {
                  source1.Add(endianBinaryReader.ReadString(Encoding.ASCII, (int) num9));
                  intList3.Add(id2);
                  directoryEntry2.Files.Add(new NARC.FileEntry(source1.Last<string>(), intList3.Count - 1));
                }
                else if (num9 > (byte) 128 && num9 < (byte) 176)
                {
                  byte num10 = (byte) ((uint) num9 - 128U);
                  source2.Add(endianBinaryReader.ReadString(Encoding.ASCII, (int) num10));
                  source3.Add((int) endianBinaryReader.ReadByte());
                  intList4.Add(id2);
                  int num11 = (int) endianBinaryReader.ReadByte();
                  directoryEntry2.Subdirs.Add(new NARC.DirectoryEntry(source2.Last<string>(), source3.Last<int>()));
                }
                else
                {
                  ++id2;
                  directoryEntry2 = directoryEntry1.GetDirById(id2);
                  --index;
                }
              }
            }
            while (!(endianBinaryReader.ReadString(Encoding.ASCII, 4) == "GMIF"))
              endianBinaryReader.BaseStream.Position -= 3L;
            endianBinaryReader.ReadInt32();
            int position1 = (int) endianBinaryReader.BaseStream.Position;
            for (id1 = 0; id1 < num3; ++id1)
            {
              endianBinaryReader.BaseStream.Position = (long) (position1 + intList1[id1]);
              directoryEntry1.SetFile(endianBinaryReader.ReadBytes(intList2[id1] - intList1[id1]), id1);
            }
          }
          else
          {
            int num12 = (int) MessageBox.Show("Error2", "error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          }
        }
        else
        {
          int num13 = (int) MessageBox.Show("Error1", "error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
      }
      else
      {
        int num = (int) MessageBox.Show("It isn't a narc file, or it is compressed.", "error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      endianBinaryReader.Close();
      return directoryEntry1;
    }

    public static void Pack(NARC.DirectoryEntry indir, string outfile)
    {
      System.IO.File.Create(outfile).Close();
      System.IO.File.WriteAllBytes(outfile, NARC.Pack(indir));
    }

    public static byte[] Pack(NARC.DirectoryEntry indir)
    {
      MemoryStream memoryStream1 = new MemoryStream();
      EndianBinaryWriter endianBinaryWriter = new EndianBinaryWriter((Stream) memoryStream1, Endianness.LittleEndian);
      endianBinaryWriter.Write(nameof (NARC), Encoding.ASCII, false);
      endianBinaryWriter.Write(new byte[4]
      {
        (byte) 254,
        byte.MaxValue,
        (byte) 0,
        (byte) 1
      }, 0, 4);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write((short) 16);
      endianBinaryWriter.Write((short) 3);
      endianBinaryWriter.Write("BTAF", Encoding.ASCII, false);
      int nrOfFiles = indir.NrOfFiles;
      endianBinaryWriter.Write(12 + nrOfFiles * 8);
      endianBinaryWriter.Write(nrOfFiles);
      int num = 0;
      for (int id = 0; id < nrOfFiles; ++id)
      {
        endianBinaryWriter.Write(num);
        num += indir.GetFile(id).Content.Length;
        endianBinaryWriter.Write(num);
        while (num % 4 != 0)
          ++num;
      }
      MemoryStream memoryStream2 = new MemoryStream();
      EndianBinaryWriter er2 = new EndianBinaryWriter((Stream) memoryStream2, Endianness.LittleEndian);
      MemoryStream memoryStream3 = new MemoryStream();
      EndianBinaryWriter er4 = new EndianBinaryWriter((Stream) memoryStream3, Endianness.LittleEndian);
      er4.Write(8 + indir.NrOfDirs * 8);
      er4.Write((short) 0);
      er4.Write((short) (indir.NrOfDirs + 1));
      List<sbyte> mapID_ = new List<sbyte>();
      List<sbyte> sbyteList1 = new List<sbyte>();
      List<sbyte> sbyteList2 = new List<sbyte>();
      sbyte mapID = 1;
      short items = 0;
      indir.GetNarcData(ref er2, ref er4, true, (sbyte) 1, indir, ref items, ref mapID_, ref mapID, 0);
      er2.Write((byte) 0);
      while (er2.BaseStream.Length % 4L != 0L)
        er2.Write(byte.MaxValue);
      er2.Close();
      er4.Close();
      endianBinaryWriter.Write("BTNF", Encoding.ASCII, false);
      endianBinaryWriter.Write(memoryStream2.ToArray().Length + memoryStream3.ToArray().Length + 8);
      byte[] array1 = memoryStream3.ToArray();
      endianBinaryWriter.Write(array1, 0, array1.Length);
      byte[] array2 = memoryStream2.ToArray();
      endianBinaryWriter.Write(array2, 0, array2.Length);
      MemoryStream memoryStream4 = new MemoryStream();
      EndianBinaryWriter er3 = new EndianBinaryWriter((Stream) memoryStream4, Endianness.LittleEndian);
      indir.WriteFileData(ref er3);
      er3.Close();
      endianBinaryWriter.Write("GMIF", Encoding.ASCII, false);
      endianBinaryWriter.Write(memoryStream4.ToArray().Length + 8);
      byte[] array3 = memoryStream4.ToArray();
      endianBinaryWriter.Write(array3, 0, array3.Length);
      endianBinaryWriter.BaseStream.Position = 8L;
      endianBinaryWriter.Write((int) endianBinaryWriter.BaseStream.Length);
      byte[] array4 = memoryStream1.ToArray();
      endianBinaryWriter.Close();
      return array4;
    }

    public class DirectoryEntry
    {
      public List<NARC.DirectoryEntry> Subdirs = new List<NARC.DirectoryEntry>();
      public List<NARC.FileEntry> Files = new List<NARC.FileEntry>();
      public string Name;
      public int Id;

      public DirectoryEntry(string name, int id)
      {
        this.Name = name;
        this.Id = id;
      }

      public NARC.DirectoryEntry GetDirById(int id)
      {
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          if (this.Subdirs[index].Id == id)
            return this.Subdirs[index];
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          if (this.Subdirs[index].GetDirById(id) != null)
            return this.Subdirs[index].GetDirById(id);
        }
        return (NARC.DirectoryEntry) null;
      }

      public void Extract(string path)
      {
        for (int index = 0; index < this.Subdirs.Count; ++index)
          System.IO.Directory.CreateDirectory(path + "\\" + this.Subdirs[index].Name);
        for (int index = 0; index < this.Files.Count; ++index)
        {
          System.IO.File.Create(path + "\\" + this.Files[index].Name).Close();
          System.IO.File.WriteAllBytes(path + "\\" + this.Files[index].Name, this.Files[index].Content);
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
          this.Subdirs[index].Extract(path + "\\" + this.Subdirs[index].Name);
      }

      public bool SetFile(byte[] bytes, int id)
      {
        for (int index = 0; index < this.Files.Count; ++index)
        {
          if (this.Files[index].Id == id)
          {
            this.Files[index].Content = bytes;
            return true;
          }
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          if (this.Subdirs[index].SetFile(bytes, id))
            return true;
        }
        return false;
      }

      public bool SetFile(NARC.FileEntry f)
      {
        for (int index = 0; index < this.Files.Count; ++index)
        {
          if (this.Files[index].Id == f.Id)
          {
            this.Files[index] = f;
            return true;
          }
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          if (this.Subdirs[index].SetFile(f))
            return true;
        }
        return false;
      }

      public int NrOfFiles
      {
        get
        {
          int count = this.Files.Count;
          for (int index = 0; index < this.Subdirs.Count; ++index)
            count += this.Subdirs[index].NrOfFiles;
          return count;
        }
      }

      public int NrOfDirs
      {
        get
        {
          int count = this.Subdirs.Count;
          for (int index = 0; index < this.Subdirs.Count; ++index)
            count += this.Subdirs[index].NrOfDirs;
          return count;
        }
      }

      public NARC.FileEntry GetFile(int id)
      {
        for (int index = 0; index < this.Files.Count; ++index)
        {
          if (this.Files[index].Id == id)
            return this.Files[index];
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          NARC.FileEntry file = this.Subdirs[index].GetFile(id);
          if (file != null)
            return file;
        }
        return (NARC.FileEntry) null;
      }

      public void GetNarcData(
        ref EndianBinaryWriter er2,
        ref EndianBinaryWriter er4,
        bool isRoot,
        sbyte depth,
        NARC.DirectoryEntry Root,
        ref short items,
        ref List<sbyte> mapID_,
        ref sbyte mapID,
        int j = 0)
      {
        if (isRoot)
        {
          for (int index = 0; index < this.Subdirs.Count; ++index)
          {
            mapID_.Add(mapID);
            er2.Write((sbyte) (this.Subdirs[index].Name.Length + 128));
            er2.Write(this.Subdirs[index].Name, Encoding.ASCII, false);
            er2.Write(mapID);
            er2.Write((byte) 240);
            mapID += (sbyte) (this.Subdirs[index].Subdirs.Count + 1);
          }
          for (int index = 0; index < this.Files.Count; ++index)
          {
            er2.Write((sbyte) this.Files[index].Name.Length);
            er2.Write(this.Files[index].Name, Encoding.ASCII, false);
            ++items;
          }
          List<sbyte> mapID_1 = new List<sbyte>();
          sbyte mapID1 = (sbyte) ((int) depth + 1);
          for (int j1 = 0; j1 < this.Subdirs.Count; ++j1)
          {
            er2.Write((byte) 0);
            er4.Write((int) er2.BaseStream.Position + (8 + Root.NrOfDirs * 8));
            er4.Write(items);
            er4.Write((sbyte) 0);
            er4.Write((byte) 240);
            this.Subdirs[j1].GetNarcData(ref er2, ref er4, false, (sbyte) ((int) depth + 1), Root, ref items, ref mapID_1, ref mapID1, j1);
          }
        }
        else
        {
          for (int index = 0; index < this.Subdirs.Count; ++index)
          {
            mapID_.Add(mapID);
            er2.Write((sbyte) (this.Subdirs[index].Name.Length + 128));
            er2.Write(this.Subdirs[index].Name, Encoding.ASCII, false);
            er2.Write(mapID);
            er2.Write((byte) 240);
            mapID += (sbyte) (this.Subdirs[index].Subdirs.Count + 1);
          }
          if (j != Root.Subdirs.Count && depth == (sbyte) 2)
            ++mapID;
          for (int index = 0; index < this.Files.Count; ++index)
          {
            er2.Write((sbyte) this.Files[index].Name.Length);
            er2.Write(this.Files[index].Name, Encoding.ASCII, false);
            ++items;
          }
          sbyte mapID1 = (sbyte) ((int) depth + 1);
          List<sbyte> mapID_1 = new List<sbyte>();
          for (int index = 0; index < this.Subdirs.Count; ++index)
          {
            er2.Write((byte) 0);
            er4.Write((int) er2.BaseStream.Position + (8 + Root.NrOfDirs * 8));
            er4.Write(items);
            er4.Write(mapID_[index]);
            er4.Write((byte) 240);
            this.Subdirs[index].GetNarcData(ref er2, ref er4, false, (sbyte) ((int) depth + 1), Root, ref items, ref mapID_1, ref mapID1, 0);
          }
        }
      }

      public void WriteFileData(ref EndianBinaryWriter er3)
      {
        for (int index = 0; index < this.Files.Count; ++index)
        {
          er3.Write(this.Files[index].Content, 0, this.Files[index].Content.Length);
          while ((double) er3.BaseStream.Length % 4.0 != 0.0)
            er3.Write(byte.MaxValue);
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
          this.Subdirs[index].WriteFileData(ref er3);
      }

      public void GetDirectoryTree(ref TreeNodeCollection t)
      {
        TreeNodeCollection nodes = t.Add(this.Name).Nodes;
        for (int index = 0; index < this.Subdirs.Count; ++index)
          this.Subdirs[index].GetDirectoryTree(ref nodes);
      }

      public NARC.DirectoryEntry GetDirectoryByPath(string Path)
      {
        if (Path == this.Name.Replace("root", "\\"))
          return this;
        if (!Path.StartsWith(this.Name.Replace("root", "\\")))
          return (NARC.DirectoryEntry) null;
        Path = Path.Remove(0, this.Name.Replace("root", "\\").Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          NARC.DirectoryEntry directoryByPath = this.Subdirs[index].GetDirectoryByPath(string.Join("\\", strArray));
          if (directoryByPath != null)
            return directoryByPath;
        }
        return (NARC.DirectoryEntry) null;
      }

      public NARC.FileEntry GetFileByPath(string Path)
      {
        if (!Path.StartsWith(this.Name.Replace("root", "\\")))
          return (NARC.FileEntry) null;
        Path = Path.Remove(0, this.Name.Replace("root", "\\").Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1)
        {
          for (int index = 0; index < this.Files.Count; ++index)
          {
            if (this.Files[index].Name == strArray[0])
              return this.Files[index];
          }
        }
        for (int index = 0; index < this.Subdirs.Count; ++index)
        {
          NARC.FileEntry fileByPath = this.Subdirs[index].GetFileByPath(string.Join("\\", strArray));
          if (fileByPath != null)
            return fileByPath;
        }
        return (NARC.FileEntry) null;
      }

      public void GetDirectoryContents(ListView l)
      {
        foreach (NARC.DirectoryEntry subdir in this.Subdirs)
          l.Items.Add(new ListViewItem(subdir.Name, 0)
          {
            Group = l.Groups[0]
          });
        foreach (NARC.FileEntry file1 in this.Files)
        {
          ListViewItem l1 = new ListViewItem(file1.Name);
          l1.SubItems.Add(FileSize.FormatSize(file1.Content.Length));
          ByteFileInfo file2 = new ByteFileInfo(file1);
          string type = FileHandler.GetType(file2);
          if (file2.IsLZ77Compressed)
            l1.ForeColor = Color.Blue;
          file2.Dispose();
          FileHandler.SetListViewItemInfo(l1, type, l);
          l.Items.Add(l1);
        }
      }

      public void RefreshIds(ref int startidx)
      {
        foreach (NARC.FileEntry file in this.Files)
          file.Id = startidx++;
        foreach (NARC.DirectoryEntry subdir in this.Subdirs)
          subdir.RefreshIds(ref startidx);
      }
    }

    public class FileEntry
    {
      public int Id;
      public string Name;
      public byte[] Content;

      public FileEntry(string name, int id)
      {
        this.Name = name;
        this.Id = id;
      }
    }
  }
}
