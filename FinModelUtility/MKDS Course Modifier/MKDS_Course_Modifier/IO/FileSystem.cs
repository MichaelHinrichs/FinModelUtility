// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.FileSystem
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;
using System.IO;

namespace MKDS_Course_Modifier.IO
{
  public class FileSystem
  {
    public FileSystem.Directory Root;

    public virtual byte[] Write()
    {
      return (byte[]) null;
    }

    public class File
    {
      public string FileName;
      public int FileID;
      public byte[] Data;
      public FileSystem.Directory Parent;

      public File(int Id, string Name, FileSystem.Directory Parent)
      {
        this.FileID = Id;
        this.FileName = Name;
        this.Parent = Parent;
      }

      public bool IsVirtualFile { get; private set; }

      public override string ToString()
      {
        return this.Parent.ToString() + this.FileName;
      }
    }

    public class Directory
    {
      public string DirectoryName;
      public ushort DirectoryID;
      public FileSystem.Directory Parent;
      public List<FileSystem.Directory> SubDirectories;
      public List<FileSystem.File> Files;

      public Directory(string Name, bool Root)
      {
        this.DirectoryName = Name;
        this.IsRoot = Root;
        this.SubDirectories = new List<FileSystem.Directory>();
        this.Files = new List<FileSystem.File>();
      }

      public Directory(ushort Id)
      {
        this.DirectoryID = Id;
        this.IsRoot = false;
        this.SubDirectories = new List<FileSystem.Directory>();
        this.Files = new List<FileSystem.File>();
      }

      public bool IsRoot { get; set; }

      public bool IsVirtualDirectory { get; set; }

      public FileSystem.File this[string index]
      {
        get
        {
          foreach (FileSystem.File file in this.Files)
          {
            if (file.FileName == index)
              return file;
          }
          return (FileSystem.File) null;
        }
        set
        {
          for (int index1 = 0; index1 < this.Files.Count; ++index1)
          {
            if (this.Files[index1].FileName == index)
            {
              this.Files[index1] = value;
              return;
            }
          }
          throw new FileNotFoundException("The file " + index + " does not exist in this directory.");
        }
      }

      public uint TotalNrSubDirectories
      {
        get
        {
          uint count = (uint) this.SubDirectories.Count;
          foreach (FileSystem.Directory subDirectory in this.SubDirectories)
            count += subDirectory.TotalNrSubDirectories;
          return count;
        }
      }

      public uint TotalNrSubFiles
      {
        get
        {
          uint count = (uint) this.Files.Count;
          foreach (FileSystem.Directory subDirectory in this.SubDirectories)
            count += subDirectory.TotalNrSubFiles;
          return count;
        }
      }

      public FileSystem.File GetFileByID(int Id)
      {
        foreach (FileSystem.File file in this.Files)
        {
          if (file.FileID == Id)
            return file;
        }
        foreach (FileSystem.Directory subDirectory in this.SubDirectories)
        {
          FileSystem.File fileById = subDirectory.GetFileByID(Id);
          if (fileById != null)
            return fileById;
        }
        return (FileSystem.File) null;
      }

      public override string ToString()
      {
        return !this.IsRoot ? this.Parent.ToString() + this.DirectoryName + "/" : "/";
      }
    }
  }
}
